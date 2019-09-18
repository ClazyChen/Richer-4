using System;
using System.Collections.Generic;

public class P_LianPo: PGeneral {


    public P_LianPo() : base("廉颇") {
        Sex = PSex.Male;
        Age = PAge.Classic;
        Index = 1;
        PSkill FuJing = new PSkill("负荆") {
            Initiative = true
        };
        SkillList.Add(FuJing
            .AnnouceTurnOnce()
            .AddTimeTrigger(
            new PTime[] {
                PPeriod.FirstFreeTime.During,
                PPeriod.SecondFreeTime.During
            },
            (PTime Time, PPlayer Player, PSkill Skill) => {
                return new PTrigger(FuJing.Name) {
                    IsLocked = false,
                    Player = Player,
                    Time = Time,
                    AIPriority = 180,
                    Condition = (PGame Game) => {
                        return Player.Equals(Game.NowPlayer) && (Player.IsAI || Game.Logic.WaitingForEndFreeTime()) && Player.RemainLimit(FuJing.Name) ;
                    },
                    AICondition = (PGame Game) => {
                        if (Player.Money >= 15000) {
                            return true;
                        } else {
                            bool CanGo = true;
                            PAiMapAnalyzer.NextBlocks(Game, Player).ForEach((PBlock Block) => {
                                if (Block.Lord == null && Block.CanPurchase && Block.Price >= Player.Money - 3000) {
                                    CanGo = false;
                                }
                                if (Player.Equals(Block.Lord) && Block.HousePrice >= Player.Money - 3000) {
                                    CanGo = false;
                                }
                                if (Block.Lord != null && Block.Lord.TeamIndex != Player.TeamIndex && Block.Toll >= Player.Money - 3000) {
                                    CanGo = false;
                                }
                            });
                            return CanGo;
                        }
                    },
                    Effect = (PGame Game) => {
                        FuJing.AnnouceUseSkill(Player);
                        PPlayer Target = null;
                        if (Player.IsAI) {
                            Target = PMath.Max(Game.Teammates(Player, false), (PPlayer _Player) => _Player.AiCardExpectation).Key;
                        } else {
                            Target = PNetworkManager.NetworkServer.ChooseManager.AskForTargetPlayer(Player, PTrigger.Except(Player), FuJing.Name, true);
                        }
                        if (Target != null) {
                            Game.Injure(Target, Player, 3000, FuJing);
                            Game.GetCard(Target, 1);
                            Player.Tags.FindPeekTag<PUsedTag>(PUsedTag.TagNamePrefix + FuJing.Name).Count++;
                        }
                    }
                };
            }));
    }

}