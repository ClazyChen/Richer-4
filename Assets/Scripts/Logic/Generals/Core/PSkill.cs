using System;
using System.Collections.Generic;
/// <summary>
/// PSkill类：技能的模型
/// </summary>
public class PSkill: PObject {

    /// <summary>
    /// 是否是可锁定技。可锁定技可以切换软锁定和非软锁定状态。
    /// 硬锁定：软锁开关关闭，锁打开
    /// 软锁定开启：软锁开关打开，锁打开
    /// 软锁定关闭：软锁开关打开，锁关闭
    /// 普通被动：软锁开关关闭，锁关闭
    /// </summary>
    public bool SoftLockOpen;
    public bool Initiative;
    public bool Lock;
    public List<Func<PPlayer, PSkill, PTrigger>> TriggerList;

    public PSkill(string _Name) {
        Name = _Name;
        TriggerList = new List<Func<PPlayer, PSkill, PTrigger>>();
        Lock = false;
        Initiative = false;
        SoftLockOpen = false;
    }

    public void AnnouceUseSkill(PPlayer Player) {
        PNetworkManager.NetworkServer.TellClients(new PShowInformationOrder(Player.Name + "发动了" + Name));
    }

    public PSkill AddTrigger(Func<PPlayer, PSkill, PTrigger> TriggerGenerator) {
        TriggerList.Add(TriggerGenerator);
        return this;
    }

    public PSkill AddTimeTrigger(PTime[] TimeList, Func<PTime, PPlayer, PSkill, PTrigger> TriggerGenerator) {
        foreach (PTime Time in TimeList) {
            TriggerList.Add((PPlayer Player, PSkill Skill) => TriggerGenerator(Time, Player, this));
        }
        return this;
    }

    public PSkill AnnouceGameOnce() {
        TriggerList.Add((PPlayer Player, PSkill Skill) => {
            return new PTrigger(Skill.Name + "[初始化使用次数]") {
                IsLocked = true,
                Player = null,
                Time = PTime.StartGameTime,
                Effect = (PGame Game) => {
                    Player.Tags.CreateTag(new PUsedTag(Skill.Name, 1));
                }
            };
        });
        return this;
    }

    public PSkill AnnouceTurnOnce() {
        TriggerList.Add((PPlayer Player, PSkill Skill) => {
            return new PTrigger(Skill.Name + "[初始化使用次数]") {
                IsLocked = true,
                Player = null,
                Time = PPeriod.StartTurn.Start,
                Condition = (PGame Game) => {
                    return Player.Equals(Game.NowPlayer);
                },
                Effect = (PGame Game) => {
                    Player.Tags.CreateTag(new PUsedTag(Skill.Name, 1));
                }
            };
        });
        return this;
    }

}