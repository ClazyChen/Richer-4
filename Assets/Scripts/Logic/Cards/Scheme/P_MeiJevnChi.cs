using System;
using System.Collections.Generic;
/// <summary>
/// 美人计
/// </summary>
public class P_MeiJevnChi : PSchemeCardModel {

    public List<PPlayer> AIEmitTargets(PGame Game, PPlayer Player) {
        PPlayer Target1 = PMath.Min(Game.Enemies(Player).FindAll((PPlayer _Player) => !_Player.HasEquipment<P_YooHsi>() && !(_Player.General is P_LiuJi)), (PPlayer _Player) => _Player.Money).Key;
        PPlayer Target2 = PMath.Min(Game.Enemies(Player).FindAll((PPlayer _Player) => !_Player.Equals(Target1) && !_Player.HasEquipment<P_YooHsi>() && !(_Player.General is P_LiuJi)), (PPlayer _Player) => _Player.Money).Key;
        return new List<PPlayer>() { Target1, Target2 };
    }

    public override int AIInHandExpectation(PGame Game, PPlayer Player) {
        int Basic = 3000;
        if (Game.Enemies(Player).Count < 2) {
            return 0;
        }
        return Math.Max(Basic, base.AIInHandExpectation(Game, Player));
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
                            #region 成就：请尊重女性
                            if (!Targets[0].IsAlive && !Targets[1].IsAlive) {
                                PArch.Announce(Game, Player, "请尊重女性");
                            }
                            #endregion
                        }
                        Game.CardManager.MoveCard(Card, Game.CardManager.SettlingArea, Game.CardManager.ThrownCardHeap);
                        Game.Monitor.CallTime(PTime.Card.EndSettleTime, new PUseCardTag(Card, Player, Targets));
                    }
                };
            });
        }
    }
}