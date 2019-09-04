public class PCardToolTip : PObject {
    public string ToolTip;

    private PCardToolTip(string _Name, string _ToolTip) {
        Name = _Name;
        ToolTip = _ToolTip;
    }
    public static PCardToolTip Empty = new PCardToolTip("默认", "");

    public static PCardToolTip ManTiienKuoHai = new PCardToolTip("瞒天过海", "【主】空闲时间点，对一名其他角色使用。你进行一次判定，然后对其造成200X点伤害。");
    public static PCardToolTip WeiWeiChiuChao = new PCardToolTip("围魏救赵", "【被】当你受到伤害时，对伤害来源使用。你与其进行一次拼点，若你赢则防止伤害。");
    public static PCardToolTip ChiehTaoShaJevn = new PCardToolTip("借刀杀人", "【主】空闲时间点，对一名有装备的其他角色使用。其选择一项：1.你获得其一座房屋；2.你获得其一件装备。");
    public static PCardToolTip IITaiLao = new PCardToolTip("以逸待劳", "【主】空闲时间点，对至少一名角色使用。依次摸一张牌并弃一张牌。");
    public static PCardToolTip CheevnHuoTaChieh = new PCardToolTip("趁火打劫", "【被】其他角色受到伤害时，对其使用。你获得其一张牌。");
    public static PCardToolTip ShevngTungChiTsi = new PCardToolTip("声东击西", "【被】一张普通计策牌指定唯一目标后，你选择一项：1.令其无效；2.令其目标改变为另一名角色。");
    public static PCardToolTip WuChungShevngYou = new PCardToolTip("无中生有", "【主】空闲时间点，对你自己使用。摸两张牌。");
    public static PCardToolTip AnTuCheevnTsaang = new PCardToolTip("暗度陈仓", "【被】第一个空闲时间点结束时，对你自己使用。选择一个格子并移动到该处。");
    public static PCardToolTip KevAnKuanHuo = new PCardToolTip("隔岸观火", "【被】当你成为一张群体普通计策牌的目标后，你令其对你无效。该牌结算完毕后，你摸500。");
    public static PCardToolTip HsiaoLiTsaangTao = new PCardToolTip("笑里藏刀", "【主】空闲时间点，对一名其他角色使用。你将手牌中的一张装备牌置入其装备区（替换原装备），然后对其造成500X点伤害[X=其此时装备数]。");
    public static PCardToolTip LiTaiTaaoChiang = new PCardToolTip("李代桃僵", "【被】当你受到伤害时，对伤害来源使用。其弃置你的一座房屋，然后防止此伤害。");
    public static PCardToolTip ShunShouChiienYang = new PCardToolTip("顺手牵羊", "【主】空闲时间点，对一名其他角色使用。你获得其区域内的一张牌。");
    public static PCardToolTip TaTssaoChingShev = new PCardToolTip("打草惊蛇", "【主】空闲时间点，对有房屋的所有其他角色（至少一名）使用。依次弃置一座房屋。");
    public static PCardToolTip ChiehShihHuanHun = new PCardToolTip("借尸还魂", "【主】空闲时间点或你濒死时，对你自己使用。弃置所有牌，将你的现金摸或弃至与另一名角色相同。这张牌使你增加的钱数至多为10000。");
    public static PCardToolTip TiaoHuLiShan = new PCardToolTip("调虎离山", "【主】空闲时间点，对一名其他角色使用。将其移出游戏。");
    public static PCardToolTip YooChiinKuTsung = new PCardToolTip("欲擒故纵", "【被】当你造成伤害时，若目标有手牌，对目标使用。防止此伤害，然后获得其两张手牌。");
    public static PCardToolTip PaaoChuanYinYoo = new PCardToolTip("抛砖引玉", "【主】空闲时间点，对其他所有角色使用。你弃一张装备牌，目标依次交给你一座房屋。");
    public static PCardToolTip ChiinTsevChiinWang = new PCardToolTip("擒贼擒王", "【主】空闲时间点，对现金最多的一名角色使用。其翻面。");
    public static PCardToolTip FuTiChoouHsin = new PCardToolTip("釜底抽薪", "【被】当你受到过路费造成的伤害后，弃置所有相关房屋。");
    public static PCardToolTip HunShuiMoYoo = new PCardToolTip("浑水摸鱼", "【被】当一张群体普通计策牌结算完毕后，对其所有目标使用。你依次对目标造成500点伤害。");
    public static PCardToolTip ChinChaanToowKeev = new PCardToolTip("金蝉脱壳", "【被】行走阶段开始前，对你自己使用。将武将牌翻面，并指定1至6的一个数字作为步数。此计策不能被【声东击西】响应。");
    public static PCardToolTip KuanMevnChoTsev = new PCardToolTip("关门捉贼", "【主】空闲时间点，对所在位置没有房屋的所有其他角色（至少一名）使用。你对目标造成1000点伤害。");
    public static PCardToolTip YooenChiaoChinKung = new PCardToolTip("远交近攻", "【主】空闲时间点，对一名其他角色使用。其摸一张牌，然后你选择一项：1.你摸1000；2.令另一名其他角色弃1000。");
    public static PCardToolTip ChiaTaoFaKuo = new PCardToolTip("假道伐虢", "【被】回合结束阶段，若本回合内你经过了至少两名其他角色的土地，对其中一名角色使用。你对目标造成1000点伤害。");
    public static PCardToolTip ToouLiangHuanChu = new PCardToolTip("偷梁换柱", "【主】空闲时间点，你指定两片不同的有主领地。交换其上所有的房屋。");
    public static PCardToolTip ChihSangMaHuai = new PCardToolTip("指桑骂槐", "【被】当你受到有来源的伤害时，对除伤害来源外的其他角色使用。伤害转移给目标。");
    public static PCardToolTip ChiaChiihPuTien = new PCardToolTip("假痴不癫", "【主】空闲时间点，若你的现金为全场最少，对你自己使用。移出游戏。");
    public static PCardToolTip ShangWuChoouTii = new PCardToolTip("上屋抽梯", "【主】空闲时间点，对一名范围为3的其他角色使用。目标下个行走阶段的步数*0。");
    public static PCardToolTip ShuShangKaaiHua = new PCardToolTip("树上开花", "【主】空闲时间点，令一片有主土地增加一座房屋。");
    public static PCardToolTip FanKeevWeiChu = new PCardToolTip("反客为主", "【被】当你受到过路费造成的伤害后，若你不为该土地的领主且其上的房屋数为1，获得该土地。");
    public static PCardToolTip MeiJevnChi = new PCardToolTip("美人计", "【主】空闲时间点，依次指定两名角色对其使用。第一名角色对第二名角色发起一次拼点，赢的角色弃一张牌，没赢的角色弃1000。");
    public static PCardToolTip KuungCheevngChi = new PCardToolTip("空城计", "【主】空闲时间点，对你自己使用。弃置所有手牌，然后至你的下回合结束，防止所有受到的伤害。");
    public static PCardToolTip FanChienChi = new PCardToolTip("反间计", "【主】空闲时间点，对其他所有角色使用。依次选择一个1至6的数字并进行一次判定，若所选数字不等于X，其选择一项：1.翻面；2.受到你造成的1000点伤害。");
    public static PCardToolTip KuuJouChi = new PCardToolTip("苦肉计", "【主】空闲时间点，对你自己使用。弃1000，然后摸一张牌。");
    public static PCardToolTip LienHuanChi = new PCardToolTip("连环计", "【主】空闲时间点，对一或两名角色使用。目标被连环（若已被连环则解开）。被连环的角色受到伤害时，其他被连环的角色依次受到等量伤害，然后解开连环。");
    public static PCardToolTip TsouWeiShangChi = new PCardToolTip("走为上计", "【被】当你濒死时，对你自己使用。摸5000，然后移出游戏。");
    public static PCardToolTip ChuKevLienNu = new PCardToolTip("诸葛连弩", "【被】锁定技，你于每个结算阶段购买土地或房屋的次数上限+3。");
    public static PCardToolTip KuTingTao = new PCardToolTip("古锭刀", "【被】锁定技，当你通过过路费造成伤害时，若目标没有手牌，该伤害*2。");
    public static PCardToolTip YinYooehChiiang = new PCardToolTip("银月枪", "【被】当你于回合外使用一张牌后，你可对一名其他角色造成1000点伤害。");
    public static PCardToolTip ChevnHunChiin = new PCardToolTip("镇魂琴", "【被】回合开始时，若你的现金数不多于10000，你可选择一项：1.令所有角色依次弃置500；2.令所有角色依次摸500。");
    public static PCardToolTip LoFevngKung = new PCardToolTip("落凤弓", "【被】当你通过过路费造成伤害时，你可令目标金钱数不能增加至其下回合开始。");
    public static PCardToolTip ToouShihCheev = new PCardToolTip("投石车", "【主】回合内限一次，你可弃3000拆除场上的一座房屋，若其为城堡，你进行一次判定，额外拆除X座房屋。");
    public static PCardToolTip PaKuaChevn = new PCardToolTip("八卦阵", "【被】当你受到伤害时，你可进行一次判定，若为奇数，此伤害*50%。");
    public static PCardToolTip PaiHuaChooon = new PCardToolTip("百花裙", "【被】当你受到异性角色造成的伤害时，你可令此伤害*50%。");
    public static PCardToolTip YooHsi = new PCardToolTip("玉玺", "【被】锁定技，群体普通计策牌对你无效。");
    public static PCardToolTip ChiiHsingPaao = new PCardToolTip("七星袍", "【被】锁定技，伏兵牌对你无效。");
    public static PCardToolTip TaaiPiingYaoShu = new PCardToolTip("太平要术", "【被】当你的牌即将被其他角色获得时，你可以改为将其置入弃牌堆。");
    public static PCardToolTip YinYangChing = new PCardToolTip("阴阳镜", "【被】锁定技，防止你受到的无来源伤害。");
    public static PCardToolTip ChiihTuu = new PCardToolTip("赤兔", "【被】锁定技，你的行走步数+1。");
    public static PCardToolTip ChanYing = new PCardToolTip("战鹰", "【主】回合内限一次，你可观看一名其他角色的手牌。");
    public static PCardToolTip TsaangLang = new PCardToolTip("苍狼", "【被】当你的单体普通计策牌造成伤害时，你可以获得目标的一张牌。");
    public static PCardToolTip HsiYooYangToow = new PCardToolTip("西域羊驼", "【被】锁定技，与你时代不同的角色不能弃置或获得你的手牌。");
    public static PCardToolTip NanManHsiang = new PCardToolTip("南蛮象", "【被】锁定技，防止你受到的不大于1000点的伤害。");
    public static PCardToolTip MuNiuLiuMa = new PCardToolTip("木牛流马", "【主】回合内限一次，你可弃2000，令一名其他角色摸一张牌。");
}