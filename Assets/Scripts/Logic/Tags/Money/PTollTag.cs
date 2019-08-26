

public class PTollTag : PTag {
    public static string TagName = "过路费";
    public static string FromPlayerFieldName = "收取过路费的玩家";
    public static string ToPlayerFieldName = "过路费的目标";
    public static string TollFieldName = "过路费数额";
    public PTollTag(PPlayer FromPlayer, PPlayer ToPlayer, int Toll): base(TagName) {
        AppendField(FromPlayerFieldName, FromPlayer);
        AppendField(ToPlayerFieldName, ToPlayer);
        AppendField(TollFieldName, Toll);
    }
    public PPlayer FromPlayer {
        get {
            return GetField<PPlayer>(FromPlayerFieldName, null);
        }
        set {
            SetField(FromPlayerFieldName, value);
        }
    }
    public PPlayer ToPlayer {
        get {
            return GetField<PPlayer>(ToPlayerFieldName, null);
        }
        set {
            SetField(ToPlayerFieldName, value);
        }
    }
    public int Toll {
        get {
            return GetField(TollFieldName, 0);
        }
        set {
            SetField(TollFieldName, value);
        }
    }

}