

public class PLoseMoneyTag : PTag {
    public static string TagName = "失去金钱";
    public static string PlayerFieldName = "失去金钱的玩家";
    public static string MoneyFieldName = "失去的金钱量";
    public static string IsInjureFieldName = "是否为伤害";

    public PLoseMoneyTag(PPlayer Player, int Money, bool IsInjure = false): base(TagName) {
        AppendField(PlayerFieldName, Player);
        AppendField(MoneyFieldName, Money);
        AppendField(IsInjureFieldName, IsInjure);
    }

    public bool IsInjure {
        get {
            return GetField(IsInjureFieldName, false);
        }
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