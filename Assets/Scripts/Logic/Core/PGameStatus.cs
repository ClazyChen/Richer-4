using System.Collections.Generic;

public class PGameStatus {
    public PMap Map;
    public PMode GameMode;


    public int PlayerNumber {
        get {
            return GameMode.PlayerNumber;
        }
    }

    public List<PPlayer> PlayerList;
    public PPlayer NowPlayer;
    public PPeriod NowPeriod;

    public int NowPlayerIndex {
        get {
            return PlayerList.FindIndex((PPlayer Player) => Player.Equals(NowPlayer));
        }
        set {
            if (0 <= value && value < PlayerList.Count) {
                NowPlayer = PlayerList[value];
            }
        }
    }

    public PPlayer GetNextPlayer(PPlayer Player) {
        int Index = Player.Index + 1;
        if (Index >= PlayerNumber) {
            Index = 0;
        }
        while (!PlayerList[Index].IsAlive) {
            ++Index;
            if (Index == Player.Index + 1) {
                return Player;
            } else if (Index >= PlayerNumber) {
                Index = 0;
            }
        }
        return PlayerList[Index];
    }

    /// <summary>
    /// 新建一个游戏状态
    /// </summary>
    /// <param name="_Map">原型地图（新建的游戏采用其复制品）</param>
    /// <param name="_GameMode">游戏模式</param>
    public PGameStatus(PMap _Map, PMode _GameMode) {
        Map = (PMap)_Map.Clone();
        GameMode = _GameMode;
    }

    // 这个是Client端的StartGame
    // Server的StartGame在PGame里
    public void StartGame() {
        #region 初始化玩家列表
        PlayerList = new List<PPlayer>();
        for (int i = 0; i < PlayerNumber; ++i) {
            PlayerList.Add(new PPlayer() {
                Index = i,
                Name = PSystem.CurrentRoom.PlayerList[i].PlayerType.Equals(PPlayerType.Player) ? PSystem.CurrentRoom.PlayerList[i].Nickname : "P" + (i + 1).ToString(),
                IsAlive = true,
                Money = PPlayer.Config.DefaultMoney,
                TeamIndex = GameMode.Seats[i].Party - 1
            });
            PBlock Position = Map.BlockList.Find((PBlock Block) => Block.StartPointIndex == i % Map.StartPointNumber);
            if (Position != null) {
                PlayerList[i].Position = Position;
            } else {
                PlayerList[i].Position = Map.BlockList[0];
            }
        }
        #endregion
        NowPlayer = null;
        NowPeriod = null;
    }
}
