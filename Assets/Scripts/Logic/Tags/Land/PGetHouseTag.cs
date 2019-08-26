

public class PGetHouseTag : PTag {
    public static string TagName = "获得房屋";
    public static string BlockFieldName = "获得房屋的土地";
    public static string HouseFieldName = "获得的房屋数量";
    public PGetHouseTag(PBlock Block, int House): base(TagName) {
        AppendField(BlockFieldName, Block);
        AppendField(HouseFieldName, House);
    }
    public PBlock Block {
        get {
            return GetField<PBlock>(BlockFieldName, null);
        }
        set {
            SetField(BlockFieldName, value);
        }
    }
    public int House {
        get {
            return GetField(HouseFieldName, 0);
        }
        set {
            SetField(HouseFieldName, value);
        }
    }

}