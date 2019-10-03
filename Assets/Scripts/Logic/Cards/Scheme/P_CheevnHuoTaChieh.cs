using System;
using System.Collections.Generic;
/// <summary>
/// 趁火打劫
/// </summary>
public class P_CheevnHuoTaChieh: PSchemeCardModel {

    private List<PPlayer> AIEmitTargets(PGame Game, PPlayer Player) {
        PPlayer Target = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName).ToPlayer;
        return new List<PPlayer>() { Target };
    }

    public override int AIInHandExpectation(PGame Game, PPlayer Player) {
        int Basic = 4000;
        return Math.Max(Basic, base.AIInHandExpectation(Game, Player));
    }

    public readonly static string CardName = "趁火打劫";

    public P_CheevnHuoTaChieh():base(CardName) {
        Point = 1;
        Index = 5;
        foreach (PTime Time in new PTime[] {
            PTime.Injure.AcceptInjure
        }) {
            MoveInHandTriggerList.Add((PPlayer Player, PCard Card) => {
                return new PTrigger(CardName) {
                    IsLocked = false,
                    Player = Player,
                    Time = Time,
                    AIPriority = 10,
                    Condition = (PGame Game) => {
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        return !Player.Equals(InjureTag.ToPlayer) && InjureTag.Injure > 0 && InjureTag.ToPlayer != null && InjureTag.ToPlayer.Area.HandCardArea.CardNumber + InjureTag.ToPlayer.Area.EquipmentCardArea.CardNumber > 0;
                    },
                    AICondition = (PGame Game) => {
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        return Player.TeamIndex != InjureTag.ToPlayer.TeamIndex && PAiCardExpectation.FindMostValuableToGet(Game, Player, InjureTag.ToPlayer).Value > 3000;
                    },
                    Effect = MakeNormalEffect(Player, Card, AIEmitTargets, AIEmitTargets,
                        (PGame Game, PPlayer User, PPlayer Target) => {
                            PCard Got = Game.GetCardFrom(User, Target);
                            #region 成就：分一杯羹
                            if (Got.Model is P_MuNiuLiuMa && User.Area.HandCardArea.CardList.Contains(Got)) {
                                PArch.Announce(Game, User, "分一杯羹");
                            }
                            #endregion
                        })
                };
            });
        }
    }
}