using System;
using System.Collections.Generic;
/// <summary>
/// 擒贼擒王
/// </summary>
public class P_ChiinTsevChiinWang : PSchemeCardModel {

    public List<PPlayer> AIEmitTargets(PGame Game, PPlayer Player) {
        int MaxMoney = PMath.Max(Game.PlayerList, (PPlayer _Player) => _Player.Money).Value;
        PPlayer Target =  PMath.Max(Game.PlayerList.FindAll((PPlayer _Player) => Player.Money == MaxMoney), (PPlayer _Player) => {
            if (Player.TeamIndex == _Player.TeamIndex) {
                return PAiMapAnalyzer.ChangeFaceExpect(Game, _Player);
            } else {
                return -PAiMapAnalyzer.ChangeFaceExpect(Game, _Player);
            }
        }).Key;
        return new List<PPlayer>() { Target };
    }

    public override int AIInHandExpectation(PGame Game, PPlayer Player) {
        int Basic = 300;
        int MaxMoney = PMath.Max(Game.PlayerList, (PPlayer _Player) => _Player.Money).Value;
        int Test = PMath.Max(Game.PlayerList.FindAll((PPlayer _Player) => Player.Money == MaxMoney), (PPlayer _Player) => {
            if (Player.TeamIndex == _Player.TeamIndex) {
                return PAiMapAnalyzer.ChangeFaceExpect(Game, _Player);
            } else {
                return -PAiMapAnalyzer.ChangeFaceExpect(Game, _Player);
            }
        }).Value;
        return Math.Max(Basic, Test);
    }

    public readonly static string CardName = "擒贼擒王";

    public P_ChiinTsevChiinWang():base(CardName) {
        Point = 3;
        Index = 17;
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
                        return AIInHandExpectation(Game, Player) > 300;
                    },
                    Effect = MakeNormalEffect(Player, Card, AIEmitTargets,
                        (PGame Game, PPlayer _Player) => {
                            return _Player.Money == PMath.Max(Game.PlayerList, (PPlayer __Player) => __Player.Money).Value;
                        },
                        (PGame Game, PPlayer User, PPlayer Target) => {
                            Game.ChangeFace(Target);
                        })
                };
            });
        }
    }
}