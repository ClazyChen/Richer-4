using System;
using System.Collections.Generic;

public class P_Xdyu : PGeneral {

    public P_Xdyu() : base("项羽") {
        Sex = PSex.Male;
        Age = PAge.Classic;
        Index = 17;
        Cost = 30;
        Tips = "定位：攻击\n" +
            "难度：中等\n" +
            "史实：秦末西楚霸王。曾在巨鹿之战中破釜沉舟，大破秦军。\n" +
            "攻略：\n项羽是一个十分考验玩家判断力的武将，【霸王】在小图的发挥显著强于大图，能够配合虞姬的【剑舞】，也可以增加天灾对敌人的伤害，还可以在一定程度上辅助队友的输出，范围为1限制了项羽的跨队列输出，但敌人越多就越强，颇有乌江快战之风范。\n"+
            "【沉舟】是一个爆发力极强的技能，相应地也要付出较大的代价。要想达到正收益，技能发动往往是在中后期，而购物中心往往是项羽的不二选择，当现金小于18000时【沉舟】已经是正收益，而对于领地来说只需花费小于正常花费即可一试。当然，在现金较低的情况下，项羽甚至可以连续【沉舟】，在高风险的同时可能获得极高的回报，或可重振西楚霸业。\n";

        PSkill Bawh = new PSkill("霸王");
        SkillList.Add(Bawh
            .AddTimeTrigger(
            new PTime[] {
                PTime.Injure.AcceptInjure
            },
            (PTime Time, PPlayer Player, PSkill Skill) => {
                return new PTrigger(Bawh.Name) {
                    IsLocked = false,
                    Player = Player,
                    Time = Time,
                    AIPriority = 200,
                    Condition = (PGame Game) => {
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        return InjureTag.Injure > 0 && InjureTag.ToPlayer != null && !Player.Equals(InjureTag.ToPlayer) && InjureTag.ToPlayer.Distance(Player) <= 1;
                    },
                    AICondition = (PGame Game) => {
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        return InjureTag.ToPlayer.TeamIndex != Player.TeamIndex;
                    },
                    Effect = (PGame Game) => {
                        Bawh.AnnouceUseSkill(Player);
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        InjureTag.Injure += 800;
                    }
                };
            }));

        PSkill Ifvb = new PSkill("沉舟") {
            Initiative = true
        };
        SkillList.Add(Ifvb
            .AnnounceTurnOnce()
            .AddTimeTrigger(
            new PTime[] {
                PPeriod.FirstFreeTime.During,
                PPeriod.SecondFreeTime.During
            },
            (PTime Time, PPlayer Player, PSkill Skill) => {
                return new PTrigger(Ifvb.Name) {
                    IsLocked = false,
                    Player = Player,
                    Time = Time,
                    AIPriority = 200,
                    CanRepeat = true,
                    Condition = (PGame Game) => {
                        return Player.Equals(Game.NowPlayer) && (Player.IsAI || Game.Logic.WaitingForEndFreeTime()) && 
                        Player.RemainLimit(Ifvb.Name) &&
                        Player.Position.Lord != null;
                    },
                    AICondition = (PGame Game) => {
                        if (Player.Money <= 1000) {
                            return false;
                        }
                        return PMath.Percent(Player.Money, 50) + 1500 < 3 * PAiMapAnalyzer.HouseValue(Game, Player, Player.Position);
                    },
                    Effect = (PGame Game) => {
                        Ifvb.AnnouceUseSkill(Player);
                        Game.LoseMoney(Player, PMath.Percent(Player.Money, 50));
                        Game.GetHouse(Player.Position, 3);
                        Ifvb.DeclareUse(Player);
                    }
                };
            }));
    }

}