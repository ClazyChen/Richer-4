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
    public static PSkillInfo 轻敏 = new PSkillInfo("轻敏", "【主】你可以将一张点数为1的手牌当做【顺手牵羊】使用。", PSkillType.Initiative);
    public static PSkillInfo 太极 = new PSkillInfo("太极", "【被】锁定技，回合开始时，你须选择“阴”或“阳”。锁定技，当你处在“阴”状态时，造成的伤害+20%；当你处在“阳”状态时，受到的伤害-20%。", PSkillType.Lock);
    public static PSkillInfo 残杀 = new PSkillInfo("残杀", "【被】当你通过过路费造成伤害时，你可以令收费地的地价-1000，令目标弃置伤害量*100%的现金。", PSkillType.Passive);
    public static PSkillInfo 耀武 = new PSkillInfo("耀武", "【被】锁定技，你的起始资金*150%。", PSkillType.Lock);
    public static PSkillInfo 叫阵 = new PSkillInfo("叫阵", "【主】空闲时间点，若你有装备，你可以与一名其他角色拼点，若你赢，你对其造成1000点伤害；若你没赢，你弃置一张装备。", PSkillType.Initiative);
    public static PSkillInfo 抢掠 = new PSkillInfo("抢掠", "【被】当你通过过路费造成伤害时，你可以进行一次判定，若X为偶数，视为你使用了一张【趁火打劫】。", PSkillType.SoftLock);
    public static PSkillInfo 妙算 = new PSkillInfo("妙算", "【被】当你需要判定时，你可以从1到6中选择一个数字作为结果。", PSkillType.SoftLock);
    public static PSkillInfo 天妒 = new PSkillInfo("天妒", "【被】当你的判定生效后，你可以摸200X。", PSkillType.SoftLock);

    public PSkillInfo Copy() {
        return new PSkillInfo(Name, ToolTip, Type);
    }

    public PSkillInfo SetType(PSkillType _Type) {
        Type = _Type;
        return this;
    }

}