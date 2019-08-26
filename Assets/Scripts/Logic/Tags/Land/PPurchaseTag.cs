

public class PPurchaseTag : PTag {
    public static string TagName = "购买土地或房屋次数";
    public static string LimitFieldName = "次数上限";
    public static string CountFieldName = "已进行的次数";
    public PPurchaseTag(int Limit, int Count) : base(TagName) {
        AppendField(LimitFieldName, Limit);
        AppendField(CountFieldName, Count);
    }
    public int Limit {
        get {
            return GetField(LimitFieldName, 0);
        }
        set {
            SetField(LimitFieldName, value);
        }
    }
    public int Count {
        get {
            return GetField(CountFieldName, 0);
        }
        set {
            SetField(CountFieldName, value);
        }
    }
}