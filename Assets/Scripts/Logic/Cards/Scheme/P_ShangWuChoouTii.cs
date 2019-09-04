
using System.Collections.Generic;
/// <summary>
/// 上屋抽梯
/// </summary>
public class P_ShangWuChoouTii: PSchemeCardModel {

    public List<PPlayer> AIEmitTargets(PGame Game, PPlayer Player) {
        return new List<PPlayer>() { PMath.Max(Game.PlayerList.FindAll((PPlayer _Player) => _Player.IsAlive && !_Player.Equals(Player) && _Player.Distance(Player) <= 3 && !_Player.NoLadder), (PPlayer _Player) => {
            int Value = PAiMapAnalyzer.Expect(Game, _Player, _Player.Position);
            if (_Player.TeamIndex == Player.TeamIndex) {
                return Value;
            } else {
                return -Value;
            }
        }, true).Key };
    }

    public override int AIInHandExpectation(PGame Game, PPlayer Player) {
        int Basic = 3000;
        return Basic;
    }

    public readonly static string CardName = "上屋抽梯";

    public P_ShangWuChoouTii():base(CardName) {
        Point = 5;
        Index = 28;
        foreach (PTime Time in new PTime[] {
            PPeriod.FirstFreeTime.During,
            PPeriod.SecondFreeTime.During
        }) {
            MoveInHandTriggerList.Add((PPlayer Player, PCard Card) => {
                return new PTrigger(CardName) {
                    IsLocked = false,
                    Player = Player,
                    Time = Time,
                    AIPriority = 105,
                    Condition = (PGame Game) => {
                        return Player.Equals(Game.NowPlayer) && (Player.IsAI || Game.Logic.WaitingForEndFreeTime()) && Game.PlayerList.Exists((PPlayer _Player) => !_Player.Equals(Player) && _Player.Distance(Player) <= 3);
                    },
                    AICondition = (PGame Game) => {
                        return AIEmitTargets(Game, Player)[0] != null;
                    },
                    Effect = MakeNormalEffect(Player, Card, AIEmitTargets, (PGame Game, PPlayer _Player) => !_Player.Equals(Player) && _Player.Distance(Player) <= 3,
                        (PGame Game, PPlayer User, PPlayer Target) => {
                            Target.Tags.CreateTag(PTag.NoLadderTag);
                            PTrigger StepZero = null;
                            StepZero = new PTrigger(CardName + "[步数变为0]") {
                                IsLocked = true,
                                Player = Target,
                                Time = PPeriod.WalkingStage.Start,
                                Condition = (PGame _Game) => {
                                    return Target.Equals(_Game.NowPlayer);
                                },
                                Effect = (PGame _Game) => {
                                    Target.Tags.PopTag<PTag>(PTag.NoLadderTag.Name);
                                    PNetworkManager.NetworkServer.TellClients(new PPushTextOrder(Target.Index.ToString(), CardName + "效果触发", PPushType.Information.Name));
                                    _Game.TagManager.FindPeekTag<PStepCountTag>(PStepCountTag.TagName).StepCount = 0;
                                    _Game.Monitor.RemoveTrigger(StepZero);
                                }
                            };
                            Game.Monitor.AddTrigger(StepZero);
                        })
                };
            });
        }
    }
}