using System;
using System.Collections.Generic;
/// <summary>
/// 金蝉脱壳
/// </summary>
public class P_ChinChaanToowChiiao : PSchemeCardModel {
    private List<PPlayer> AIEmitTargets(PGame Game, PPlayer Player) {
        return new List<PPlayer> { Player };
    }

    public override int AIInHandExpectation(PGame Game, PPlayer Player) {
        int Basic = 3000;
        int MinValue = int.MaxValue;
        int MaxValue = int.MinValue;
        PBlock Target = null;
        PBlock Block = Player.Position.NextBlock;
        for (int i = 0; i < 6; ++ i, Block = Block.NextBlock) {
            int Value = PAiMapAnalyzer.Expect(Game, Player, Block);
            MinValue = Math.Min(Value, MinValue);
            if (Value > MaxValue) {
                Target = Block;
            }
            MaxValue = Math.Max(Value, MaxValue);
        }
        return Math.Max(Basic, (MaxValue-MinValue)/2 + PAiMapAnalyzer.ChangeFaceExpect(Game, Player, Target));
    }

    public readonly static string CardName = "金蝉脱壳";

    public P_ChinChaanToowChiiao():base(CardName) {
        Point = 4;
        Index = 21;
        foreach (PTime Time in new PTime[] {
            PPeriod.WalkingStage.Before
        }) {
            MoveInHandTriggerList.Add((PPlayer Player, PCard Card) => {
                return new PTrigger(CardName) {
                    IsLocked = false,
                    Player = Player,
                    Time = Time,
                    AIPriority = 5,
                    Condition = (PGame Game) => {
                        return Player.Equals(Game.NowPlayer);
                    },
                    AICondition = (PGame Game) => {
                        // 没有被上屋抽梯
                        return AIInHandExpectation(Game, Player) > 3000;
                    },
                    Effect = MakeNormalEffect(Player, Card, AIEmitTargets, AIEmitTargets,
                        (PGame Game, PPlayer User, PPlayer Target) => {
                            Game.ChangeFace(Target);
                            int StepCount = 1;
                            if (Target.IsAI) {
                                int MaxValue = int.MinValue;
                                PBlock TestBlock = Player.Position.NextBlock;
                                for (int i = 0; i < 6; ++i, TestBlock = TestBlock.NextBlock) {
                                    int Value = PAiMapAnalyzer.Expect(Game, Player, TestBlock);
                                    if (Value > MaxValue) {
                                        StepCount = i + 1;
                                    }
                                    MaxValue = Math.Max(Value, MaxValue);
                                }
                            } else {
                                StepCount = PNetworkManager.NetworkServer.ChooseManager.Ask(Target, "选择1个数字作为行走步数", new string[] { "1", "2", "3", "4", "5", "6" }) + 1;
                            }
                            PNetworkManager.NetworkServer.TellClients(new PShowInformationOrder(Target.Name + "选择了" + StepCount));
                            Game.TagManager.FindPeekTag<PStepCountTag>(PStepCountTag.TagName).StepCount = StepCount;
                        })
                };
            });
        }
    }
}