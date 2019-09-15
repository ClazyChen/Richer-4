public class PHostType : PObject {
    private PHostType(string _Name) {
        Name = _Name;
    }
    public static PHostType NoType = new PHostType("无类型");
    public static PHostType Server = new PHostType("服务器");
    public static PHostType Client = new PHostType("客户端");
}