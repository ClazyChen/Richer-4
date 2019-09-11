
using System.Collections.Generic;
/// <summary>
/// 美人计
/// </summary>
public class P_MeiJevnChi : PSchemeCardModel {

    public List<PPlayer> AIEmitTargets(PGame Game, PPlayer Player) {
        int ExpectedMoney = 1000;
        PPlayer Target1 = PAiTargetChooser.InjureTarget(Game, Player, (PGame _Game, PPlayer _Player) => {
            return  !(_Player.Defensor != null && _Player.Defensor.Model is P_YooHsi);
        }, ExpectedMoney);
        PPlayer Target2 = PAiTargetChooser.InjureTarget(Game, Player, (PGame _Game, PPlayer _Player) => {
            return !_Player.Equals(Target1) && !(_Player.Defensor != null && _Player.Defensor.Model is P_YooHsi);
        }, ExpectedMoney);
        return new List<PPlayer>() { Target1, Target2 };
    }

    public override int AIInHandExpectation(PGame Game, PPlayer Player) {
        int Basic = 3000;
        if (Game.Enemies(Player).Count < 2) {
            return 0;
        }
        return Basic;
    }

    public readonly static string CardName = "美人计";

    public P_MeiJevnChi():base(CardName) {
        Point = 6;
        Index = 31;
        foreach (PTime Time in new PTime[] {
            PPeriod.FirstFreeTime.During,
            PPeriod.SecondFreeTime.During
        }) {
            MoveInHandTriggerList.Add((PPlayer Player, PCard Card) => {
                return new PTrigger(CardName) {
                    IsLocked = false,
                    Player = Player,
                    Time = Time,
                    AIPriority = 150,
                    Condition = (PGame Game) => {
                        return Player.Equals(Game.NowPlayer) && (Player.IsAI || Game.Logic.WaitingForEndFreeTime());
                    },
                    AICondition = (PGame Game) => {
                        return AIEmitTargets(Game, Player)[0] != null && AIEmitTargets(Game, Player)[1] != null;
                    },
                    Effect = (PGame Game) => {
                        List<PPlayer> Targets = new List<PPlayer>();
                        if (Player.IsAI) {
                            Targets = AIEmitTargets(Game, Player);
                        } else {
                            Targets.Add(PNetworkManager.NetworkServer.ChooseManager.AskForTargetPlayer(Player, PTrigger.NoCondition, "美人计[第一个目标]"));
                            Targets.Add(PNetworkManager.NetworkServer.ChooseManager.AskForTargetPlayer(Player, PTrigger.Except(Targets[0]), "美人计[第二个目标]"));
                        }
                        Targets.RemoveAll((PPlayer _Player) => _Player == null);
                        if (Targets.Count == 0) { return; }
                        Game.Monitor.CallTime(PTime.Card.AfterEmitTargetTime, new PUseCardTag(Card, Player, Targets));
                        Game.CardManager.MoveCard(Card, Player.Area.HandCardArea, Game.CardManager.SettlingArea);
                        Game.Monitor.CallTime(PTime.Card.AfterBecomeTargetTime, new PUseCardTag(Card, Player, Targets));
                        if (Targets.Count >= 2) {
                            int Result = Game.PkPoint(Targets[0], Targets[1]);
                            if (Result > 0) {
                                if (Targets[0].Area.OwnerCardNumber > 0) {
                                    Game.ThrowCard(Targets[0], Targets[0]);
                                }
                            } else {
                                Game.LoseMoney(Targets[0], 1000);
                            }
                            if (Result < 0) {
                                if (Targets[1].Area.OwnerCardNumber > 0) {
                                    Game.ThrowCard(Targets[1], Targets[1]);
                                }
                            } else {
                                Game.LoseMoney(Targets[1], 1000);
                            }
                        }
                        Game.CardManager.MoveCard(Card, Game.CardManager.SettlingArea, Game.CardManager.ThrownCardHeap);
                        Game.Monitor.CallTime(PTime.Card.EndSettleTime, new PUseCardTag(Card, Player, Targets));
                    }
                };
            });
        }
    }
}