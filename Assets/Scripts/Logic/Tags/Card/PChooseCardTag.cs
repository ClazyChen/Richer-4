

public class PChooseCardTag : PTag {
    public static string TagName = "等待选择卡牌";
    public static string PlayerFieldName = "选择卡牌的玩家";
    public static string CardFieldName = "被选择的卡牌";
    public static string AllowHandCardsName = "是否允许选择手牌";
    public static string AllowEquipmentName = "是否允许选择装备";
    public static string AllowJudgeName = "是否允许选择伏兵";
    public PChooseCardTag(PPlayer Player, PCard Card, bool AllowHandCards = true, bool AllowEquipment = true, bool AllowJudge = false): base(TagName) {
        AppendField(PlayerFieldName, Player);
        AppendField(CardFieldName, Card);
        AppendField(AllowHandCardsName, AllowHandCards);
        AppendField(AllowEquipmentName, AllowEquipment);
        AppendField(AllowJudgeName, AllowJudge);
    }
    public PPlayer Player {
        get {
            return GetField<PPlayer>(PlayerFieldName, null);
        }
        set {
            SetField(PlayerFieldName, value);
        }
    }
    public PCard Card {
        get {
            return GetField<PCard>(CardFieldName, null);
        }
        set {
            SetField(CardFieldName, value);
        }
    }
    public bool AllowHandCards {
        get {
            return GetField(AllowHandCardsName, false);
        }
    }
    public bool AllowEquipment {
        get {
            return GetField(AllowEquipmentName, false);
        }
    }
    public bool AllowJudge {
        get {
            return GetField(AllowJudgeName, false);
        }
    }
}