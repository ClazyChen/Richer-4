using System;
using System.Collections.Generic;
/// <summary>
/// 顺手牵羊
/// </summary>
public class P_ShunShouChiienYang: PSchemeCardModel {

    public static KeyValuePair<PPlayer, int> AIExpect(PGame Game, PPlayer Player, int BaseValue = 4000) {
        return PMath.Max(Game.PlayerList.FindAll((PPlayer TargetPlayer) => TargetPlayer.IsAlive && !TargetPlayer.Equals(Player)/* && !(TargetPlayer.General is P_ShiQian)*/), (PPlayer TargetPlayer) => {
            return PAiCardExpectation.FindMostValuableToGet(Game, Player, TargetPlayer, true, true, true).Value - BaseValue;
        }, true);
    }

    public static List<PPlayer> AIBaseEmitTargets(PGame Game, PPlayer Player, int BaseValue = 4000) {

        return new List<PPlayer> { AIExpect(Game, Player, BaseValue).Key };
    }

    public List<PPlayer> AIEmitTargets(PGame Game, PPlayer Player) {
        return AIBaseEmitTargets(Game, Player);
    }

    public override int AIInHandExpectation(PGame Game, PPlayer Player) {
        int Basic = 4000;
        int ShiQian = 0;// Game.Enemies(Player).FindAll((PPlayer _Player) => _Player.General is P_ShiQian).Count;
        Basic *= Game.Enemies(Player).Count - ShiQian;
        Basic /= Game.Enemies(Player).Count;
        return Math.Max(Basic, base.AIInHandExpectation(Game, Player));
    }

    public readonly static string CardName = "顺手牵羊";

    public P_ShunShouChiienYang():base(CardName) {
        Point = 2;
        Index = 12;
        foreach (PTime Time in new PTime[] {
            PPeriod.FirstFreeTime.During,
            PPeriod.SecondFreeTime.During
        }) {
            MoveInHandTriggerList.Add((PPlayer Player, PCard Card) => {
                return new PTrigger(CardName) {
                    IsLocked = false,
                    Player = Player,
                    Time = Time,
                    AIPriority = 120,
                    Condition = (PGame Game) => {
                        return Player.Equals(Game.NowPlayer) && (Player.IsAI || Game.Logic.WaitingForEndFreeTime()) && Game.PlayerList.Find((PPlayer _Player) => _Player.Area.CardNumber > 0 && !_Player.Equals(Player)) != null;
                    },
                    AICondition = (PGame Game) => {
                        return AIEmitTargets(Game, Player)[0] != null;
                    },
                    Effect = MakeNormalEffect(Player, Card, AIEmitTargets, (PGame Game, PPlayer _Player) => {
                        return _Player.Area.CardNumber > 0 && !_Player.Equals(Player);
                    },
                        (PGame Game, PPlayer User, PPlayer Target) => {
                            PCard Got = Game.GetCardFrom(User, Target, true, true, true);
                            #region 成就：名副其实
                            if (Got.Model is P_HsiYooYangToow && User.Area.HandCardArea.CardList.Contains(Got)) {
                                PArch.Announce(Game, User, "名副其实");
                            }
                            #endregion
                        })
                };
            });
        }
    }
}