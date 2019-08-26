

public class PDiceResultTag : PTag {
    public static string TagName = "掷骰结果";
    public PDiceResultTag(int DiceResult): base(TagName) {
        AppendField(TagName, DiceResult);
    }
    public int DiceResult {
        get {
            return GetField(TagName, 0);
        }
        set {
            SetField(TagName, value);
        }
    }
}