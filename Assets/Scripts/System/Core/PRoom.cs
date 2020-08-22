using System.Collections.Generic;

public class PRoom {

    public class PlayerInRoom {
        public string Nickname = string.Empty;
        public PPlayerType PlayerType = PPlayerType.Waiting;
        public string IPAddress = string.Empty;
    }

    public List<PlayerInRoom> PlayerList;

    /// <summary>
    /// 创建空房间（全部Waiting）
    /// </summary>
    /// <param name="_Capacity">房间容量</param>
    public PRoom(int _Capacity) {
        PlayerList = new List<PlayerInRoom>();
        for (int i = 0; i < _Capacity; ++i) {
            PlayerList.Add(new PlayerInRoom());
        }
    }

    public PRoom(PMode Mode) : this(Mode.PlayerNumber) {
        for (int i = 0; i < Mode.PlayerNumber; ++i) {
            PlayerList[i].PlayerType = Mode.Seats[i].DefaultType;
            PLogger.Log((i + 1).ToString() + " NICKNAME=" + PlayerList[i].Nickname + " NAME=" + Mode.Seats[i].Name);
            if (PlayerList[i].Nickname.Equals(string.Empty)) {
                PlayerList[i].Nickname = Mode.Seats[i].Name;
            }   
        }
    }

    /// <summary>
    /// 房间的容量
    /// </summary>
    public int Capacity {
        get {
            return PlayerList.Count;
        }
    }

    /// <summary>
    /// 安全地获取房间内的玩家
    /// </summary>
    public PlayerInRoom GetPlayer(int Index) {
        if (Index <0 || Index >= Capacity) {
            return null;
        } else {
            return PlayerList[Index];
        }
    }

    /// <summary>
    /// 在房间中加入一个玩家
    /// </summary>
    /// <param name="NickName">新加入玩家的昵称</param>
    /// <param name="IPAddress">新加入玩家的IP地址</param>
    /// <returns>是否加入成功（若房间已满则不成功）</returns>
    public bool AddPlayer(string NickName, string IPAddress) {
        lock (PlayerList) {
            foreach (PlayerInRoom Player in PlayerList) {
                if (Player.PlayerType.Equals(PPlayerType.Waiting)) {
                    Player.Nickname = NickName;
                    Player.IPAddress = IPAddress;
                    Player.PlayerType = PPlayerType.Player;
                    return true;
                }
            }
            return false;
        }
    }

    /// <summary>
    /// 交换两个位置上的玩家/AI/空位
    /// </summary>
    /// <param name="From">位置1</param>
    /// <param name="To">位置2</param>
    public void MovePlayer(int From, int To) {
        if (0 <= From && From < Capacity && 0 <= To && To < Capacity) {
            lock (PlayerList) {
                PlayerInRoom Temp = PlayerList[From];
                PlayerList[From] = PlayerList[To];
                PlayerList[To] = Temp;
            }
        }
    }

    /// <summary>
    /// 切换座位的属性（AI和Waiting）
    /// </summary>
    /// <param name="Index">目标座位的序号</param>
    public void SwitchSeatAttribute(int Index) {
        if (0 <= Index && Index < Capacity) {
            lock (PlayerList) {
                if (PlayerList[Index].PlayerType.Equals(PPlayerType.Waiting)) {
                    PlayerList[Index].PlayerType = PPlayerType.AI;
                } else if (PlayerList[Index].PlayerType.Equals(PPlayerType.AI)) {
                    PlayerList[Index].PlayerType = PPlayerType.Waiting;
                }
            }
        }
    }

    /// <summary>
    /// 删除一个玩家并重置其位置
    /// </summary>
    /// <param name="Index">目标座位的序号</param>
    public void RemovePlayer(int Index) {
        lock (PlayerList) {
            PlayerList[Index] = new PlayerInRoom();
        }
    }

    /// <summary>
    /// 断线一名玩家，保留IP地址，但属性暂时变成AI
    /// </summary>
    /// <param name="Index">目标座位的序号</param>
    public void DisconnectPlayer(int Index) {
        lock (PlayerList) {
            PlayerList[Index].PlayerType = PPlayerType.AI;
        }
    }

    public int FindIndexByIPAddress(string IPAddress) {
        for (int i = 0; i < Capacity; ++i) {
            if (PlayerList[i].IPAddress.Equals(IPAddress)) {
                return i;
            }
        }
        return -1;
    }
    /// <summary>
    /// 删除一个玩家并重置其位置
    /// </summary>
    /// <param name="IPAddress">玩家的IP地址，若找不到该玩家则无操作</param>
    public void RemovePlayer(string IPAddress) {
        int index = FindIndexByIPAddress(IPAddress);
        if (index >= 0) {
            RemovePlayer(index);
        }
    }

    /// <summary>
    /// 是否已经被人和AI填满
    /// </summary>
    public bool IsFull() {
        return PlayerList.TrueForAll((PlayerInRoom Player) => 
            Player.PlayerType.Equals(PPlayerType.AI) || Player.PlayerType.Equals(PPlayerType.Player));
    }

    public bool AllAi() {
        return PlayerList.TrueForAll((PlayerInRoom Player) => Player.PlayerType.Equals(PPlayerType.AI));
    }

    #region 重载ToString以用来在TCP上发送
    /*
     * The format is:
     * Capaity
     * Space between {
     *      PPlayerType[i] (string form)
     *      Nickname[i] (if empty, give "&")
     * }
     */
    public override string ToString() {
        string[] tempRoomStringList = new string[Capacity];
        for (int i = 0; i < Capacity; ++i) {
            tempRoomStringList[i] = PlayerList[i].PlayerType.ToString();
            if (!PlayerList[i].Nickname.Equals(string.Empty)) {
                tempRoomStringList[i] += " " + PlayerList[i].Nickname;
            } else {
                tempRoomStringList[i] += " " + "&";
            }
        }
        return Capacity + " " + string.Join(" ", tempRoomStringList);
    }
    #endregion
}
