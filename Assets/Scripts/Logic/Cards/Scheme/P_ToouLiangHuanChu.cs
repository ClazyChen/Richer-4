using System;
using System.Collections.Generic;
/// <summary>
/// 偷梁换柱
/// </summary>
public class P_ToouLiangHuanChu : PSchemeCardModel {

    private KeyValuePair<KeyValuePair<PBlock,PBlock>, int> MaxLandPair(PGame Game, PPlayer Player) {
        int Test = int.MinValue;
        List<PBlock> LandList = Game.Map.BlockList.FindAll((PBlock Block) => Block.Lord != null && !Block.IsBusinessLand);
        PBlock Ans1 = null;
        PBlock Ans2 = null;
        foreach (PBlock Land1 in LandList) {
            foreach (PBlock Land2 in LandList) {
                if (Land1.Index < Land2.Index) {
                    int HouseCount1 = Land1.HouseNumber;
                    int HouseCount2 = Land2.HouseNumber;
                    int ValueBefore = PAiMapAnalyzer.HouseValue(Game, Player, Land1) * HouseCount1 +
                        PAiMapAnalyzer.HouseValue(Game, Player, Land2) * HouseCount2;
                    int ValueAfter = PAiMapAnalyzer.HouseValue(Game, Player, Land1) * HouseCount2 +
                        PAiMapAnalyzer.HouseValue(Game, Player, Land2) * HouseCount1;
                    if (ValueAfter - ValueBefore > Test) {
                        Test = ValueAfter - ValueBefore;
                        Ans1 = Land1;
                        Ans2 = Land2;
                    }
                }
            }
        }
        return new KeyValuePair<KeyValuePair<PBlock, PBlock>, int>(new KeyValuePair<PBlock, PBlock>(Ans1, Ans2), Test);
    }

    public override int AIInHandExpectation(PGame Game, PPlayer Player) {
        int Basic = 5400;
        int Test = MaxLandPair(Game, Player).Value;
        return Math.Max(Basic, Test);
    }

    public readonly static string CardName = "偷梁换柱";

    public P_ToouLiangHuanChu():base(CardName) {
        Point = 5;
        Index = 25;
        foreach (PTime Time in new PTime[] {
            PPeriod.FirstFreeTime.During,
            PPeriod.SecondFreeTime.During
        }) {
            MoveInHandTriggerList.Add((PPlayer Player, PCard Card) => {
                return new PTrigger(CardName) {
                    IsLocked = false,
                    Player = Player,
                    Time = Time,
                    AIPriority = 70,
                    Condition = (PGame Game) => {
                        return Player.Equals(Game.NowPlayer) && (Player.IsAI || Game.Logic.WaitingForEndFreeTime()) && Game.Map.BlockList.FindAll((PBlock Block) => Block.Lord != null && !Block.IsBusinessLand).Count >= 2;
                    },
                    AICondition = (PGame Game) => {
                        return MaxLandPair(Game, Player).Value >= 5400;
                    },
                    Effect = (PGame Game) => {
                        List<PPlayer> Targets = new List<PPlayer>();
                        Game.Monitor.CallTime(PTime.Card.AfterEmitTargetTime, new PUseCardTag(Card, Player, Targets));
                        Game.CardManager.MoveCard(Card, Player.Area.HandCardArea, Game.CardManager.SettlingArea);
                        Game.Monitor.CallTime(PTime.Card.AfterBecomeTargetTime, new PUseCardTag(Card, Player, Targets));

                        PBlock Land1 = null;
                        PBlock Land2 = null;
                        if (Player.IsAI) {
                            KeyValuePair<PBlock, PBlock> Lands = MaxLandPair(Game, Player).Key;
                            Land1 = Lands.Key;
                            PNetworkManager.NetworkServer.TellClients(new PHighlightBlockOrder(Land1.Index.ToString()));
                            Land2 = Lands.Value;
                            PNetworkManager.NetworkServer.TellClients(new PHighlightBlockOrder(Land2.Index.ToString()));
                        } else {
                            Land1 = PNetworkManager.NetworkServer.ChooseManager.AskToChooseBlock(Player, "偷梁换柱[第1片土地]", (PBlock Block) => Block.Lord != null && !Block.IsBusinessLand);
                            PNetworkManager.NetworkServer.TellClients(new PHighlightBlockOrder(Land1.Index.ToString()));
                            Land2 = PNetworkManager.NetworkServer.ChooseManager.AskToChooseBlock(Player, "偷梁换柱[第2片土地]", (PBlock Block) => Block.Lord != null && !Block.IsBusinessLand && !Block.Equals(Land1));
                            PNetworkManager.NetworkServer.TellClients(new PHighlightBlockOrder(Land2.Index.ToString()));
                        }
                        if (Land1 != null && Land2 != null) {
                            int House1 = Land1.HouseNumber;
                            int House2 = Land2.HouseNumber;
                            #region 成就：偷天换日
                            if (Math.Abs(House1-House2) >= 5) {
                                PArch.Announce(Game, Player, "偷天换日");
                            }
                            #endregion
                            Game.LoseHouse(Land1, Land1.HouseNumber);
                            Game.LoseHouse(Land2, Land2.HouseNumber);
                            Game.GetHouse(Land1, House2);
                            Game.GetHouse(Land2, House1);
                        }

                        Game.CardManager.MoveCard(Card, Game.CardManager.SettlingArea, Game.CardManager.ThrownCardHeap);
                        Game.Monitor.CallTime(PTime.Card.EndSettleTime, new PUseCardTag(Card, Player, Targets));
                    }
                };
            });
        }
    }
}