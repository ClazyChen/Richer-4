using System;
using System.Collections.Generic;

public class P_ZhaoYun : PGeneral {

    public class PDanTag : PNumberedTag {
        public static string TagName = "胆";
        public PDanTag() : base(TagName, 1) {

        }
    }

    public P_ZhaoYun() : base("赵云") {
        Sex = PSex.Male;
        Age = PAge.Medieval;
        Index = 6;
        PSkill LongDan = new PSkill("龙胆") {
            Initiative = true
        };
        SkillList.Add(LongDan
            .AnnouceTurnOnce()
            .AddTimeTrigger(
            new PTime[] {
                PPeriod.FirstFreeTime.During,
                PPeriod.SecondFreeTime.During
            },
            (PTime Time, PPlayer Player, PSkill Skill) => {
                return new PTrigger(LongDan.Name) {
                    IsLocked = false,
                    Player = Player,
                    Time = Time,
                    AIPriority = 20,
                    CanRepeat = true,
                    Condition = (PGame Game) => {
                        return Player.Equals(Game.NowPlayer) && (Player.IsAI || Game.Logic.WaitingForEndFreeTime()) && Player.RemainLimit(LongDan.Name) && Player.HasHouse;
                    },
                    AICondition = (PGame Game) => {
                        PBlock Block = PAiMapAnalyzer.MinValueHouse(Game, Player, true).Key;
                        if (Block == null) {
                            return false;
                        } else if (!Player.Tags.ExistTag(PDanTag.TagName)) {
                            return  Block.Price < 3000;
                        }
                        int Dan = Player.Tags.FindPeekTag<PDanTag>(PDanTag.TagName).Value;
                        return (Dan < 4 && Block.Price < 3000) || Block.Price < 2000;
                    },
                    Effect = (PGame Game) => {
                        LongDan.AnnouceUseSkill(Player);
                        Game.ThrowHouse(Player, Player, LongDan.Name);
                        Player.Tags.CreateTag(new PDanTag());
                    }
                };
            })
            .AddTrigger(
            (PPlayer Player, PSkill Skill) => {
                return new PTrigger(LongDan.Name + "I") {
                    IsLocked = false,
                    Player = Player,
                    Time = PTime.Injure.EmitInjure,
                    AIPriority = 100,
                    Condition = (PGame Game) => {
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        return Player.Tags.ExistTag(PDanTag.TagName) && InjureTag.Injure > 0 && Player.Equals(InjureTag.FromPlayer) && InjureTag.ToPlayer != null;
                    },
                    AICondition = (PGame Game) => {
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        return InjureTag.Injure >= Math.Min(3000, InjureTag.ToPlayer.Money) &&
                               (InjureTag.ToPlayer.TeamIndex != Player.TeamIndex);
                    },
                    Effect = (PGame Game) => {
                        LongDan.AnnouceUseSkill(Player);
                        Player.Tags.MinusTag(PDanTag.TagName, 1);
                        Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName).Injure = PMath.Percent(Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName).Injure, 150);
                    }
                };
            })
            .AddTrigger(
            (PPlayer Player, PSkill Skill) => {
                return new PTrigger(LongDan.Name + "II") {
                    IsLocked = true,
                    Player = Player,
                    Time = PTime.Injure.AcceptInjure,
                    AIPriority = 100,
                    Condition = (PGame Game) => {
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        return Player.Tags.ExistTag(PDanTag.TagName) && InjureTag.Injure > 0 && Player.Equals(InjureTag.ToPlayer);
                    },
                    AICondition = (PGame Game) => {
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        return InjureTag.Injure >= Math.Min(Math.Min(Player.Money, PMath.Percent(Player.Money, 50)), 2000) &&
                               (InjureTag.FromPlayer == null || InjureTag.FromPlayer.TeamIndex != Player.TeamIndex);
                    },
                    Effect = (PGame Game) => {
                        LongDan.AnnouceUseSkill(Player);
                        Player.Tags.MinusTag(PDanTag.TagName, 1);
                        Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName).Injure = PMath.Percent(Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName).Injure, 50);
                    }
                };
            }));
    }

}