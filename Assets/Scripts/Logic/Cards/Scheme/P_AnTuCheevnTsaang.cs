using System;
using System.Collections.Generic;
/// <summary>
/// 暗度陈仓
/// </summary>
public class P_AnTuCheevnTsaang: PSchemeCardModel {

    private List<PPlayer> AIEmitTargets(PGame Game, PPlayer Player) {
        return new List<PPlayer>() { Player };
    }

    public override int AIInHandExpectation(PGame Game, PPlayer Player) {
        int Basic = 2000;
        Basic = Math.Max(Basic, PMath.Max(Game.Map.BlockList, (PBlock Block) => PAiMapAnalyzer.StartFromExpect(Game, Player, Block)).Value - PAiMapAnalyzer.StartFromExpect(Game, Player, Player.Position));
        return Basic;
    }

    public readonly static string CardName = "暗度陈仓";

    public P_AnTuCheevnTsaang():base(CardName) {
        Point = 2;
        Index = 8;
        foreach (PTime Time in new PTime[] {
            PPeriod.FirstFreeTime.End
        }) {
            MoveInHandTriggerList.Add((PPlayer Player, PCard Card) => {
                return new PTrigger(CardName) {
                    IsLocked = false,
                    Player = Player,
                    Time = Time,
                    AIPriority = 120,
                    Condition = (PGame Game) => {
                        return Game.NowPlayer.Equals(Player);
                    },
                    AICondition = (PGame Game) => {
                        int Ideal = PMath.Max(Game.Map.BlockList, (PBlock Block) => PAiMapAnalyzer.StartFromExpect(Game, Player, Block)).Value;
                        int Current = PAiMapAnalyzer.StartFromExpect(Game, Player, Player.Position);
                        return (Ideal - Current >= 2000) || (-Current >= Player.Money && -Ideal < Player.Money);
                    },
                    Effect = MakeNormalEffect(Player, Card, AIEmitTargets, AIEmitTargets,
                        (PGame Game, PPlayer User, PPlayer Target) => {
                            PBlock TargetBlock = Target.Position;
                            if (Target.IsAI) {
                                TargetBlock = PMath.Max(Game.Map.BlockList, (PBlock Block) => PAiMapAnalyzer.StartFromExpect(Game, Player, Block)).Key;
                            } else {
                                TargetBlock = PNetworkManager.NetworkServer.ChooseManager.AskToChooseBlock(Target, "[暗度陈仓]选择目标格子");
                            }
                            if (TargetBlock != null) {
                                PNetworkManager.NetworkServer.TellClients(new PHighlightBlockOrder(TargetBlock.Index.ToString()));
                                Game.MovePosition(Target, Target.Position, TargetBlock);
                            }
                        })
                };
            });
        }
    }
}