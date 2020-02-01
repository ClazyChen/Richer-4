using System;
using System.Collections.Generic;
/// <summary>
/// 擒贼擒王
/// </summary>
public class P_ChiinTsevChiinWang : PSchemeCardModel {

    static public List<PPlayer> AIEmitTargets(PGame Game, PPlayer Player, int BaseValue) {
        int MaxMoney = PMath.Max(Game.PlayerList, (PPlayer _Player) => _Player.Money).Value;
        PPlayer Target =  PMath.Max(Game.PlayerList.FindAll((PPlayer _Player) => _Player.Money == MaxMoney), (PPlayer _Player) => {
            if (Player.TeamIndex == _Player.TeamIndex) {
                return PAiMapAnalyzer.ChangeFaceExpect(Game, _Player) - BaseValue;
            } else {
                return -PAiMapAnalyzer.ChangeFaceExpect(Game, _Player) - BaseValue;
            }
        }, true).Key;
        return new List<PPlayer>() { Target };
    }

    public List<PPlayer> AIEmitTargets(PGame Game, PPlayer Player) {
        return AIEmitTargets(Game, Player, 500);
    }

    public override int AIInHandExpectation(PGame Game, PPlayer Player) {
        int Basic = 500;
        return Math.Max(Basic, base.AIInHandExpectation(Game, Player));
    }

    public readonly static string CardName = "擒贼擒王";

    public P_ChiinTsevChiinWang():base(CardName) {
        Point = 3;
        Index = 18;
        foreach (PTime Time in new PTime[] {
            PPeriod.FirstFreeTime.During,
            PPeriod.SecondFreeTime.During
        }) {
            MoveInHandTriggerList.Add((PPlayer Player, PCard Card) => {
                return new PTrigger(CardName) {
                    IsLocked = false,
                    Player = Player,
                    Time = Time,
                    AIPriority = 25,
                    Condition = (PGame Game) => {
                        return Player.Equals(Game.NowPlayer) && (Player.IsAI || Game.Logic.WaitingForEndFreeTime());
                    },
                    AICondition = (PGame Game) => {
                        return AIEmitTargets(Game, Player)[0] != null;
                    },
                    Effect = MakeNormalEffect(Player, Card, AIEmitTargets,
                        (PGame Game, PPlayer _Player) => {
                            int MaxMoney = PMath.Max(Game.PlayerList, (PPlayer __Player) => __Player.Money).Value;
                            return _Player.Money == MaxMoney;
                        },
                        (PGame Game, PPlayer User, PPlayer Target) => {
                            Game.ChangeFace(Target);
                            #region 成就：草头天子
                            if (User.Equals(Target)) {
                                PArch.Announce(Game, User, "草头天子");
                            }
                            #endregion
                        })
                };
            });
        }
    }
}