public class PCardToolTip : PObject {
    public string ToolTip;

    private PCardToolTip(string _Name, string _ToolTip) {
        Name = _Name;
        ToolTip = _ToolTip;
    }
    public static PCardToolTip Empty = new PCardToolTip("默认", "");
    
    public static PCardToolTip ManTiienKuoHai = new PCardToolTip("瞒天过海", "【主】空闲时间点，对一名其他角色使用。你进行一次判定，然后对其造成200X点伤害。");
    public static PCardToolTip WeiWeiChiuChao = new PCardToolTip("围魏救赵", "【被】当你受到伤害时，对伤害来源使用。你与其进行一次拼点，若你赢则防止伤害。");
    public static PCardToolTip ChiehTaoShaJevn = new PCardToolTip("借刀杀人", "【主】空闲时间点，对一名有装备的其他角色使用。其选择一项：1.你获得一座房屋；2.你获得其一件装备。");
    public static PCardToolTip IITaiLao = new PCardToolTip("以逸待劳", "【主】空闲时间点，对任意数量的角色使用（至少一名）。依次摸一张牌并弃一张牌。");
    public static PCardToolTip CheevnHuoTaChieh = new PCardToolTip("趁火打劫", "【被】其他角色受到伤害时，对其使用。你获得其一张牌。");
    public static PCardToolTip ShevngTungChiTsi = new PCardToolTip("声东击西", "【被】一张普通计策牌指定唯一目标后，你选择一项：1.令其无效；2.令其目标改变为你指定的另一名角色。");
    public static PCardToolTip WuChungShevngYou = new PCardToolTip("无中生有", "【主】空闲时间点，对你自己使用。摸两张牌。");
    public static PCardToolTip AnTuCheevnTsaang = new PCardToolTip("暗度陈仓", "【被】第一个空闲时间点结束时，对你自己使用。选择一个格子并移动到该处。");
    public static PCardToolTip KevAnKuanHuo = new PCardToolTip("隔岸观火", "【被】当你成为一张群体普通计策牌的目标后，你令其对你无效。该牌结算完毕后，你摸500。");
    public static PCardToolTip HsiaoLiTsaangTao = new PCardToolTip("笑里藏刀", "【主】空闲时间点，对一名其他角色使用。你将手牌中的一张装备牌置入其装备区（替换原装备），然后对其造成500X点伤害[X=其此时装备数]。");
    public static PCardToolTip LiTaiTaaoChiang = new PCardToolTip("李代桃僵", "【被】当你受到伤害时，对伤害来源使用。防止此伤害，然后令其弃置你的一座房屋。");
    public static PCardToolTip ShunShouChiienYang = new PCardToolTip("顺手牵羊", "【主】空闲时间点，对一名其他角色使用。你获得其区域内的一张牌。");
    public static PCardToolTip TaTssaoChingShev = new PCardToolTip("打草惊蛇", "【主】空闲时间点，对其他所有角色使用。依次弃置一座房屋。");
    public static PCardToolTip ChiehShihHuanHun = new PCardToolTip("借尸还魂", "【主】空闲时间点或你濒死时，对你自己使用。弃置所有牌，将你的现金摸或弃至与另一名角色相同。这张牌使你增加的钱数至多为10000。");
    public static PCardToolTip TiaoHuLiShan = new PCardToolTip("调虎离山", "【主】空闲时间点，对一名其他角色使用。将其移出游戏。");
    public static PCardToolTip YooChiinKuTsung = new PCardToolTip("欲擒故纵", "【被】当你造成伤害时，若目标有手牌，对目标使用。防止此伤害，然后依次获得其两张手牌。");
    public static PCardToolTip PaaoChuanYinYoo = new PCardToolTip("抛砖引玉", "【主】空闲时间点，对其他所有角色使用。你弃一张装备牌，目标依次交给你一座房屋。");
    public static PCardToolTip ChiinTsevChiinWang = new PCardToolTip("擒贼擒王", "【主】空闲时间点，对现金最多（或之一）的一名角色使用。其翻面。");
    public static PCardToolTip FuTiChoouHsin = new PCardToolTip("釜底抽薪", "【被】当你受到过路费造成的伤害后，就地拆除所有房屋。");
    public static PCardToolTip HunShuiMoYoo = new PCardToolTip("浑水摸鱼", "【主】当一张群体普通计策牌结算完毕后，对其所有目标使用。你依次对其造成500点伤害。");
    public static PCardToolTip ChinChaanToowKeev = new PCardToolTip("金蝉脱壳", "【被】行走阶段开始前，对你自己使用。将武将牌翻面，并指定1至6的一个数字作为步数。此计策不能被【声东击西】响应。");
    public static PCardToolTip KuanMevnChoTsev = new PCardToolTip("关门捉贼", "【主】空闲时间点，你对所在位置没有房屋的所有其他角色（至少一名）使用。你对其造成1000点伤害。");
    public static PCardToolTip YooenChiaoChinKung = new PCardToolTip("远交近攻", "【主】空闲时间点，对一名其他角色使用。其摸一张牌，然后你指定另一名角色：指定自己，你摸1000；指定第三人，其弃1000。");
    public static PCardToolTip ChiaTaoFaKuo = new PCardToolTip("假道伐虢", "【被】回合结束阶段，若本回合内你经过了至少两名其他角色的土地，对其中一名角色使用。你对其造成1000点伤害。");
    public static PCardToolTip ToouLiangHuanChu = new PCardToolTip("偷梁换柱", "【主】空闲时间点，你指定两片不同的有主领地，交换其上所有的房屋。");
    public static PCardToolTip ChihSangMaHuai = new PCardToolTip("指桑骂槐", "【被】当你受到有来源的伤害时，对除伤害来源外的第三人使用。伤害转移给该角色。");
    public static PCardToolTip ChiaChiihPuTien = new PCardToolTip("假痴不癫", "【主】空闲时间点，若你的现金为全场最少（或之一），对你自己使用。移出游戏。");
    public static PCardToolTip ShangWuChoouTii = new PCardToolTip("上屋抽梯", "【主】空闲时间点，对一名其他角色[范围=3]使用。其下个行走阶段的步数*0。");
    public static PCardToolTip ShuShangKaaiHua = new PCardToolTip("树上开花", "【主】空闲时间点，令一片有主土地增加一座房屋。");
    public static PCardToolTip FanKeevWeiChu = new PCardToolTip("反客为主", "【被】当你受到过路费造成的伤害后，若你不为该土地的领主且其上的房屋数为1，获得该土地及其上的房屋。");
    public static PCardToolTip MeiJevnChi = new PCardToolTip("美人计", "【主】空闲时间点，依次指定两名角色对其使用。第一名角色对第二名角色发起一次拼点，赢的角色弃一张牌，没赢的角色弃1000。");
    public static PCardToolTip KuungCheevngChi = new PCardToolTip("空城计", "【主】空闲时间点，对你自己使用。弃置所有手牌，然后至你的下回合结束，防止所有受到的伤害。");
    public static PCardToolTip FanChienChi = new PCardToolTip("反间计", "【主】空闲时间点，对其他所有角色使用。依次选择一个1至6的数字，然后进行一次判定，若所选数字与X不同，其选择一项：1.翻面；2.受到你造成的1000点伤害。");
    public static PCardToolTip KuuJouChi = new PCardToolTip("苦肉计", "【主】空闲时间点，对你自己使用。弃1000，然后摸一张牌。");
    public static PCardToolTip LienHuanChi = new PCardToolTip("连环计", "【主】空闲时间点，对一或两名角色使用。目标被连环（若已被连环则解开）。被连环的角色受到伤害时，其他被连环的角色依次受到等量伤害，然后解开连环。");
    public static PCardToolTip TsouWeiShangChi = new PCardToolTip("走为上计", "【主】当你濒死时，对你自己使用。摸5000，然后移出游戏。");
}