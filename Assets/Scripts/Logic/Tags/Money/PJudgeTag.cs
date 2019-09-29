

public class PJudgeTag : PTag {
    public static string TagName = "判定";
    public static string PlayerFieldName = "判定的玩家";
    public static string ResultFieldName = "判定的结果";
    public static string AdvisedResultFieldName = "判定的建议结果";
    public PJudgeTag(PPlayer Player, int Result, int AdvisedResult): base(TagName) {
        AppendField(PlayerFieldName, Player);
        AppendField(ResultFieldName, Result);
        AppendField(AdvisedResultFieldName, AdvisedResult);
    }
    public PPlayer Player {
        get {
            return GetField<PPlayer>(PlayerFieldName, null);
        }
        set {
            SetField(PlayerFieldName, value);
        }
    }
    public int Result {
        get {
            return GetField(ResultFieldName, 0);
        }
        set {
            SetField(ResultFieldName, value);
        }
    }
    public int AdvisedResult {
        get {
            return GetField(AdvisedResultFieldName, 0);
        }
        set {
            SetField(AdvisedResultFieldName, value);
        }
    }

}