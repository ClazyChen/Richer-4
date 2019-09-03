using System.Collections.Generic;

/// <summary>
/// PNumberedTag类：有一个特定的Number域记录数字
/// </summary>
public class PNumberedTag : PTag {

    public static string NumberFieldName = "数值";

    public PNumberedTag(string TagName, int BaseValue) : base(TagName) {
        AppendField(NumberFieldName, BaseValue);
    }
    public override string Mark() {
        return base.Mark() + Value.ToString();
    }

    public int Value {
        get {
            return GetField(NumberFieldName, 0);
        }
        set {
            SetField(NumberFieldName, value);
        }
    }
}