using System;
using System.Collections.Generic;
/// <summary>
/// 假痴不癫
/// </summary>
public class P_ChiaChiihPuTien : PSchemeCardModel {

    public List<PPlayer> AIEmitTargets(PGame Game, PPlayer Player) {
        return new List<PPlayer>() { Player };
    }

    public override int AIInHandExpectation(PGame Game, PPlayer Player) {
        int Basic = 0;
        int Test = PAiMapAnalyzer.OutOfGameExpect(Game, Player, true);
        return Math.Max(Basic, Test);
    }

    public readonly static string CardName = "假痴不癫";

    public P_ChiaChiihPuTien():base(CardName) {
        Point = 5;
        Index = 27;
        foreach (PTime Time in new PTime[] {
            PPeriod.FirstFreeTime.During,
            PPeriod.SecondFreeTime.During
        }) {
            MoveInHandTriggerList.Add((PPlayer Player, PCard Card) => {
                return new PTrigger(CardName) {
                    IsLocked = false,
                    Player = Player,
                    Time = Time,
                    AIPriority = 180,
                    Condition = (PGame Game) => {
                        int MinMoney = PMath.Min(Game.PlayerList, (PPlayer _Player) => _Player.Money).Value;
                        return Player.Equals(Game.NowPlayer) && (Player.IsAI || Game.Logic.WaitingForEndFreeTime()) && Player.Money == MinMoney;
                    },
                    AICondition = (PGame Game) => {
                        return PAiMapAnalyzer.OutOfGameExpect(Game, Player, true) > 0 && !Player.OutOfGame;
                    },
                    Effect = MakeNormalEffect(Player, Card, AIEmitTargets, AIEmitTargets,
                        (PGame Game, PPlayer User, PPlayer Target) => {
                            Target.Tags.CreateTag(PTag.OutOfGameTag);
                            PTrigger ReturnGame = null;
                            ReturnGame = new PTrigger(CardName + "[移回游戏]") {
                                IsLocked = true,
                                Player = Target,
                                Time = PPeriod.StartTurn.During,
                                Condition = (PGame _Game) => {
                                    return Target.Equals(_Game.NowPlayer);
                                },
                                Effect = (PGame _Game) => {
                                    Target.Tags.PopTag<PTag>(PTag.OutOfGameTag.Name);
                                    _Game.Monitor.RemoveTrigger(ReturnGame);
                                }
                            };
                            Game.Monitor.AddTrigger(ReturnGame);
                        })
                };
            });
        }
    }
}