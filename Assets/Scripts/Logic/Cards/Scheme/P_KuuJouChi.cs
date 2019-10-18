using System;
using System.Collections.Generic;
/// <summary>
/// 苦肉计
/// </summary>

public class P_KuuJouChi : PSchemeCardModel {

    public List<PPlayer> AIEmitTargets(PGame Game, PPlayer Player) {
        return new List<PPlayer> { Player };
    }

    public override int AIInHandExpectation(PGame Game, PPlayer Player) {
        int Basic = 1000;
        return Math.Max(Basic, base.AIInHandExpectation(Game, Player));
    }

    public readonly static string CardName = "苦肉计";

    public P_KuuJouChi():base(CardName) {
        Point = 6;
        Index = 34;
        foreach (PTime Time in new PTime[] {
            PPeriod.FirstFreeTime.During,
            PPeriod.SecondFreeTime.During
        }) {
            MoveInHandTriggerList.Add((PPlayer Player, PCard Card) => {
                return new PTrigger(CardName) {
                    IsLocked = false,
                    Player = Player,
                    Time = Time,
                    AIPriority = 20,
                    Condition = (PGame Game) => {
                        return Player.Equals(Game.NowPlayer) && (Player.IsAI || Game.Logic.WaitingForEndFreeTime());
                    },
                    AICondition = (PGame Game) => {
                        return Player.Money >= 3000;
                    },
                    Effect = MakeNormalEffect(Player, Card, AIEmitTargets, AIEmitTargets,
                        (PGame Game, PPlayer User, PPlayer Target) => {
                            Game.LoseMoney(Target, 1000);
                            #region 成就：最后一滴血
                            if (User.Equals(Target) && !User.IsAlive) {
                                PArch.Announce(Game, User, "最后一滴血");
                            }
                            #endregion
                            Game.GetCard(Target);
                        })
                };
            });
        }
    }
}