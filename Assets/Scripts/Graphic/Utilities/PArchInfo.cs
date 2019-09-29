public class PArchInfo : PObject {
    public string Info;
    public int ArchPoint = 0;

    private PArchInfo(string _Name, string _Info, int _ArchPoint = 0) {
        Name = _Name;
        Info = _Info;
        ArchPoint = _ArchPoint;
    }
    public static PArchInfo 开始游戏 = new PArchInfo("开始游戏", "开始一局大富翁游戏。", 5);
    public static PArchInfo 炸弹 = new PArchInfo("炸弹", "单次造成伤害达到10000点。", 15);
    public static PArchInfo 踩到地雷 = new PArchInfo("踩到地雷", "单次受到伤害达到10000点。", 15);
    public static PArchInfo 吃掉电脑屏幕 = new PArchInfo("吃掉电脑屏幕", "手牌数至少为17赢得一场游戏。", 50);
    public static PArchInfo 地产大亨 = new PArchInfo("地产大亨", "土地数至少为15赢得一场游戏。", 60);
    public static PArchInfo 大包工头 = new PArchInfo("大包工头", "房屋数至少为30赢得一场游戏。", 40);
    public static PArchInfo 双11剁手 = new PArchInfo("双11剁手", "被购物中心所杀。", 10);
    public static PArchInfo 化学爆炸 = new PArchInfo("化学爆炸", "被研究所所杀。", 10);
    public static PArchInfo 舍身饲虎 = new PArchInfo("舍身饲虎", "被公园所杀。", 15);
    public static PArchInfo 撞墙 = new PArchInfo("撞墙", "被城堡所杀。", 10);
    public static PArchInfo 卖身 = new PArchInfo("卖身", "被当铺所杀。", 30);
    public static PArchInfo 连锁商城 = new PArchInfo("连锁商城", "拥有至少3处购物中心赢得一场游戏。", 20);
    public static PArchInfo _5A级景区 = new PArchInfo("5A级景区", "在一片土地上拥有至少5座公园赢得一场游戏。", 20);
    public static PArchInfo 海天一色 = new PArchInfo("海天一色", "使用【瞒天过海】杀死一名角色。", 20);
    public static PArchInfo 叫我爸爸 = new PArchInfo("叫我爸爸", "回合开始时现金数达到88000。", 80);
    public static PArchInfo 疯狂试探 = new PArchInfo("疯狂试探", "在现金少于3000的情况下赢得一场游戏。", 15);
    public static PArchInfo 十三太堡 = new PArchInfo("十三太堡", "在一片土地上拥有至少13座城堡赢得一场游戏。", 15);

    // 第2批成就
    public static PArchInfo 独孤求败 = new PArchInfo("独孤求败", "在8人混战模式下获胜。", 20);
    public static PArchInfo 千钧一发 = new PArchInfo("千钧一发", "使用【围魏救赵】防止一次至少10000点的伤害。", 15);
    public static PArchInfo 老司机 = new PArchInfo("老司机", "使用【借刀杀人】获得【投石机】。", 10);
    public static PArchInfo 躺尸 = new PArchInfo("躺尸", "使用【以逸待劳】获得【借尸还魂】。", 15);
    public static PArchInfo 分一杯羹 = new PArchInfo("分一杯羹", "使用【趁火打劫】获得【木牛流马】。", 10);
    public static PArchInfo 地府的公正 = new PArchInfo("地府的公正", "使用【声东击西】令【借尸还魂】无效。", 15);
    public static PArchInfo 鸿运当头 = new PArchInfo("鸿运当头", "使用【无中生有】获得【无中生有】。", 20);
    public static PArchInfo 出其不意 = new PArchInfo("出其不意", "在自己的同一回合内，使用【暗度陈仓】并购买一片商业用地。", 10);
    public static PArchInfo 不辞劳苦 = new PArchInfo("不辞劳苦", "使用【隔岸观火】取消【以逸待劳】。", 15);
    public static PArchInfo 口蜜腹剑 = new PArchInfo("口蜜腹剑", "使用【笑里藏刀】杀死一名角色。", 20);
    public static PArchInfo 桃李不言 = new PArchInfo("桃李不言", "使用【李代桃僵】防止一次致命伤害。", 15);
    public static PArchInfo 名副其实 = new PArchInfo("名副其实", "使用【顺手牵羊】获得【西域羊驼】。", 10);

    //第3批成就
    public static PArchInfo 竹篮打水 = new PArchInfo("竹篮打水", "使用【打草惊蛇】弃置了0座房屋。", 20);
    public static PArchInfo 百足之虫 = new PArchInfo("百足之虫", "在一局游戏中，使用2次【借尸还魂】。", 30);
    public static PArchInfo 护花使者 = new PArchInfo("护花使者", "使用【调虎离山】防止一次队友被伤害。", 10);
}