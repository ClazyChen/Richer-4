

public class PTransportTag : PTag {
    public static string TagName = "移动位置";
    public static string PlayerFieldName = "玩家";
    public static string SourceFieldName = "起点";
    public static string DestinationFieldName = "起点";
    public PTransportTag(PPlayer Player, PBlock Source, PBlock Destination): base(TagName) {
        AppendField(PlayerFieldName, Player);
        AppendField(SourceFieldName, Source);
        AppendField(DestinationFieldName, Destination);
    }
    public PPlayer Player {
        get {
            return GetField<PPlayer>(PlayerFieldName, null);
        }
        set {
            SetField(PlayerFieldName, value);
        }
    }
    public PBlock Source {
        get {
            return GetField<PBlock>(SourceFieldName, null);
        }
        set {
            SetField(SourceFieldName, value);
        }
    }
    public PBlock Destination {
        get {
            return GetField<PBlock>(DestinationFieldName, null);
        }
        set {
            SetField(DestinationFieldName, value);
        }
    }
}