using System;
using System.Collections.Generic;
/// <summary>
/// 打草惊蛇
/// </summary>
public class P_TaTsaaoChingShev: PSchemeCardModel {

    public List<PPlayer> AIEmitTargets(PGame Game, PPlayer Player) {
        return Game.PlayerList.FindAll((PPlayer _Player) => _Player.IsAlive && !_Player.Equals(Player));
    }

    public override int AIInHandExpectation(PGame Game, PPlayer Player) {
        int Basic = 0;
        Game.Teammates(Player, false).ForEach((PPlayer _Player) => {
            KeyValuePair<PBlock, int> Test = PAiMapAnalyzer.MinValueHouse(Game, Player);
            if (Test.Key != null) {
                Basic -= Test.Value;
            }
        });
        Game.Enemies(Player).ForEach((PPlayer _Player) => {
            KeyValuePair<PBlock, int> Test = PAiMapAnalyzer.MinValueHouse(Game, Player);
            if (Test.Key != null) {
                Basic += Test.Value;
            }
        });
        return Math.Max(Basic, 500);
    }

    public readonly static string CardName = "打草惊蛇";

    public P_TaTsaaoChingShev():base(CardName) {
        Point = 3;
        Index = 13;
        foreach (PTime Time in new PTime[] {
            PPeriod.FirstFreeTime.During,
            PPeriod.SecondFreeTime.During
        }) {
            MoveInHandTriggerList.Add((PPlayer Player, PCard Card) => {
                return new PTrigger(CardName) {
                    IsLocked = false,
                    Player = Player,
                    Time = Time,
                    AIPriority = 95,
                    Condition = (PGame Game) => {
                        return Player.Equals(Game.NowPlayer) && (Player.IsAI || Game.Logic.WaitingForEndFreeTime());
                    },
                    AICondition = (PGame Game) => {
                        return AIInHandExpectation(Game, Player) > 900;
                    },
                    Effect = MakeNormalEffect(Player, Card, AIEmitTargets, AIEmitTargets,
                        (PGame Game, PPlayer User, PPlayer Target) => {
                            if (Target.HasHouse) {
                                PBlock TargetBlock = null;
                                if (Target.IsAI) {
                                    TargetBlock = PAiMapAnalyzer.MinValueHouse(Game, Target).Key;
                                } else {
                                    TargetBlock = PNetworkManager.NetworkServer.ChooseManager.AskToChooseBlock(Target, "[打草惊蛇]选择目标格子", (PBlock Block) => Target.Equals(Block.Lord) && Block.HouseNumber > 0);
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