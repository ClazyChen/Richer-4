using System;
using System.Collections.Generic;
/// <summary>
/// 空城计
/// </summary>

public class PKuungCheevngChiTag : PNumberedTag {
    public static string TagName = "空城计";
    public PKuungCheevngChiTag() : base(TagName, 2) {

    }
}

public class P_KuungCheevngChi : PSchemeCardModel {

    public List<PPlayer> AIEmitTargets(PGame Game, PPlayer Player) {
        return new List<PPlayer> { Player };
    }

    public override int AIInHandExpectation(PGame Game, PPlayer Player) {
        int Basic = 0;
        return Math.Max(Basic, PAiMapAnalyzer.OutOfGameExpect(Game, Player, true, true) - 2000 * (Player.Area.HandCardArea.CardNumber - 1));
    }

    public readonly static string CardName = "空城计";

    public P_KuungCheevngChi():base(CardName) {
        Point = 6;
        Index = 32;
        foreach (PTime Time in new PTime[] {
            PPeriod.FirstFreeTime.During,
            PPeriod.SecondFreeTime.During
        }) {
            MoveInHandTriggerList.Add((PPlayer Player, PCard Card) => {
                return new PTrigger(CardName) {
                    IsLocked = false,
                    Player = Player,
                    Time = Time,
                    AIPriority = 30,
                    Condition = (PGame Game) => {
                        return Player.Equals(Game.NowPlayer) && (Player.IsAI || Game.Logic.WaitingForEndFreeTime());
                    },
                    AICondition = (PGame Game) => {
                        return Game.NowPeriod.Equals(PPeriod.FirstFreeTime) && AIInHandExpectation(Game, Player) >= 1000;
                    },
                    Effect = MakeNormalEffect(Player, Card, AIEmitTargets, AIEmitTargets,
                        (PGame Game, PPlayer User, PPlayer Target) => {
                            for (int i = Target.Area.HandCardArea.CardNumber-1; i>=0;--i) {
                                Game.CardManager.MoveCard(Target.Area.HandCardArea.CardList[i], Target.Area.HandCardArea, Game.CardManager.ThrownCardHeap);
                            }
                            if (Target.Tags.ExistTag(PKuungCheevngChiTag.TagName)) {
                                Target.Tags.PopTag<PKuungCheevngChiTag>(PKuungCheevngChiTag.TagName);
                            }
                            Target.Tags.CreateTag(new PKuungCheevngChiTag());
                            PTrigger KuungCheevngChiTrigger = null;
                            Game.Monitor.AddTrigger(KuungCheevngChiTrigger = new PTrigger("空城状态更新") {
                                IsLocked = true,
                                Player = Target,
                                Time = PPeriod.EndTurn.During,
                                Condition = (PGame _Game) => {
                                    return Target.Equals(_Game.NowPlayer);
                                },
                                Effect = (PGame _Game) => {
                                    if (Target.Tags.ExistTag(PKuungCheevngChiTag.TagName)) {
                                        if (--Target.Tags.FindPeekTag<PKuungCheevngChiTag>(PKuungCheevngChiTag.TagName).Value <= 0) {
                                            Target.Tags.PopTag<PKuungCheevngChiTag>(PKuungCheevngChiTag.TagName);
                                        }
                                        PNetworkManager.NetworkServer.TellClients(new PRefreshMarkStringOrder(Target));
                                    }
                                    if (!Target.Tags.ExistTag(PKuungCheevngChiTag.TagName)) {
                                        _Game.Monitor.RemoveTrigger(KuungCheevngChiTrigger);
                                    }
                                }
                            });
                        })
                };
            });
        }
    }
}