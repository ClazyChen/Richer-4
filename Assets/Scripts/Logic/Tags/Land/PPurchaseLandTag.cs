

public class PPurchaseLandTag : PTag {
    public static string TagName = "购买土地";
    public static string PlayerFieldName = "购买土地的玩家";
    public static string BlockFieldName = "被购买的土地";
    public static string LandPriceFieldName = "土地价格";
    public PPurchaseLandTag(PPlayer _Player, PBlock _Block) : base(TagName) {
        AppendField(PlayerFieldName, _Player);
        AppendField(BlockFieldName, _Block);
        AppendField(LandPriceFieldName, _Block.Price);
    }
    public PPlayer Player {
        get {
            return GetField<PPlayer>(PlayerFieldName, null);
        }
        set {
            SetField(PlayerFieldName, value);
        }
    }
    public PBlock Block {
        get {
            return GetField<PBlock>(BlockFieldName, null);
        }
        set {
            SetField(BlockFieldName, value);
        }
    }
    public int LandPrice {
        get {
            return GetField<int>(LandPriceFieldName, 0);
        }
        set {
            SetField(LandPriceFieldName, value);
        }
    }
}