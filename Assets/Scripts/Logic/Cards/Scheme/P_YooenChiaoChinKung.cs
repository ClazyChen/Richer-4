
using System.Collections.Generic;
/// <summary>
/// 远交近攻
/// </summary>
public class P_YooenChiaoChinKung: PSchemeCardModel {

    public static List<PPlayer> AIEmitTargets(PGame Game, PPlayer Player) {
        
        return new List<PPlayer> { PAiCardExpectation.MostValuableCardUser(Game, Game.Teammates(Player, false)) };
    }

    public override int AIInHandExpectation(PGame Game, PPlayer Player) {
        int Basic = 3000;
        if (Game.Teammates(Player).Count == 1) {
            return 0;
        }
        return Basic;
    }

    public readonly static string CardName = "远交近攻";

    public P_YooenChiaoChinKung():base(CardName) {
        Point = 4;
        Index = 23;
        foreach (PTime Time in new PTime[] {
            PPeriod.FirstFreeTime.During,
            PPeriod.SecondFreeTime.During
        }) {
            MoveInHandTriggerList.Add((PPlayer Player, PCard Card) => {
                return new PTrigger(CardName) {
                    IsLocked = false,
                    Player = Player,
                    Time = Time,
                    AIPriority = 75,
                    Condition = (PGame Game) => {
                        return Player.Equals(Game.NowPlayer) && (Player.IsAI || Game.Logic.WaitingForEndFreeTime());
                    },
                    AICondition = (PGame Game) => {
                        return AIEmitTargets(Game, Player)[0] != null;
                    },
                    Effect = MakeNormalEffect(Player, Card, AIEmitTargets, PTrigger.Except(Player),
                        (PGame Game, PPlayer User, PPlayer Target) => {
                            Game.GetCard(Target);
                            PPlayer Another = null;
                            if (User.IsAI) {
                                if (Game.Enemies(User).Exists((PPlayer _Player) => _Player.Money <= 1000) || User.Money > 15000) {
                                    Another = PMath.Min(Game.Enemies(User), (PPlayer _Player) => _Player.Money).Key;
                                } else {
                                    Another = User;
                                }
                            } else {
                                Another = PNetworkManager.NetworkServer.ChooseManager.AskForTargetPlayer(User, PTrigger.Except(Target), "远交近攻[第二目标]");
                            }
                            if (Another != null) {
                                if (Another.Equals(User)) {
                                    Game.GetMoney(Another, 1000);
                                } else {
                                    Game.LoseMoney(Another, 1000);
                                }
                            }
                        })
                };
            });
        }
    }
}