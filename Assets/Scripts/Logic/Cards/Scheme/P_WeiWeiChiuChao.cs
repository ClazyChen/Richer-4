using System;
using System.Collections.Generic;
/// <summary>
/// 围魏救赵
/// </summary>
public class P_WeiWeiChiuChao: PSchemeCardModel {

    private List<PPlayer> AIEmitTargets(PGame Game, PPlayer Player) {
        PPlayer Target = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName).FromPlayer;
        return new List<PPlayer>() { Target };
    }

    public override int AIInHandExpectation(PGame Game, PPlayer Player) {
        int Basic = 1000;
        foreach (int Thr in new int[] {
            15000, 8000, 5000, 3000, 2000, 1500, 1000, 600, 400, 200
        }) {
            if (Player.Money <= Thr) {
                Basic += 500;
            }
        }
        return Math.Max(Basic, base.AIInHandExpectation(Game, Player));
    }

    public readonly static string CardName = "围魏救赵";

    public P_WeiWeiChiuChao():base(CardName) {
        Point = 1;
        Index = 2;
        foreach (PTime Time in new PTime[] {
            PTime.Injure.AcceptInjure
        }) {
            MoveInHandTriggerList.Add((PPlayer Player, PCard Card) => {
                return new PTrigger(CardName) {
                    IsLocked = false,
                    Player = Player,
                    Time = Time,
                    AIPriority = 100,
                    Condition = (PGame Game) => {
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        return Player.Equals(InjureTag.ToPlayer) && InjureTag.FromPlayer != null && InjureTag.Injure > 0;
                    },
                    AICondition = (PGame Game) => {
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        return Player.Money <= InjureTag.Injure || (InjureTag.Injure >= 3000 && InjureTag.FromPlayer.TeamIndex != Player.TeamIndex);
                    },
                    Effect = MakeNormalEffect(Player, Card, AIEmitTargets, AIEmitTargets,
                        (PGame Game, PPlayer User, PPlayer Target) => {
                            if (Game.PkPoint(User, Target) == 1) {
                                PNetworkManager.NetworkServer.TellClients(new PPushTextOrder(User.Index.ToString(), "围魏救赵：防止伤害", PPushType.Information.Name));
                                #region 成就：千钧一发
                                if (Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName).Injure >= 10000) {
                                    PArch.Announce(Game, User, "千钧一发");
                                }
                                #endregion
                                Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName).Injure = 0;
                            }
                        })
                };
            });
        }
    }
}