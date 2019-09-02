public class PCardToolTip : PObject {
    public string ToolTip;

    private PCardToolTip(string _Name, string _ToolTip) {
        Name = _Name;
        ToolTip = _ToolTip;
    }
    public static PCardToolTip Empty = new PCardToolTip("默认", "");

    public static PCardToolTip ManTiienKuoHai = new PCardToolTip("瞒天过海", "【主】空闲时间点，对一名其他角色使用。你进行一次判定，然后对该角色造成200X点伤害。");
    public static PCardToolTip WeiWeiChiuChao = new PCardToolTip("围魏救赵", "【被】当你受到伤害时，对伤害来源使用。你与其进行一次拼点，若你赢则防止伤害。");
    public static PCardToolTip IITaiLao = new PCardToolTip("以逸待劳", "【主】空闲时间点，对任意数量的角色使用（至少一名）。依次摸一张牌并弃一张牌。");
    public static PCardToolTip CheevnHuoTaChieh = new PCardToolTip("趁火打劫", "【被】其他角色受到伤害时，对其使用。你获得其一张牌。");
    public static PCardToolTip ShevngTungChiTsi = new PCardToolTip("声东击西", "【被】一张普通计策牌指定唯一目标后，你选择一项：1.令其无效。2.令其目标改变为你指定的另一名角色。");
    public static PCardToolTip WuChungShevngYou = new PCardToolTip("无中生有", "【主】空闲时间点，对你自己使用。摸两张牌。");
    public static PCardToolTip AnTuCheevnTsaang = new PCardToolTip("暗度陈仓", "【被】第一个空闲时间点结束时，对你自己使用。选择一个格子并移动到该处。");
    public static PCardToolTip KevAnKuanHuo = new PCardToolTip("隔岸观火", "【被】当你成为一张指定至少两名目标的普通计策牌的目标后，你令其对你无效。该牌结算完毕后，你摸500。");
    public static PCardToolTip LiTaiTaaoChiang = new PCardToolTip("李代桃僵", "【被】当你受到伤害时，对伤害来源使用。防止此伤害，然后令其弃置你的一座房屋。");
    public static PCardToolTip ShunShouChiienYang = new PCardToolTip("顺手牵羊", "【主】空闲时间点，对一名其他角色使用。你获得其区域内的一张牌。");
    public static PCardToolTip TaTssaoChingShev = new PCardToolTip("打草惊蛇", "【主】空闲时间点，对其他所有角色使用。目标依次弃置一座房屋。");
    public static PCardToolTip ChiehShihHuanHun = new PCardToolTip("借尸还魂", "【主】空闲时间点或你濒死时，对你自己使用。弃置其他所有牌，将你的现金摸或弃调整与一名其他角色相同。这张牌使你增加的钱数至多为10000。");
    public static PCardToolTip TiaoHuLiShan = new PCardToolTip("调虎离山", "【主】空闲时间点，对一名其他角色使用。将其移出游戏至其下回合开始。");
}