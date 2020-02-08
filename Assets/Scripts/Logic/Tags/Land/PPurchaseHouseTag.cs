

public class PPurchaseHouseTag : PTag {
    public static string TagName = "购买房屋";
    public static string PlayerFieldName = "购买房屋的玩家";
    public static string BlockFieldName = "购买的房屋所在土地";
    public static string HousePriceFieldName = "房屋价格";
    public PPurchaseHouseTag(PPlayer _Player, PBlock _Block) : base(TagName) {
        AppendField(PlayerFieldName, _Player);
        AppendField(BlockFieldName, _Block);
        AppendField(HousePriceFieldName, _Block.HousePrice);
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
    public int HousePrice {
        get {
            return GetField(HousePriceFieldName, 0);
        }
        set {
            SetField(HousePriceFieldName, value);
        }
    }
}