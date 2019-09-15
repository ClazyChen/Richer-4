using System;
public class PPlayer: PObject {
    public class Config {
        public static int DefaultMoney = 30000;
    }

    public int Index;
    public int Money = 0;
    public PSex Sex = PSex.NoSex;
    public PAge Age = PAge.NoAge;
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

    public int Distance(PPlayer Another) {
        return Math.Abs(Position.X - Another.Position.X) + Math.Abs(Position.Y - Another.Position.Y);
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

    public bool CanBeInjured {
        get {
            return !OutOfGame && !Tags.ExistTag(PKuungCheevngChiTag.TagName);
        }
    }

    public bool NoLadder {
        get {
            return Tags.ExistTag(PTag.NoLadderTag.Name);
        }
    }

    public int PurchaseLimit {
        get {
            int Base = 1;
            PCard Weapon = GetEquipment(PCardType.WeaponCard);
            if (Weapon != null && Weapon.Model is P_ChuKevLienNu) {
                Base += 3;
            }
            return Base;
        }
    }

    public PCard GetEquipment(PCardType EquipType) {
        return Area.EquipmentCardArea.CardList.Find((PCard Card) => Card.Type.Equals(EquipType));
    }

    public PCard Weapon {
        get {
            return GetEquipment(PCardType.WeaponCard);
        }
    }

    public PCard Defensor {
        get {
            return GetEquipment(PCardType.DefensorCard);
        }
    }
    public PCard Traffic {
        get {
            return GetEquipment(PCardType.TrafficCard);
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
    /// 客户端专用，用于设置装备域
    /// </summary>
    public string EquipString = "|";
    /// <summary>
    /// 客户端专用，用于设置伏兵域
    /// </summary>
    public string AmbushString = "|";

    /// <summary>
    /// 服务器端专用，用于设置标记域string
    /// </summary>
    /// <returns></returns>
    public string GetMarkString() {
        return Tags.TagString;
    }

    public string GetEquipString() {
        string Result = string.Empty;
        foreach (PCard Card in new PCard[] { Weapon, Defensor, Traffic}) {
            if (Card != null) {
                Result += "|" + Card.Name[Card.Name.Length - 1];
            }
        }
        if (Result.Length <= 1) {
            return "|";
        }
        return Result;
    }

    public string GetAmbushString() {
        string Result = string.Empty;
        foreach (PCard Card in Area.AmbushCardArea.CardList) {
            if (Card != null) {
                Result += "|" + Card.Name[0];
            }
        } 
        if (Result.Length <= 1) {
            return "|";
        }
        return Result;
    }

}