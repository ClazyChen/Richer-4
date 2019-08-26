

public class PStepCountTag : PTag {
    public static string TagName = "行走步数";
    public PStepCountTag(int StepCount) : base(TagName) {
        AppendField(TagName, StepCount);
    }

    public int StepCount {
        get {
            return GetField(TagName, 0);
        }
        set {
            SetField(TagName, value);
        }
    }
}