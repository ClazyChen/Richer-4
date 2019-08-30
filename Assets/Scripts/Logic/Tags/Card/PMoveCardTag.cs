

public class PMoveCardTag : PTag {
    public static string TagName = "移动卡牌";
    public static string CardFieldName = "被移动的卡牌";
    public static string SourceFieldName = "卡牌移出的区域";
    public static string DestinationFieldName = "卡牌移入的区域";
    public PMoveCardTag(PCard _Card, PCardArea _Source, PCardArea _Destination): base(TagName) {
        AppendField(CardFieldName, _Card);
        AppendField(SourceFieldName, _Source);
        AppendField(DestinationFieldName, _Destination);
    }
    public PCard Card {
        get {
            return GetField<PCard>(CardFieldName, null);
        }
    }
    public PCardArea Source {
        get {
            return GetField<PCardArea>(SourceFieldName, null);
        }
    }
    public PCardArea Destination {
        get {
            return GetField<PCardArea>(DestinationFieldName, null);
        }
        set {
            SetField(DestinationFieldName, value);
        }
    }

}