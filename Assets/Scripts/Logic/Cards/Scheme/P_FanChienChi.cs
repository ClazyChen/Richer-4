using System;
using System.Collections.Generic;
/// <summary>
/// 反间计
/// </summary>

public class P_FanChienChi : PSchemeCardModel {

    public List<PPlayer> AIEmitTargets(PGame Game, PPlayer Player) {
        return Game.PlayerList.FindAll((PPlayer _Player) => _Player.IsAlive && !_Player.Equals(Player)); ;
    }

    public override int AIInHandExpectation(PGame Game, PPlayer Player) {
        int Basic = 1000;
        int Sum = 0;
        AIEmitTargets(Game, Player).ForEach((PPlayer _Player) => {
            int Choose1 = PAiMapAnalyzer.ChangeFaceExpect(Game, _Player);
            int Choose2 = _Player.TeamIndex == Player.TeamIndex ? (_Player.Money <= 1000 ? -30000 : 0) : -2000;
            int Chosen = Math.Max(Choose1, Choose2);
            Sum += Chosen * (_Player.TeamIndex == Player.TeamIndex ? 1 : -1);
        });
        Sum = Sum * 5 / 6;
        return Math.Max(Basic, Sum);
    }

    public readonly static string CardName = "反间计";

    public P_FanChienChi():base(CardName) {
        Point = 6;
        Index = 33;
        foreach (PTime Time in new PTime[] {
            PPeriod.FirstFreeTime.During,
            PPeriod.SecondFreeTime.During
        }) {
            MoveInHandTriggerList.Add((PPlayer Player, PCard Card) => {
                return new PTrigger(CardName) {
                    IsLocked = false,
                    Player = Player,
                    Time = Time,
                    AIPriority = 65,
                    Condition = (PGame Game) => {
                        return Player.Equals(Game.NowPlayer) && (Player.IsAI || Game.Logic.WaitingForEndFreeTime());
                    },
                    AICondition = (PGame Game) => {
                        return AIInHandExpectation(Game, Player) > 1000;
                    },
                    Effect = MakeNormalEffect(Player, Card, AIEmitTargets, AIEmitTargets,
                        (PGame Game, PPlayer User, PPlayer Target) => {
                            int ChosenNumber = 1;
                            if (Target.IsAI) {
                                ChosenNumber = PMath.RandInt(1, 6);
                            } else {
                                ChosenNumber = PNetworkManager.NetworkServer.ChooseManager.Ask1To6(Target, "反间计[选择1个数字]");
                            }
                            PNetworkManager.NetworkServer.TellClients(new PShowInformationOrder(Target.Name + "选择了" + ChosenNumber));
                            int JudgeResult = Game.Judge(Target);
                            if (JudgeResult != ChosenNumber) {
                                int Test = 0;
                                if (Target.IsAI) {
                                    int Choose1 = PAiMapAnalyzer.ChangeFaceExpect(Game, Target);
                                    int Choose2 = Target.TeamIndex == User.TeamIndex ? (Target.Money <= 1000 ? -30000 : 0) : -2000;
                                    Test = (Choose1 >= Choose2 ? 0 : 1);
                                } else {
                                    Test = PNetworkManager.NetworkServer.ChooseManager.Ask(Target, "反间计[选择一项]", new string[] {
                                        "翻面", "受到" + User.Name + "造成的1000点伤害"
                                    });
                                }
                                if (Test == 0) {
                                    PNetworkManager.NetworkServer.TellClients(new PShowInformationOrder(Target.Name + "选择了翻面"));
                                    Game.ChangeFace(Target);
                                } else {
                                    PNetworkManager.NetworkServer.TellClients(new PShowInformationOrder(Target.Name + "选择了受到伤害"));
                                    Game.Injure(User, Target, 1000, Card);
                                }
                            }
                        })
                };
            });
        }
    }
}