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

    /// <summary>
    /// 牌堆期望收益，仅服务器可获取
    /// </summary>
    public int AiCardExpectation {
        get {
            if (PNetworkManager.CurrentHostType.Equals(PHostType.Server)) {
                return PAiCardExpectation.Expect(PNetworkManager.NetworkServer.Game, this);
            } else {
                return 0;
            }
        }
    }

    public PPlayerCardArea Area {
        get {
            if (PNetworkManager.CurrentHostType.Equals(PHostType.Server)) {
                return PNetworkManager.NetworkServer.Game.CardManager.PlayerAreaList[Index];
            } else {
                return null;
            }
        }
    }
    
    /// <summary>
    /// 客户端专用，用于更新手牌数量
    /// </summary>
    public int HandCardNumber = 0;


    private string _EquipCards = string.Empty;
    public int EquipCardNumber {
        get {
            if (PNetworkManager.CurrentHostType.Equals(PHostType.Client)) {
                return _EquipCards.Length;
            } else {
                return Area.EquipmentCardArea.CardNumber;
            }
        }
    }

}