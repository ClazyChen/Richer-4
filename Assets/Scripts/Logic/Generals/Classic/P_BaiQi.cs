using System;
using System.Collections.Generic;

public class P_BaiQi: PGeneral {

    public P_BaiQi() : base("白起") {
        Sex = PSex.Male;
        Age = PAge.Classic;
        Index = 9;
        Cost = 25;
        Tips = "定位：爆发\n" +
            "难度：简单\n" +
            "史实：战国时期秦国名将，“战国四大名将”之一，曾在伊阙之战、长平之战中大败三晋军队，歼敌数十万，功封武安君。\n" +
            "攻略：\n白起是一个强制命中系武将，白起的存在使得对方无法通过【李代桃僵】、【指桑骂槐】一类的牌免除伤害，或通过【八卦阵】、【龙胆】等方式减少伤害，而必须硬吃下伤害的100%（通常这个数字比伤害值高）。随之带来的损失则是收费地的地价永久性减少，很多时候这是不值的，但在必杀一击上，白起一点都不会含糊。";

        PSkill CanSha = new PSkill("残杀");
        SkillList.Add(CanSha
            .AddTrigger(
            (PPlayer Player, PSkill Skill) => {
                return new PTrigger(CanSha.Name) {
                    IsLocked = false,
                    Player = Player,
                    Time = PTime.Injure.EmitInjure,
                    AIPriority = 100,
                    Condition = (PGame Game) => {
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        return InjureTag.Injure > 0 && InjureTag.ToPlayer != null && Player.Equals(InjureTag.FromPlayer) &&
                        InjureTag.InjureSource is PBlock && ((PBlock)InjureTag.InjureSource).Price >= 1000;
                    },
                    AICondition = (PGame Game) => {
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        if (InjureTag.ToPlayer.TeamIndex != Player.TeamIndex) {
                            int Benefit = PMath.Percent(InjureTag.Injure, 100);
                            if (Benefit + InjureTag.Injure >= InjureTag.ToPlayer.Money) {
                                // 一击必杀
                                return true;
                            }
                            if (Benefit >= 2000) {
                                PBlock Source = (PBlock)InjureTag.InjureSource;
                                int Cost = 1000 + PAiMapAnalyzer.HouseValue(Game, Player, Source) * Source.HouseNumber * 1000 / Source.Price;
                                if (Cost < Benefit) {
                                    return true;
                                }
                            }
                        }
                        return false;
                    },
                    Effect = (PGame Game) => {
                        CanSha.AnnouceUseSkill(Player);
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        PBlock Source = (PBlock)InjureTag.InjureSource;
                        Source.Price -= 1000;
                        PNetworkManager.NetworkServer.TellClients(new PRefreshBlockBasicOrder(Source));
                        Game.LoseMoney(InjureTag.ToPlayer, PMath.Percent(InjureTag.Injure, 100));
                    }
                };
            }));
    }

}