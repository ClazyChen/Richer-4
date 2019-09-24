using System;
using System.Collections.Generic;

public class P_ZhaoYun : PGeneral {

    public class PDanTag : PNumberedTag {
        public static string TagName = "胆";
        public PDanTag() : base(TagName, 1) {

        }
    }

    public static bool LongDanICondition(PGame Game, PPlayer Player, PPlayer Target, int BaseInjure) {
        return (PMath.Percent(BaseInjure, 150) - BaseInjure) * 2 > PAiMapAnalyzer.MinValueHouse(Game, Player, true).Value + 2000 &&
               (Target.TeamIndex != Player.TeamIndex);
    }

    public static bool LongDanIICondition(PGame Game, PPlayer Player, PPlayer Source, int BaseInjure) {
        return (BaseInjure >= Player.Money ||
            (BaseInjure - PMath.Percent(BaseInjure, 50)) * (Source == null ? 1 : 2) > PAiMapAnalyzer.MinValueHouse(Game, Player, true).Value + 2000) &&
               (Source == null || Source.TeamIndex != Player.TeamIndex);
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
                        PBlock Block = PAiMapAnalyzer.MinValueHouse(Game, Player, false, true).Key;
                        if (Block == null) {
                            return false;
                        } else if (!Player.Tags.ExistTag(PDanTag.TagName)) {
                            return  Block.Price <= 2000;
                        }
                        return Block.Price <= 1000;
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
                return new PTrigger(LongDan.Name) {
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
                        return LongDanICondition(Game, Player, InjureTag.ToPlayer, InjureTag.Injure);
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
                    IsLocked = false,
                    Player = Player,
                    Time = PTime.Injure.AcceptInjure,
                    AIPriority = 100,
                    Condition = (PGame Game) => {
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        return Player.Tags.ExistTag(PDanTag.TagName) && InjureTag.Injure > 0 && Player.Equals(InjureTag.ToPlayer);
                    },
                    AICondition = (PGame Game) => {
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        return LongDanIICondition(Game, Player, InjureTag.FromPlayer, InjureTag.Injure);
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