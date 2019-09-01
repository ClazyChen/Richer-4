using System;
using System.Collections.Generic;
/// <summary>
/// 李代桃僵
/// </summary>
public class P_LiTaiTaaoChiang: PSchemeCardModel {

    private List<PPlayer> AIEmitTargets(PGame Game, PPlayer Player) {
        PPlayer Target = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName).FromPlayer;
        return new List<PPlayer>() { Target };
    }

    public override int AIInHandExpectation(PGame Game, PPlayer Player) {
        int Basic = 0;
        foreach (int Thr in new int[] {
            15000, 8000, 5000, 3000, 2000, 1500, 1000, 600, 400, 200
        }) {
            if (Player.Money <= Thr) {
                Basic += 1000;
            }
        }
        return Math.Max(Basic, 2000);
    }

    public readonly static string CardName = "李代桃僵";

    public P_LiTaiTaaoChiang():base(CardName) {
        Point = 2;
        Index = 11;
        foreach (PTime Time in new PTime[] {
            PTime.Injure.AcceptInjure
        }) {
            MoveInHandTriggerList.Add((PPlayer Player, PCard Card) => {
                return new PTrigger(CardName) {
                    IsLocked = false,
                    Player = Player,
                    Time = Time,
                    AIPriority = 80,
                    Condition = (PGame Game) => {
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        return Player.Equals(InjureTag.ToPlayer) && InjureTag.FromPlayer != null && InjureTag.Injure > 0 && Player.HasHouse;
                    },
                    AICondition = (PGame Game) => {
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        return Player.Money <= InjureTag.Injure || (InjureTag.Injure >= 1000 + PAiMapAnalyzer.MaxValueHouse(Game, Player).Value && InjureTag.FromPlayer.TeamIndex != Player.TeamIndex);
                    },
                    Effect = MakeNormalEffect(Player, Card, AIEmitTargets, AIEmitTargets,
                        (PGame Game, PPlayer User, PPlayer Target) => {
                            PNetworkManager.NetworkServer.TellClients(new PPushTextOrder(User.Index.ToString(), "李代桃僵：防止伤害", PPushType.Information.Name));
                            Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName).Injure = 0;
                            if (User.HasHouse) {
                                PBlock TargetBlock = null;
                                if (Target.IsAI) {
                                    TargetBlock = PAiMapAnalyzer.MaxValueHouse(Game, User).Key;
                                } else {
                                    TargetBlock = PNetworkManager.NetworkServer.ChooseManager.AskToChooseBlock(Target, "[李代桃僵]选择目标格子", (PBlock Block) => User.Equals(Block.Lord) && Block.HouseNumber > 0);
                                }
                                if (TargetBlock != null) {
                                    PNetworkManager.NetworkServer.TellClients(new PHighlightBlockOrder(TargetBlock.Index.ToString()));
                                    Game.LoseHouse(TargetBlock, 1);
                                }
                            }
                        })
                };
            });
        }
    }
}