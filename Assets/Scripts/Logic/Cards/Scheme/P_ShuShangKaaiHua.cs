using System;
using System.Collections.Generic;
/// <summary>
/// 树上开花
/// </summary>
public class P_ShuShangKaaiHua : PSchemeCardModel {

    public override int AIInHandExpectation(PGame Game, PPlayer Player) {
        int Basic = 2500;
        int Test = PMath.Max(Game.Map.BlockList.FindAll((PBlock Block) => Block.Lord != null && Block.Lord.TeamIndex == Player.TeamIndex), (PBlock Block) => {
            return PAiMapAnalyzer.HouseValue(Game, Block.Lord, Block);
        }).Value;
        Basic = Math.Max(Basic, Test);
        return Math.Max(Basic, base.AIInHandExpectation(Game, Player));
    }

    public readonly static string CardName = "树上开花";

    public P_ShuShangKaaiHua():base(CardName) {
        Point = 5;
        Index = 29;
        foreach (PTime Time in new PTime[] {
            PPeriod.FirstFreeTime.During,
            PPeriod.SecondFreeTime.During
        }) {
            MoveInHandTriggerList.Add((PPlayer Player, PCard Card) => {
                return new PTrigger(CardName) {
                    IsLocked = false,
                    Player = Player,
                    Time = Time,
                    AIPriority = 100,
                    Condition = (PGame Game) => {
                        return Player.Equals(Game.NowPlayer) && (Player.IsAI || Game.Logic.WaitingForEndFreeTime()) && Game.Map.BlockList.Exists((PBlock Block) => Block.Lord != null);
                    },
                    AICondition = (PGame Game) => {
                        return PMath.Max(Game.Map.BlockList.FindAll((PBlock Block) => Block.Lord != null && Block.Lord.TeamIndex == Player.TeamIndex), (PBlock Block) => {
                            return PAiMapAnalyzer.HouseValue(Game, Block.Lord, Block);
                        }).Value >= 2500;
                    },
                    Effect = (PGame Game) => {
                        List<PPlayer> Targets = new List<PPlayer>();
                        Game.Monitor.CallTime(PTime.Card.AfterEmitTargetTime, new PUseCardTag(Card, Player, Targets));
                        Game.CardManager.MoveCard(Card, Player.Area.HandCardArea, Game.CardManager.SettlingArea);
                        Game.Monitor.CallTime(PTime.Card.AfterBecomeTargetTime, new PUseCardTag(Card, Player, Targets));

                        PBlock Target = null;
                        if (Player.IsAI) {
                            Target = PMath.Max(Game.Map.BlockList.FindAll((PBlock Block) => Block.Lord != null && Block.Lord.TeamIndex == Player.TeamIndex), (PBlock Block) => {
                                return PAiMapAnalyzer.HouseValue(Game, Block.Lord, Block);
                            }).Key;
                        } else {
                            Target = PNetworkManager.NetworkServer.ChooseManager.AskToChooseBlock(Player, "树上开花[选择有主土地]", (PBlock Block) => Block.Lord != null);
                        }
                        if (Target != null) {
                            PNetworkManager.NetworkServer.TellClients(new PHighlightBlockOrder(Target.Index.ToString()));
                            Game.GetHouse(Target, 1);
                            #region 成就：花开富贵
                            if (Target.BusinessType.Equals(PBusinessType.Park)) {
                                PArch.Announce(Game, Player, "花开富贵");
                            }
                            #endregion
                        }

                        Game.CardManager.MoveCard(Card, Game.CardManager.SettlingArea, Game.CardManager.ThrownCardHeap);
                        Game.Monitor.CallTime(PTime.Card.EndSettleTime, new PUseCardTag(Card, Player, Targets));
                    }
                };
            });
        }
    }
}