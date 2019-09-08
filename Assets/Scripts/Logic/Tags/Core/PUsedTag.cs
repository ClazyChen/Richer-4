using System.Collections.Generic;

/// <summary>
/// PUsedTag：使用次数的限制标记
/// </summary>
public class PUsedTag : PTag {

    public static string TagNamePrefix = "使用记录";
    public static string LimitFieldName = "限制次数";
    public static string CountFieldName = "已使用次数";

    public PUsedTag(string TagName, int BaseLimit) : base(TagNamePrefix + TagName) {
        Visible = false;
        AppendField(LimitFieldName, BaseLimit);
        AppendField(CountFieldName, 0);
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