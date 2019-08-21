using System.Collections.Generic;

public class PTagManager {
    private readonly List<PTag> TagList;

    public PTagManager() {
        TagList = new List<PTag>();
    }

    public void CreateTag(PTag Tag) {
        PLogger.Log("创建标签：" + Tag.Name);
        Tag.FieldList.ForEach((PTag.PTagField Field) => PLogger.Log("  域 " + Field.Name + " = " + Field.Field.ToString()));
        TagList.Add(Tag);
    }

    public PTag FindPeekTag(string Name) {
        return TagList.FindLast((PTag Tag) => Tag.Name.Equals(Name));
    }

    public bool ExistTag(string Name) {
        return TagList.Exists((PTag Tag) => Tag.Name.Equals(Name));
    }

    public T PopTag<T>(string Name)where T:PTag {
        PTag Tag = FindPeekTag(Name);
        if (Tag != null) {
            TagList.Remove(Tag);
        }
        return (T)Tag;
    }

    public void RemoveAll() {
        TagList.Clear();
    }
}