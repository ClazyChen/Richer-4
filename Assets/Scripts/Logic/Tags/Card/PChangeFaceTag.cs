

public class PChangeFaceTag : PTag {
    public static string TagName = "翻面中";
    public static string PlayerFieldName = "翻面的玩家";
    public PChangeFaceTag(PPlayer Player): base(TagName) {
        AppendField(PlayerFieldName, Player);
    }
    public PPlayer Player {
        get {
            return GetField<PPlayer>(PlayerFieldName, null);
        }
    }
}