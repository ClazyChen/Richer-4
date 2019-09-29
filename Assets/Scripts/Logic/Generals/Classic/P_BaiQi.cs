using System;
using System.Collections.Generic;

public class P_BaiQi: PGeneral {

    public P_BaiQi() : base("白起") {
        Sex = PSex.Male;
        Age = PAge.Classic;
        Index = 9;
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