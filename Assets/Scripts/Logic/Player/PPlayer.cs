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

    public PTagManager Tags;

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

    public bool HasHouse {
        get {
            if (PNetworkManager.CurrentHostType.Equals(PHostType.Server)) {
                return PNetworkManager.Game.Map.BlockList.Exists((PBlock Block ) => Equals(Block.Lord) && Block.HouseNumber > 0);
            } else {
                return false;
            }
        }
    }

    public bool BackFace {
        get {
            return Tags.ExistTag(PTag.BackFaceTag.Name);
        }
    }

    public bool OutOfGame {
        get {
            return Tags.ExistTag(PTag.OutOfGameTag.Name);
        }
    }

    public bool NoLadder {
        get {
            return Tags.ExistTag(PTag.NoLadderTag.Name);
        }
    }

    /// <summary>
    /// 客户端专用，用于更新手牌数量
    /// </summary>
    public int HandCardNumber = 0;
    /// <summary>
    /// 客户端专用，用于更新领地数量
    /// </summary>
    public int NormalLandNumber = 0;
    /// <summary>
    /// 客户端专用，用于更新商业用地数量
    /// </summary>
    public int BusinessLandNumber = 0;
    /// <summary>
    /// 客户端专用，用于设置标记域
    /// </summary>
    public string MarkString = "|";

    /// <summary>
    /// 服务器端专用，用于设置标记域string
    /// </summary>
    /// <returns></returns>
    public string GetMarkString() {
        return Tags.TagString;
    }


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