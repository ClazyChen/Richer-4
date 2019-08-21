using System.Collections.Generic;

/// <summary>
/// PTag类：用来记录一些全局标签
/// 在触发时机的时候也用来在Game中记录变量
/// </summary>
public class PTag : PObject {
    public class PTagField : PObject {
        public object Field;
        public PTagField(string _Name, object _Field = null) {
            Name = _Name;
            Field = _Field;
        }
        public T GetField<T>() {
            return (T) Field;
        }
    }

    public List<PTagField> FieldList;
    public PTag(string _Name) {
        Name = _Name;
        FieldList = new List<PTagField>();
    }

    public PTag AppendField(string _Name, object _Field = null) {
        FieldList.Add(new PTagField(_Name, _Field));
        return this;
    }

    public T GetField<T>(string FieldName, T Default) {
        PTagField TagField = FieldList.Find((PTagField Field) => Field.Name.Equals(FieldName));
        if (TagField != null) {
            return TagField.GetField<T>();
        } else {
            return Default;
        }
    }

    public void SetField(string FieldName, object Value) {
        PLogger.Log("重设标签[" + Name + "." + FieldName + "] = " + Value.ToString());
        PTagField TagField = FieldList.Find((PTagField Field) => Field.Name.Equals(FieldName));
        if (TagField != null) {
            TagField.Field = Value;
        }
    }

    public static PTag FreeTimeOperationTag = new PTag("空闲时间点操作中");
}