

public class PDyingTag : PTag {
    public static string TagName = "濒死状态";
    public static string PlayerFieldName = "进入濒死状态的玩家";
    public PDyingTag(PPlayer Player): base(TagName) {
        AppendField(PlayerFieldName, Player);
    }
    public PPlayer Player {
        get {
            return GetField<PPlayer>(PlayerFieldName, null);
        }
    }
}