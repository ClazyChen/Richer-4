using UnityEngine;

public class PSkillType : PObject {
    public readonly Color SkillColor;

    private PSkillType(string _Name, Color _Color) {
        Name = _Name;
        SkillColor = _Color;
    }

    public static PSkillType Passive = new PSkillType("被动技能", new Color(0, 0, 0.8f));
    public static PSkillType Initiative = new PSkillType("主动技能", new Color(0, 0.5f, 0));
    public static PSkillType InitiativeInactive = new PSkillType("主动技能[不可用]", new Color(0, 0.3f, 0));
    public static PSkillType SoftLockUnlock = new PSkillType("被动技能[可软锁定]", new Color(0, 0.5f, 0.5f));
    public static PSkillType SoftLock = new PSkillType("被动技能[软锁定]", new Color(0f, 0.3f, 0.3f));
    public static PSkillType Lock = new PSkillType("被动技能[锁定]", new Color(0, 0, 0));
}
