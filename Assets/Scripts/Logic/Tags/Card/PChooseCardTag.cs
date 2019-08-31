

public class PChooseCardTag : PTag {
    public static string TagName = "等待选择卡牌";
    public static string PlayerFieldName = "选择卡牌的玩家";
    public static string CardFieldName = "被选择的卡牌";
    public PChooseCardTag(PPlayer Player, PCard Card): base(TagName) {
        AppendField(PlayerFieldName, Player);
        AppendField(CardFieldName, Card);
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
            return GetField<PCard>(TagName, null);
        }
        set {
            SetField(TagName, value);
        }
    }
}