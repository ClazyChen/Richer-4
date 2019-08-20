public class PPlayerType : PObject {
    private PPlayerType(string _Name) {
        Name = _Name;
    }
    public static PPlayerType NoType = new PPlayerType("无类型");
    public static PPlayerType Player = new PPlayerType("玩家");
    public static PPlayerType AI = new PPlayerType("AI");
    public static PPlayerType Waiting = new PPlayerType("等待中");
}