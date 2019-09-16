using System;
using System.Collections.Generic;
/// <summary>
/// 抛砖引玉
/// </summary>
public class P_PaaoChuanYinYoo : PSchemeCardModel {

    public List<PPlayer> AIEmitTargets(PGame Game, PPlayer Player) {
        return Game.ListPlayers((PPlayer _Player) => !_Player.Equals(Player), Player);
    }

    public override int AIInHandExpectation(PGame Game, PPlayer Player) {
        int Basic = 3000;
        if (!Game.Map.BlockList.Exists((PBlock Block) => Player.Equals(Block.Lord))) {
            return Basic;
        }
        int TargetNumber = Game.AlivePlayerNumber - 1;
        int Test = (int)PMath.Sum(Game.PlayerList.FindAll((PPlayer _Player) => _Player.IsAlive && !(_Player.Defensor != null && _Player.Defensor.Model is P_YooHsi && TargetNumber > 1)).ConvertAll((PPlayer _Player) => {
            return (double)PAiMapAnalyzer.MaxValueHouse(Game, Player, true).Value + PAiMapAnalyzer.MinValueHouse(Game, _Player).Value * (_Player.TeamIndex == Player.TeamIndex ? -1 : 1);
        }));
        
        return Math.Max(Basic,Test);
    }

    public readonly static string CardName = "抛砖引玉";

    public P_PaaoChuanYinYoo():base(CardName) {
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
                    AIPriority = 140,
                    Condition = (PGame Game) => {
                        return Player.Equals(Game.NowPlayer) && (Player.IsAI || Game.Logic.WaitingForEndFreeTime()) && Game.Map.BlockList.Exists((PBlock Block) => Player.Equals(Block.Lord)) && (Player.Area.EquipmentCardArea.CardNumber > 0 || Player.Area.HandCardArea.CardList.Exists((PCard _Card) => Card.Type.IsEquipment()));
                    },
                    AICondition = (PGame Game) => {
                        return Player.Area.HandCardArea.CardList.Exists((PCard _Card) => Card.Type.IsEquipment()) && AIInHandExpectation(Game, Player) > 3000;
                    },
                    Effect = MakeNormalEffect(Player, Card, AIEmitTargets, AIEmitTargets,
                        (PGame Game, PPlayer User, PPlayer Target) => {
                            if (Target.HasHouse) {
                                PBlock TargetBlock = null;
                                if (Target.IsAI) {
                                    TargetBlock = PAiMapAnalyzer.MinValueHouse(Game, Target).Key;
                                } else {
                                    TargetBlock = PNetworkManager.NetworkServer.ChooseManager.AskToChooseBlock(Target, "选择一座房屋以交给" + User.Name, (PBlock Block) => Target.Equals(Block.Lord) && Block.HouseNumber > 0);
                                }
                                if (TargetBlock != null) {
                                    Game.LoseHouse(TargetBlock, 1);
                                    TargetBlock = null;
                                    if (User.IsAI) {
                                        TargetBlock = PAiMapAnalyzer.MaxValueHouse(Game, User, true).Key;
                                    } else {
                                        TargetBlock = PNetworkManager.NetworkServer.ChooseManager.AskToChooseBlock(User, "选择一处你的土地放置房屋", (PBlock Block) => User.Equals(Block.Lord));
                                    }
                                    if (TargetBlock != null) {
                                        Game.GetHouse(TargetBlock, 1);
                                    }
                                }
                            }
                        }, (PGame Game, PPlayer User, List<PPlayer> Targets) => {
                            PCard TargetCard = null;
                            if (User.IsAI) {
                                TargetCard = PMath.Min(User.Area.HandCardArea.CardList.FindAll((PCard _Card) => _Card.Type.IsEquipment()), (PCard _Card) => _Card.Model.AIInHandExpectation(Game, User)).Key;
                            } else {
                                do {
                                    TargetCard = PNetworkManager.NetworkServer.ChooseManager.AskToChooseOwnCard(User, CardName + "[选择一张装备牌]", true, true);
                                } while (!TargetCard.Type.IsEquipment());
                            }
                            Game.CardManager.MoveCard(TargetCard, User.Area.HandCardArea.CardList.Contains(TargetCard) ? User.Area.HandCardArea : User.Area.EquipmentCardArea, Game.CardManager.ThrownCardHeap);
                        })
                };
            });
        }
    }
}