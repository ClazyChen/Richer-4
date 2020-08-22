

public class PDyingTag : PTag {
    public static string TagName = "濒死状态";
    public static string PlayerFieldName = "进入濒死状态的玩家";
    public static string KillerFieldName = "使其濒死的玩家";

    public PDyingTag(PPlayer Player, PPlayer Killer = null): base(TagName) {
        AppendField(PlayerFieldName, Player);
        AppendField(KillerFieldName, Killer);
    }
    public PPlayer Player {
        get {
            return GetField<PPlayer>(PlayerFieldName, null);
        }
    }

    public PPlayer Killer {
        get {
            return GetField<PPlayer>(KillerFieldName, null);
        }
    }
}