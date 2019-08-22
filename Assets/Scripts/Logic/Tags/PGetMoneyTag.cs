

public class PGetMoneyTag : PTag {
    public static string TagName = "获得金钱";
    public static string PlayerFieldName = "获得金钱的玩家";
    public static string MoneyFieldName = "获得的金钱量";
    public PGetMoneyTag(PPlayer Player, int Money): base(TagName) {
        AppendField(PlayerFieldName, Player);
        AppendField(MoneyFieldName, Money);
    }
    public PPlayer Player {
        get {
            return GetField<PPlayer>(PlayerFieldName, null);
        }
        set {
            SetField(PlayerFieldName, value);
        }
    }
    public int Money {
        get {
            return GetField(MoneyFieldName, 0);
        }
        set {
            SetField(MoneyFieldName, value);
        }
    }

}