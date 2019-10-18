using System;
using System.Collections.Generic;

public class P_XdYu : PGeneral {

    public P_XdYu() : base("项羽") {
        Sex = PSex.Male;
        Age = PAge.Classic;
        Index = 17;
        Cost = 30;
        Tips = "定位：攻击\n" +
            "难度：简单\n" +
            "史实：秦末西楚霸王。曾在巨鹿之战中破釜沉舟，大破秦军。\n" +
            "攻略：--\n";

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
            .AnnouceTurnOnce()
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
                        Player.Tags.FindPeekTag<PUsedTag>(PUsedTag.TagNamePrefix + Ifvb.Name).Count++;
                    }
                };
            }));
    }

}