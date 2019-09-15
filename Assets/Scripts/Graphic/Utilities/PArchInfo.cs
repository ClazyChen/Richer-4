public class PArchInfo : PObject {
    public string Info;

    private PArchInfo(string _Name, string _Info) {
        Name = _Name;
        Info = _Info;
    }
    public static PArchInfo 开始游戏 = new PArchInfo("开始游戏", "开始一局大富翁游戏。");
    public static PArchInfo 炸弹 = new PArchInfo("炸弹", "单次造成伤害达到10000点。");
    public static PArchInfo 踩到地雷 = new PArchInfo("踩到地雷", "单次受到伤害达到10000点。");
    public static PArchInfo 吃掉电脑屏幕 = new PArchInfo("吃掉电脑屏幕", "手牌数至少为17赢得一场游戏。");
    public static PArchInfo 地产大亨 = new PArchInfo("地产大亨", "土地数至少为15赢得一场游戏。");
    public static PArchInfo 大包工头 = new PArchInfo("大包工头", "房屋数至少为30赢得一场游戏。");
    public static PArchInfo 双11剁手 = new PArchInfo("双11剁手", "被购物中心所杀。");
    public static PArchInfo 化学爆炸 = new PArchInfo("化学爆炸", "被研究所所杀。");
    public static PArchInfo 舍身饲虎 = new PArchInfo("舍身饲虎", "被公园所杀。");
    public static PArchInfo 撞墙 = new PArchInfo("撞墙", "被城堡所杀。");
    public static PArchInfo 卖身 = new PArchInfo("卖身", "被当铺所杀。");
    public static PArchInfo 连锁商城 = new PArchInfo("连锁商城", "拥有至少3处购物中心赢得一场游戏。");
    public static PArchInfo _5A级景区 = new PArchInfo("5A级景区", "在一片土地上拥有至少5座公园赢得一场游戏。");
    public static PArchInfo 海天一色 = new PArchInfo("海天一色", "使用【瞒天过海】杀死一名角色。");
    public static PArchInfo 叫我爸爸 = new PArchInfo("叫我爸爸", "回合开始时现金数达到88000。");
    public static PArchInfo 疯狂试探 = new PArchInfo("疯狂试探", "在现金少于3000的情况下赢得一场游戏。");
    public static PArchInfo 十三太堡 = new PArchInfo("十三太堡", "在一片土地上拥有至少13座城堡赢得一场游戏。");

}