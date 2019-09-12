using System;
using System.Collections.Generic;
/// <summary>
/// 镇魂琴
/// </summary>
public class P_ChevnHunChiin : PEquipmentCardModel {

    public override int AIInEquipExpectation(PGame Game, PPlayer Player) {
        return 500 + Player.Money >= 10000 ? 0 : 2000 * Math.Abs(Game.Enemies(Player).Count - Game.Teammates(Player).Count);
    }

    public int AiChooseResult(PGame Game, PPlayer Player) {
        List<PPlayer> Enemies = Game.Enemies(Player);
        List<PPlayer> Teammates = Game.Teammates(Player);
        if (Teammates.Exists((PPlayer _Player) => _Player.Money <= 500)) {
            return 1;
        } else if (Enemies.Exists((PPlayer _Player) => _Player.Money <= 500)) {
            return 0;
        } else {
            if (Enemies.Count > Teammates.Count) {
                return 0;
            } else if (Enemies.Count < Teammates.Count) {
                return 1;
            } else {
                return -1;
            }
        }
    }

    public readonly static string CardName = "镇魂琴";

    public P_ChevnHunChiin():base(CardName, PCardType.WeaponCard) {
        Point = 4;
        Index = 46;
        foreach (PTime Time in new PTime[] {
            PPeriod.StartTurn.During
        }) {
            MoveInEquipTriggerList.Add((PPlayer Player, PCard Card) => {
                return new PTrigger(CardName) {
                    IsLocked = false,
                    Player = Player,
                    Time = Time,
                    Condition = (PGame Game) => {
                        return Game.NowPlayer.Equals(Player) && Player.Money <= 10000;
                    },
                    AICondition = (PGame Game) => {
                        return AiChooseResult(Game, Player) >= 0;
                    },
                    Effect = (PGame Game) => {
                        AnnouceUseEquipmentSkill(Player);
                        int Result = 0;
                        string[] Options = new string[] {
                            "全体弃500", "全体摸500"
                        };
                        if (Player.IsAI) {
                            Result = AiChooseResult(Game, Player);
                        } else {
                            Result = PNetworkManager.NetworkServer.ChooseManager.Ask(Player, CardName, Options);
                        }
                        PNetworkManager.NetworkServer.TellClients(new PShowInformationOrder(Player.Name + "选择了" + Options[Result]));
                        if (Result == 0) {
                            Game.Traverse((PPlayer _Player) => {
                                Game.LoseMoney(_Player, 500);
                            }, Player);
                        } else if (Result == 1) {
                            Game.Traverse((PPlayer _Player) => {
                                Game.GetMoney(_Player, 500);
                            }, Player);
                        }
                    }
                };
            });
        }
    }
}