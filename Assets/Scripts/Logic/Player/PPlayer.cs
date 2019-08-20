public class PPlayer: PObject {
    public class Config {
        public static int DefaultMoney = 30000;
    }

    public int Index;
    public int Money = 0;
    public PSex Sex = PSex.NoSex;
    public bool IsAlive = true;
    public int TeamIndex;
    public PBlock Position;

    /// <summary>
    /// 玩家的IP地址，只有服务器可以获取，缺省为空串
    /// </summary>
    public string IPAddress {
        get {
            if (PNetworkManager.CurrentHostType.Equals(PHostType.Server)) {
                return PNetworkManager.NetworkServer.Game.Room.GetPlayer(Index).IPAddress;
            } else {
                return string.Empty;
            }
        }
    }
    /// <summary>
    /// 是否为真实玩家，只有服务器可以获取，缺省为false
    /// </summary>
    public bool IsUser {
        get {
            if (PNetworkManager.CurrentHostType.Equals(PHostType.Server)) {
                return PNetworkManager.NetworkServer.Game.Room.GetPlayer(Index).PlayerType.Equals(PPlayerType.Player);
            } else {
                return false;
            }
        }
    }
    public bool IsAI {
        get {
            if (PNetworkManager.CurrentHostType.Equals(PHostType.Server)) {
                return !IsUser;
            } else {
                return false;
            }
        }
    }

}