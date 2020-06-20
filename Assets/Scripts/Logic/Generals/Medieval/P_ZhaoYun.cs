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
        Tips = "定位：攻防兼备\n" +
            "难度：中等\n" +
            "史实：三国时期蜀汉名将，初从公孙瓒，后随刘备四处征战，曾多次救护后主刘禅，被誉为“一身都是胆”。\n" +
            "攻略：\n赵云和张三丰相似，都拥有控制伤害的技能。\n赵云的技能是需要消耗房屋的，一般来说，赵云可以把低价值土地上的房屋全部转换成【胆】，而留下几个城堡、购物中心之类的高伤害地点。【胆】的进攻收益和伤害基数有直接关系，留下高伤害地点可以让【胆】的效果有最大的发挥。当然，赵云也要留存一些【胆】来防御敌人的高额伤害。";

        PSkill LongDan = new PSkill("龙胆") {
            Initiative = true
        };
        SkillList.Add(LongDan
            .AnnounceTurnOnce()
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
                        LongDan.DeclareUse(Player);
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