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

    // 第2批成就
    public static PArchInfo 独孤求败 = new PArchInfo("独孤求败", "在8人混战模式下获胜。");
    public static PArchInfo 千钧一发 = new PArchInfo("千钧一发", "使用【围魏救赵】防止一次至少10000点的伤害。");
    public static PArchInfo 老司机 = new PArchInfo("老司机", "使用【借刀杀人】获得【投石机】。");
    public static PArchInfo 躺尸 = new PArchInfo("躺尸", "使用【以逸待劳】获得【借尸还魂】。");
    public static PArchInfo 分一杯羹 = new PArchInfo("分一杯羹", "使用【趁火打劫】获得【木牛流马】。");
    public static PArchInfo 地府的公正 = new PArchInfo("地府的公正", "使用【声东击西】令【借尸还魂】无效。");
    public static PArchInfo 鸿运当头 = new PArchInfo("鸿运当头", "使用【无中生有】获得【无中生有】。");
    public static PArchInfo 出其不意 = new PArchInfo("出其不意", "在自己的同一回合内，使用【暗度陈仓】并购买一片商业用地。");
    public static PArchInfo 不辞劳苦 = new PArchInfo("不辞劳苦", "使用【隔岸观火】取消【以逸待劳】。");
    public static PArchInfo 口蜜腹剑 = new PArchInfo("口蜜腹剑", "使用【笑里藏刀】杀死一名角色。");
    public static PArchInfo 桃李不言 = new PArchInfo("桃李不言", "使用【李代桃僵】防止一次致命伤害。");
    public static PArchInfo 名副其实 = new PArchInfo("名副其实", "使用【顺手牵羊】获得【西域羊驼】。");
}