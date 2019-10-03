using System;
using System.Collections.Generic;
/// <summary>
/// 李代桃僵
/// </summary>
public class P_LiTaiTaaoChiang: PSchemeCardModel {

    private List<PPlayer> AIEmitTargets(PGame Game, PPlayer Player) {
        PPlayer Target = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName).FromPlayer;
        return new List<PPlayer>() { Target };
    }

    public override int AIInHandExpectation(PGame Game, PPlayer Player) {
        int Basic = 0;
        foreach (int Thr in new int[] {
            15000, 8000, 5000, 3000, 2000, 1500, 1000, 600, 400, 200
        }) {
            if (Player.Money <= Thr) {
                Basic += 1000;
            }
        }
        Basic = Math.Max(Basic, 2000);
        return Math.Max(Basic, base.AIInHandExpectation(Game, Player));
    }

    public readonly static string CardName = "李代桃僵";

    public P_LiTaiTaaoChiang():base(CardName) {
        Point = 2;
        Index = 11;
        foreach (PTime Time in new PTime[] {
            PTime.Injure.AcceptInjure
        }) {
            MoveInHandTriggerList.Add((PPlayer Player, PCard Card) => {
                return new PTrigger(CardName) {
                    IsLocked = false,
                    Player = Player,
                    Time = Time,
                    AIPriority = 80,
                    Condition = (PGame Game) => {
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        return Player.Equals(InjureTag.ToPlayer) && InjureTag.FromPlayer != null && InjureTag.Injure > 0 && Player.HasHouse;
                    },
                    AICondition = (PGame Game) => {
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        return Player.Money <= InjureTag.Injure || (InjureTag.Injure >= 1000 + PAiMapAnalyzer.MaxValueHouse(Game, Player).Value && InjureTag.FromPlayer.TeamIndex != Player.TeamIndex);
                    },
                    Effect = MakeNormalEffect(Player, Card, AIEmitTargets, AIEmitTargets,
                        (PGame Game, PPlayer User, PPlayer Target) => {
                            PNetworkManager.NetworkServer.TellClients(new PPushTextOrder(User.Index.ToString(), "李代桃僵：防止伤害", PPushType.Information.Name));
                            #region 成就：桃李不言
                            if (Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName).Injure >= User.Money) {
                                PArch.Announce(Game, User, "桃李不言");
                            }
                            #endregion
                            Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName).Injure = 0;
                            Game.ThrowHouse(Target, User, CardName);
                        })
                };
            });
        }
    }
}