public class PSkillInfo : PObject {
    public string ToolTip;
    public PSkillType Type;

    private PSkillInfo(string _Name, string _ToolTip, PSkillType _Type) {
        Name = _Name;
        ToolTip = _ToolTip;
        Type = _Type;
    }
    public static PSkillInfo Empty = new PSkillInfo("空技能", "", PSkillType.Lock);
    public static PSkillInfo 负荆 = new PSkillInfo("负荆", "【主】回合内限一次，你可以令一名其他角色对你造成3000点伤害，然后该角色摸一张牌。", PSkillType.Initiative);
    public static PSkillInfo 英姿 = new PSkillInfo("英姿", "【被】回合开始时，你可以摸200。", PSkillType.SoftLock);
    public static PSkillInfo 闲居 = new PSkillInfo("闲居", "【被】回合结束时，若在本回合内你没有造成或受到伤害，且你在其他角色的土地上，你可以就地建造一座房屋。", PSkillType.Passive);
    public static PSkillInfo 品荔 = new PSkillInfo("品荔", "【被】当你停留在你的研究所时，你可以视为使用了一张【顺手牵羊】。", PSkillType.Passive);
    public static PSkillInfo 羞花 = new PSkillInfo("羞花", "【被】回合结束时，你可以摸200X[X=满足的条件数： 1.现金数最少；2.手牌数最少；3.土地数最少；4.房屋数最少]。", PSkillType.SoftLock);
    public static PSkillInfo 风云 = new PSkillInfo("风云", "【被】你每造成或受到一次伤害，可以在此伤害结算后摸200。", PSkillType.SoftLock);
    public static PSkillInfo 楚楚 = new PSkillInfo("楚楚", "【被】回合结束阶段，你可以进行一次判定，若X=4，你摸一张牌。", PSkillType.SoftLock);
    public static PSkillInfo 纵横 = new PSkillInfo("纵横", "【主】你可以将一张点数为3或6的手牌当做【远交近攻】使用。", PSkillType.Initiative);
    public static PSkillInfo 隐居 = new PSkillInfo("隐居", "【主】回合内限一次，你可以移动到一片你的有房屋的土地上，就地拆除一座房屋，将自己移出游戏。", PSkillType.Initiative);
    public static PSkillInfo 龙胆 = new PSkillInfo("龙胆", "【主】回合内限一次，你可弃置你的一座房屋，获得1个胆 ；当你造成伤害时，你可消耗1个胆使伤害*150%；当你受到伤害时，你可消耗1个胆使伤害*50%。", PSkillType.Initiative);
    public static PSkillInfo 飞贼 = new PSkillInfo("飞贼", "【被】锁定技，【顺手牵羊】对你无效。", PSkillType.Lock);
    public static PSkillInfo 轻敏 = new PSkillInfo("轻敏", "【主】你可以将一张点数为1的手牌当做【顺手牵羊】使用，然后摸一张牌。", PSkillType.Initiative);
    public static PSkillInfo 太极 = new PSkillInfo("太极", "【被】锁定技，回合开始时，你须选择“阴”或“阳”。锁定技，当你处在“阴”状态时，造成的伤害+20%；当你处在“阳”状态时，受到的伤害-20%。", PSkillType.Lock);
    public static PSkillInfo 残杀 = new PSkillInfo("残杀", "【被】当你通过过路费造成伤害时，你可以令收费地的地价-1000，令目标弃置伤害量*100%的现金。", PSkillType.Passive);
    public static PSkillInfo 耀武 = new PSkillInfo("耀武", "【被】锁定技，你的起始资金*150%。", PSkillType.Lock);
    public static PSkillInfo 叫阵 = new PSkillInfo("叫阵", "【主】若你有装备，你可以与一名其他角色拼点，若你赢，你对其造成1000点伤害；若你没赢，你弃置一张装备并弃1000现金。", PSkillType.Initiative);
    public static PSkillInfo 抢掠 = new PSkillInfo("抢掠", "【被】当你通过过路费造成伤害时，你可以进行一次判定，若X为偶数，视为你使用了一张【趁火打劫】。", PSkillType.SoftLock);
    public static PSkillInfo 妙算 = new PSkillInfo("妙算", "【被】当你需要判定时，你可以从1到6中选择一个数字作为结果。", PSkillType.SoftLock);
    public static PSkillInfo 天妒 = new PSkillInfo("天妒", "【被】当你的判定生效后，你可以摸200X。", PSkillType.SoftLock);
    public static PSkillInfo 剑舞 = new PSkillInfo("剑舞", "【主】你可以弃一张手牌，对所有其他角色[范围=X]造成800点伤害[X=此牌点数]。", PSkillType.Initiative);
    public static PSkillInfo 霸王 = new PSkillInfo("霸王", "【被】当一名其他角色[范围=1]受到伤害时，你可以令此伤害+800。", PSkillType.Passive);
    public static PSkillInfo 沉舟 = new PSkillInfo("沉舟", "【主】回合内限一次，你可以弃50%的现金，就地建造3座房屋。", PSkillType.Initiative);
    public static PSkillInfo 武圣 = new PSkillInfo("武圣", "【主】你可以将一张点数为偶数的牌当做【树上开花】使用。", PSkillType.Initiative);
    public static PSkillInfo 怒斩 = new PSkillInfo("怒斩", "【被】锁定技，若你的装备数多于目标，你造成的过路费伤害+600。", PSkillType.Lock);
    public static PSkillInfo 咆哮 = new PSkillInfo("咆哮", "【被】锁定技，你于每个结算阶段购买土地或房屋次数上限+3。", PSkillType.Lock);
    public static PSkillInfo 女权 = new PSkillInfo("女权", "【主】限定技，你可以令至你的下回合开始，所有女性角色以过路费或卡牌方式造成的伤害+2000。", PSkillType.Initiative);
    public static PSkillInfo 迁都 = new PSkillInfo("迁都", "【主】限定技，你可以移动到一个你的领地，将其改建为城堡，并获得城堡赠送房屋和额外的X座房屋[X=此时你的商业用地数量]。", PSkillType.Initiative);
    public static PSkillInfo 惯性 = new PSkillInfo("惯性", "【被】行走阶段开始时，若你所在的格子没有房屋，你可以弃500，移动到前方最近的一个有房屋或有其他玩家的格子。此法至多向前移动6格。", PSkillType.Passive);
    public static PSkillInfo 浪子 = new PSkillInfo("浪子", "【被】掷骰阶段开始时，你可以弃一张装备牌并选择1至6的一个数字，本次掷骰不会掷出你选择的数字。", PSkillType.Passive);
    public static PSkillInfo 风流 = new PSkillInfo("风流", "【被】当你对一名其他角色造成伤害时，你可令其选择一项：1.将一张装备移动到你的装备区。2.令此伤害+600。", PSkillType.Passive);
    public static PSkillInfo 电击 = new PSkillInfo("电击", "【主】你可以将一张点数为3或6的手牌当做【上屋抽梯】使用。", PSkillType.Initiative);

