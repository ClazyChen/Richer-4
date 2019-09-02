public class PBusinessType : PObject {

    public readonly string ToolTip;
    private PBusinessType(string _Name, string _ToolTip) {
        Name = _Name;
        ToolTip = _ToolTip;
    }

    public static PBusinessType NoType = new PBusinessType("无类型", "");
    public static PBusinessType ShoppingCenter = new PBusinessType("购物中心", "过路费*2。");
    public static PBusinessType Institute = new PBusinessType("研究所", "一名角色停留于此时，你可以令其摸随机1~3张牌。");
    public static PBusinessType Pawnshop = new PBusinessType("当铺", "一名角色停留于此时，可交给领主1张手牌并摸2000。");
    public static PBusinessType Castle = new PBusinessType("城堡", "建造时，所在所有队列上每有1座其他角色的房屋，额外赠送1座房屋。");
    public static PBusinessType Park = new PBusinessType("公园", "建造后摸地价的50%。购买房屋免费，且可获得地价10%的政府补助。");
}
