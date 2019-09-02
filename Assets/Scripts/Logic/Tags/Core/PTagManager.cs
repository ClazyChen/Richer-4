using System.Collections.Generic;

public class PTagManager {
    private readonly List<PTag> TagList;
    public readonly PPlayer Owner = null;

    public PTagManager(PPlayer _Owner = null) {
        TagList = new List<PTag>();
        Owner = _Owner;
    }

    public string TagString {
        get {
            string Result = string.Empty;
            TagList.ForEach((PTag Tag) => {
                if (Tag.Name.Length > 0) {
                    Result += "|" + Tag.Name[0];
                }
            });
            if (Result.Length <= 1) {
                return "|";
            }
            return Result;
        }
        
    }

    /// <summary>
    /// Player的标签域不能有两个同名标签
    /// </summary>
    /// <param name="Tag"></param>
    public void CreateTag(PTag Tag) {
        if (Owner == null || !ExistTag(Tag.Name)) {
            PLogger.Log("创建标签：" + Tag.Name);
            Tag.FieldList.ForEach((PTag.PTagField Field) => PLogger.Log("  域 " + Field + " = " + (Field.Field != null ? Field.Field.ToString() : "null")));
            TagList.Add(Tag);
        }
        if (Owner != null) {
            PNetworkManager.NetworkServer.TellClients(new PRefreshMarkStringOrder(Owner));
        }
    }

    public T FindPeekTag<T>(string Name) where T:PTag {
        return (T)TagList.FindLast((PTag Tag) => Tag.Name.Equals(Name));
    }

    public bool ExistTag(string Name) {
        return TagList.Exists((PTag Tag) => Tag.Name.Equals(Name));
    }

    public T PopTag<T>(string Name)where T:PTag {
        T Tag = FindPeekTag<T>(Name);
        if (Tag != null) {
            TagList.Remove(Tag);
            if (Owner != null) {
                PNetworkManager.NetworkServer.TellClients(new PRefreshMarkStringOrder(Owner));
            }
        }
        return Tag;
    }

    public void RemoveAll() {
        TagList.Clear();
        if (Owner != null) {
            PNetworkManager.NetworkServer.TellClients(new PRefreshMarkStringOrder(Owner));
        }
    }
}