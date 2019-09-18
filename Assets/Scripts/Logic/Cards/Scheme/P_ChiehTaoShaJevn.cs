using System;
using System.Collections.Generic;
/// <summary>
/// 借刀杀人
/// </summary>
public class P_ChiehTaoShaJevn : PSchemeCardModel {

    private KeyValuePair<PPlayer, int> FindTarget(PGame Game, PPlayer Player) {
        int Basic = PAiMapAnalyzer.MaxValueHouse(Game, Player).Value;
        return PMath.Max(Game.Enemies(Player).FindAll((PPlayer _Player) => _Player.Area.EquipmentCardArea.CardNumber > 0), (PPlayer _Player) => {
            int HouseValue = _Player.HasHouse ?  PAiMapAnalyzer.MaxValueHouse(Game, _Player).Value + Basic : 30000;
            int EquipValue = PMath.Max(_Player.Area.EquipmentCardArea.CardList, (PCard Card ) => Card.Model.AIInEquipExpectation(Game, _Player) + Card.Model.AIInHandExpectation(Game, Player)).Value;
            return Math.Min(HouseValue, EquipValue);
        }, true);
    }

    public List<PPlayer> AIEmitTargets(PGame Game, PPlayer Player) {
        
        return new List<PPlayer>() { FindTarget(Game, Player).Key };
    }

    public override int AIInHandExpectation(PGame Game, PPlayer Player) {
        return Math.Max(500, FindTarget(Game, Player).Value);
    }

    public readonly static string CardName = "借刀杀人";

    public P_ChiehTaoShaJevn():base(CardName) {
        Point = 1;
        Index = 3;
        foreach (PTime Time in new PTime[] {
            PPeriod.FirstFreeTime.During,
            PPeriod.SecondFreeTime.During
        }) {
            MoveInHandTriggerList.Add((PPlayer Player, PCard Card) => {
                return new PTrigger(CardName) {
                    IsLocked = false,
                    Player = Player,
                    Time = Time,
                    AIPriority = 110,
                    Condition = (PGame Game) => {
                        return Player.Equals(Game.NowPlayer) && (Player.IsAI || Game.Logic.WaitingForEndFreeTime()) && Game.PlayerList.Exists((PPlayer _Player ) => !_Player.Equals(Player) && _Player.Area.EquipmentCardArea.CardNumber > 0);
                    },
                    AICondition = (PGame Game) => {
                        return AIEmitTargets(Game, Player)[0] != null;
                    },
                    Effect = MakeNormalEffect(Player, Card, AIEmitTargets, (PGame Game, PPlayer _Player) => {
                        return !_Player.Equals(Player) && _Player.Area.EquipmentCardArea.CardNumber > 0;
                    },
                        (PGame Game, PPlayer User, PPlayer Target) => {
                            int ChosenResult = -1;
                            if (!Target.HasHouse) {
                                if (Target.Area.EquipmentCardArea.CardNumber > 0) {
                                    ChosenResult = 1;
                                }
                            } else if (Target.Area.EquipmentCardArea.CardNumber == 0) {
                                ChosenResult = 0;
                            } else {
                                if (Target.IsAI) {
                                    ChosenResult = PAiMapAnalyzer.MaxValueHouse(Game, Target).Value > PAiCardExpectation.FindMostValuable(Game, Target, Target, false).Value ? 1 : 0;
                                } else {
                                    ChosenResult = PNetworkManager.NetworkServer.ChooseManager.Ask(Target, CardName + "-选择一项", new string[] {
                                        "令" + User.Name + "获得你1座房屋",
                                        "令" + User.Name + "获得你1张装备"
                                    });
                                }
                            }
                            if (ChosenResult == 0) {
                                PBlock TargetBlock = null;
                                if (User.IsAI) {
                                    TargetBlock = PAiMapAnalyzer.MaxValueHouse(Game, Target).Key;
                                } else {
                                    TargetBlock = PNetworkManager.NetworkServer.ChooseManager.AskToChooseBlock(User, "选择一座" + Target.Name + "的房屋", (PBlock Block) => Target.Equals(Block.Lord) && Block.HouseNumber > 0);
                                }
                                if (TargetBlock != null) {
                                    Game.LoseHouse(TargetBlock, 1);
                                    if (User.IsAI) {
                                        TargetBlock = PAiMapAnalyzer.MaxValueHouse(Game, User, true).Key;
                                    } else {
                                        TargetBlock = PNetworkManager.NetworkServer.ChooseManager.AskToChooseBlock(User, "选择一处你的土地放置房屋", (PBlock Block) => User.Equals(Block.Lord));
                                    }
                                    if (TargetBlock != null) {
                                        Game.GetHouse(TargetBlock, 1);
                                    }
                                }
                            } else if (ChosenResult == 1) {
                                Game.GetCardFrom(User, Target, false);
                            }
                        })
                };
            });
        }
    }
}