    // 第二个包
    public static PSkillInfo 起义 = new PSkillInfo("起义", "【主】你可以将一张点数为奇数的手牌当做【擒贼擒王】使用。", PSkillType.Initiative);
    public static PSkillInfo 鸿鹄 = new PSkillInfo("鸿鹄", "【被】当你翻面时，你可以摸2000，然后选择前进1至6步。", PSkillType.SoftLock);
    public static PSkillInfo 纵火 = new PSkillInfo("纵火", "【主】回合内限一次，你可以就地拆除1座房屋，然后令此处地价+10%。", PSkillType.Initiative);
    public static PSkillInfo 枭姬 = new PSkillInfo("枭姬", "【被】当你失去一张装备时，可以摸一张牌，并摸1500。", PSkillType.SoftLock);
    public static PSkillInfo 易装 = new PSkillInfo("易装", "【主】限定技，你可以与一名其他角色交换装备，然后你将性别修改为男。", PSkillType.Initiative);
    public static PSkillInfo 贪污 = new PSkillInfo("贪污", "【被】锁定技，你购买土地和建造房屋的费用*50%。", PSkillType.Lock);
    public static PSkillInfo 受贿 = new PSkillInfo("受贿", "【主】每名角色限一次，你可以对一名角色造成1000点伤害，令其下一次购买土地和建造房屋的费用*50%。", PSkillType.Initiative);
    public static PSkillInfo 离骚 = new PSkillInfo("离骚", "【被】当你受到一次伤害后，你可以进行若干次判定直到判定结果数列不单调为止。然后你弃300X并摸一张牌[X=判定次数]。", PSkillType.SoftLock);
    public static PSkillInfo 勤学 = new PSkillInfo("勤学", "【主】回合内限一次，你可以弃置1座房屋，翻开牌堆顶的4张牌，获得其中的装备牌。", PSkillType.Initiative);
    public static PSkillInfo 白衣 = new PSkillInfo("白衣", "【被】当你受到过路费造成的伤害时，你可以弃一张装备牌，令其*50%。", PSkillType.Passive);
    public static PSkillInfo 民主 = new PSkillInfo("民主", "【主】每局游戏限两次，使用后四回合才能再次使用。你令其他所有角色选择一项：1.令你对其造成800X[X=其所在地房屋数]点伤害，然后摸一张牌；2.弃置所有手牌（至少一张）。", PSkillType.Initiative);
    public static PSkillInfo 安魂 = new PSkillInfo("安魂", "【被】限定技，当你濒死时，可以令所有男性角色依次选择是否弃一张牌令你回复5000。", PSkillType.Passive);
    public static PSkillInfo 夺魄 = new PSkillInfo("夺魄", "【被】当你通过过路费造成伤害时，你可以防止此伤害并进行一次判定，目标在X个回合结束前不能对你收取过路费。", PSkillType.Passive);
    public static PSkillInfo 独奏 = new PSkillInfo("独奏", "【被】当你即将造成伤害时，你可以将目标的所有手牌移入其额外区域。伤害结算后若目标存活，其获得这些牌；否则你获得这些牌，并且可以对第三人造成50%的伤害。", PSkillType.Passive);
    public static PSkillInfo 轮舞曲 = new PSkillInfo("轮舞曲", "【主】限定技，你可以令其他所有角色弃两张手牌，然后将场上一处有主商业用地改建为歌厅（收取过路费时+100%，且令目标翻面）。", PSkillType.Initiative);
    public static PSkillInfo 神兽 = new PSkillInfo("神兽", "【被】锁定技，游戏开始时你摸30000。锁定技，回合开始时，你摸500。", PSkillType.Lock);
    public static PSkillInfo 青龙 = new PSkillInfo("青龙", "【被】锁定技，当你受到伤害时，你获得伤害来源的一张牌。", PSkillType.Lock);
    public static PSkillInfo 白虎 = new PSkillInfo("白虎", "【被】锁定技，回合结束时，若你本回合经过了其他角色的土地，你令其连环。", PSkillType.Lock);
    public static PSkillInfo 朱雀 = new PSkillInfo("朱雀", "【被】锁定技，回合开始时，你令所有其他角色弃置300。", PSkillType.Lock);
    public static PSkillInfo 玄武 = new PSkillInfo("玄武", "【被】锁定技，当你受到过路费造成的伤害后，你在自己房屋最少的1片土地上建造1座房屋并翻面。", PSkillType.Lock);
    public static PSkillInfo 精灵加护_美九 = new PSkillInfo("精灵加护·美九", "【被】锁定技，当你濒死时若美九存活，你摸至10000，然后获得1点精灵力量。", PSkillType.Lock);
    public static PSkillInfo 音之天使_觉醒 = new PSkillInfo("音之天使·觉醒", "【被】限定技，回合开始时，若你有至少3点精灵力量，你可令美九获得场上所有商业用地及其房屋并改建为歌厅。", PSkillType.Passive);
    public static PSkillInfo 镇魂曲 = new PSkillInfo("镇魂曲", "【被】锁定技，美九受到的伤害-20%。", PSkillType.Lock);
    public static PSkillInfo 进行曲 = new PSkillInfo("进行曲", "【被】锁定技，美九的回合结束时，若其在自己的土地上，其就地建造X+1座房屋[X=精灵力量]，然后摸1张牌。", PSkillType.Lock);
    public static PSkillInfo 鸩杀 = new PSkillInfo("鸩杀", "【主】回合内限一次，你可以将一张手牌交给一名其他角色[范围=1]，对其造成1500点伤害。", PSkillType.Initiative);
    public static PSkillInfo 蓄谋 = new PSkillInfo("蓄谋", "【被】锁定技，当其他角色受到你造成的伤害而濒死时，其无法脱离濒死状态。", PSkillType.Lock);
    public static PSkillInfo 圣女 = new PSkillInfo("圣女", "【被】当一名角色受到过路费伤害而处于濒死状态时，你可以弃置所有牌（至少1张），视为该角色使用了【借尸还魂】，该牌结算后，你弃5000。然后若该角色不为你，其摸1张牌。", PSkillType.Passive);
    public static PSkillInfo 航帆 = new PSkillInfo("航帆", "【被】回合开始时，你可以令你本回合内的行走步数变为掷骰点数*2，若如此做，你受到的伤害*150%，购买土地或房屋的价格*0直到回合结束。", PSkillType.Passive);

    public PSkillInfo Copy() {
        return new PSkillInfo(Name, ToolTip, Type);
    }

    public PSkillInfo SetType(PSkillType _Type) {
        Type = _Type;
        return this;
    }

}