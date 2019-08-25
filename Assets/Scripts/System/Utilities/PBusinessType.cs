public class PBusinessType : PObject {
    private PBusinessType(string _Name) {
        Name = _Name;
    }

    public static PBusinessType NoType = new PBusinessType("无类型");
    public static PBusinessType ShoppingCenter = new PBusinessType("购物中心");
    public static PBusinessType Institute = new PBusinessType("研究所");
    public static PBusinessType Pawnshop = new PBusinessType("当铺");
    public static PBusinessType Castle = new PBusinessType("城堡");
    public static PBusinessType Park = new PBusinessType("公园");
}
