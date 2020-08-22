using System;
using System.Collections.Generic;
/// <summary>
/// 欲擒故纵
/// </summary>
public class P_YooChiinKuTsung: PSchemeCardModel {

    private List<PPlayer> AIEmitTargets(PGame Game, PPlayer Player) {
        PPlayer Target = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName).ToPlayer;
        return new List<PPlayer>() { Target };
    }

    public override int AIInHandExpectation(PGame Game, PPlayer Player) {
        int Basic = 6000;
        return Math.Max(Basic, base.AIInHandExpectation(Game, Player));
    }

    public readonly static string CardName = "欲擒故纵";

    public P_YooChiinKuTsung():base(CardName) {
        Point = 3;
        Index = 16;
        foreach (PTime Time in new PTime[] {
            PTime.Injure.EmitInjure
        }) {
            MoveInHandTriggerList.Add((PPlayer Player, PCard Card) => {
                return new PTrigger(CardName) {
                    IsLocked = false,
                    Player = Player,
                    Time = Time,
                    AIPriority = 10,
                    Condition = (PGame Game) => {
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        return Player.Equals(InjureTag.FromPlayer) && InjureTag.Injure > 0 && InjureTag.ToPlayer != null && InjureTag.ToPlayer.Area.HandCardArea.CardNumber > 0;
                    },
                    AICondition = (PGame Game) => {
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        if ( Player.TeamIndex == InjureTag.ToPlayer.TeamIndex) {
                            if (Player.General is P_IzayoiMiku && InjureTag.ToPlayer.General is P_Gabriel) {
                                return false;
                            }
                            return InjureTag.Injure >= InjureTag.ToPlayer.Money || (InjureTag.Injure >= 6000 && InjureTag.Injure + Player.Money >= 2* (InjureTag.ToPlayer.Money - InjureTag.Injure));
                        } else {
                            return InjureTag.Injure < InjureTag.ToPlayer.Money && InjureTag.Injure <= 1000 && InjureTag.ToPlayer.Area.HandCardArea.CardNumber >= 2 && PAiCardExpectation.FindMostValuableToGet(Game, Player, InjureTag.ToPlayer).Value > 3000;
                        }
                    },
                    Effect = MakeNormalEffect(Player, Card, AIEmitTargets, AIEmitTargets,
                        (PGame Game, PPlayer User, PPlayer Target) => {
                            PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                            PNetworkManager.NetworkServer.TellClients(new PPushTextOrder(User.Index.ToString(), "欲擒故纵：防止伤害", PPushType.Information.Name));
                            InjureTag.Injure = 0;
                            for (int i = 0; i < 2; ++ i) {
                                if (Target.Area.HandCardArea.CardNumber > 0) {
                                    PCard Got = Game.GetCardFrom(User, Target, true, false);
                                    #region 成就：七擒七纵
                                    if (Got.Model is P_NanManHsiang && User.Area.HandCardArea.CardList.Contains(Got)) {
                                        PArch.Announce(Game, User, "七擒七纵");
                                    }
                                    #endregion
                                }
                            }
                        })
                };
            });
        }
    }
}