using System;
using System.Collections.Generic;
/// <summary>
/// 调虎离山
/// </summary>
public class P_TiaoHuLiShan: PSchemeCardModel {

    public List<PPlayer> AIEmitTargets(PGame Game, PPlayer Player) {
        return new List<PPlayer>() { PMath.Max(Game.Enemies(Player).FindAll((PPlayer _Player) => !_Player.Tags.ExistTag(PTag.OutOfGameTag.Name)), (PPlayer _Player) => {
            return -PAiMapAnalyzer.OutOfGameExpect(Game, _Player);
        }, true).Key };
    }

    public override int AIInHandExpectation(PGame Game, PPlayer Player) {
        int Basic = 0;
        int OutOfGameExpect = PMath.Max(Game.Enemies(Player).FindAll((PPlayer _Player) => !_Player.Tags.ExistTag(PTag.OutOfGameTag.Name)), (PPlayer _Player) => {
            return -PAiMapAnalyzer.OutOfGameExpect(Game, _Player);
        }, true).Value;
        return Math.Max(Basic, OutOfGameExpect);
    }

    public readonly static string CardName = "调虎离山";

    public P_TiaoHuLiShan():base(CardName) {
        Point = 3;
        Index = 14;
        foreach (PTime Time in new PTime[] {
            PPeriod.FirstFreeTime.During,
            PPeriod.SecondFreeTime.During
        }) {
            MoveInHandTriggerList.Add((PPlayer Player, PCard Card) => {
                return new PTrigger(CardName) {
                    IsLocked = false,
                    Player = Player,
                    Time = Time,
                    AIPriority = 170,
                    Condition = (PGame Game) => {
                        return Player.Equals(Game.NowPlayer) && (Player.IsAI || Game.Logic.WaitingForEndFreeTime());
                    },
                    AICondition = (PGame Game) => {
                        return AIEmitTargets(Game, Player)[0] != null;
                    },
                    Effect = MakeNormalEffect(Player, Card, AIEmitTargets,
                        PTrigger.Except(Player),
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