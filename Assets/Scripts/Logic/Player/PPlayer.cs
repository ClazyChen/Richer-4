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

    public PGeneral General = new P_Soldier();

    public PTagManager Tags;

    /// <summary>
    /// ��ҵ�IP��ַ��ֻ�з��������Ի�ȡ��ȱʡΪ�մ�
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
    /// �Ƿ�Ϊ��ʵ��ң�ֻ�з��������Ի�ȡ��ȱʡΪfalse
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
    /// �ƶ��������棬���������ɻ�ȡ
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

    public int LandNumber {
        get {
            if (PNetworkManager.CurrentHostType.Equals(PHostType.Server)) {
                return (int)PMath.Sum(PNetworkManager.Game.Map.BlockList.FindAll((PBlock Block) => Equals(Block.Lord)).ConvertAll( (PBlock Block) => 1.0));
            } else {
                return 0;
            }
        }
    }

    public int HouseNumber {
        get {
            if (PNetworkManager.CurrentHostType.Equals(PHostType.Server)) {
                return (int)PMath.Sum(PNetworkManager.Game.Map.BlockList.FindAll((PBlock Block) => Equals(Block.Lord)).ConvertAll((PBlock Block) => (double) Block.HouseNumber));
            } else {
                return 0;
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

    public bool HasEquipment<T>() where T: PEquipmentCardModel {
        return Area.EquipmentCardArea.CardList.Exists((PCard Card) => Card.Model is T);
    }

    public bool HasInHand<T>() where T : PCardModel {
        return Area.HandCardArea.CardList.Exists((PCard Card) => Card.Model is T);
    }

    public bool HasEquipInArea() {
        return Area.HandCardArea.CardList.Exists((PCard Card) => Card.Type.IsEquipment()) || Area.EquipmentCardArea.CardNumber > 0;
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
    /// �ͻ���ר�ã����ڸ�����������
    /// </summary>
    public int HandCardNumber = 0;
    /// <summary>
    /// �ͻ���ר�ã����ڸ����������
    /// </summary>
    public int NormalLandNumber = 0;
    /// <summary>
    /// �ͻ���ר�ã����ڸ�����ҵ�õ�����
    /// </summary>
    public int BusinessLandNumber = 0;
    /// <summary>
    /// �ͻ���ר�ã��������ñ����
    /// </summary>
    public string MarkString = "|";
    /// <summary>
    /// �ͻ���ר�ã���������װ����
    /// </summary>
    public string EquipString = "|";
    /// <summary>
    /// �ͻ���ר�ã��������÷�����
    /// </summary>
    public string AmbushString = "|";

    /// <summary>
    /// ��������ר�ã��������ñ����string
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

    public bool RemainLimit(string UsedName, bool DefaultTrue = false) {
        PUsedTag UsedTag = Tags.FindPeekTag<PUsedTag>(PUsedTag.TagNamePrefix + UsedName);
        if (UsedTag == null) {
            if (DefaultTrue) {
                return true;
            }
            Tags.CreateTag(UsedTag = new PUsedTag(UsedName, 1));
        }
        return  UsedTag != null && UsedTag.Count < UsedTag.Limit ;
    }

    public bool RemainLimit(string UsedName, PPlayer Target, bool DefaultTrue = false) {
        PUsedTag UsedTag = Tags.FindPeekTag<PUsedTag>(PUsedTag.TagNamePrefix + UsedName + Target.Name);
        if (UsedTag == null) {
            if (DefaultTrue) {
                return true;
            }
            Tags.CreateTag(UsedTag = new PUsedTag(UsedName, 1));
        }
        return UsedTag != null && UsedTag.Count < UsedTag.Limit;
    }

    public bool RemainLimitForAlivePlayers(string UsedName, PGame _Game) {
        return _Game.AlivePlayers().Exists((PPlayer _Player) => RemainLimit(UsedName, _Player, true));
    }
}