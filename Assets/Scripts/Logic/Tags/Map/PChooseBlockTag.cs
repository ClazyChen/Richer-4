

public class PChooseBlockTag : PTag {
    public static string TagName = "等待选择格子";
    public static string PlayerFieldName = "选择格子的玩家";
    public static string BlockFieldName = "被选择的格子";
    public PChooseBlockTag(PPlayer Player, PCard Block): base(TagName) {
        AppendField(PlayerFieldName, Player);
        AppendField(BlockFieldName, Block);
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
}