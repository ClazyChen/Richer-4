using System;
using System.Collections.Generic;
/// <summary>
/// PSkill类：技能的模型
/// </summary>
public class PSkill: PObject {

    /// <summary>
    /// 是否是可锁定技。可锁定技可以切换软锁定和非软锁定状态。
    /// </summary>
    public readonly bool SoftLockOpen;
    public bool SoftLock;
    public List<Func<PPlayer, PTrigger>> TriggerList;

    public PSkill(string _Name) {
        Name = _Name;
        TriggerList = new List<Func<PPlayer, PTrigger>>();
        SoftLock = false;
        SoftLockOpen = false;
    }

    public static void AnnouceUseSkill(PPlayer Player, string SkillName) {
        PNetworkManager.NetworkServer.TellClients(new PShowInformationOrder(Player.Name + "发动了" + SkillName));
    }

    public PSkill AddTrigger(Func<PPlayer, PTrigger> TriggerGenerator) {
        TriggerList.Add(TriggerGenerator);
        return this;
    }

    public PSkill AddTimeTrigger(PTime[] TimeList, Func<PTime, PPlayer, PTrigger> TriggerGenerator) {
        foreach (PTime Time in TimeList) {
            TriggerList.Add((PPlayer Player) => TriggerGenerator(Time, Player));
        }
        return this;
    }

    public PSkill AnnouceTurnOnce(string SkillName) {
        TriggerList.Add((PPlayer Player) => {
            return new PTrigger(SkillName) {
                IsLocked = true,
                Player = Player,
                Time = PPeriod.StartTurn.Start,
                Condition = (PGame Game) => {
                    return Player.Equals(Game.NowPlayer);
                },
                Effect = (PGame Game) => {
                    Player.Tags.CreateTag(new PUsedTag(SkillName, 1));
                }
            };
        });
        return this;
    }

}