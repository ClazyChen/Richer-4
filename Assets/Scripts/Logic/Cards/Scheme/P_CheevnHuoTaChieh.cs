
using System.Collections.Generic;
/// <summary>
/// 趁火打劫
/// </summary>
public class P_CheevnHuoTaChieh: PSchemeCardModel {

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
        return Basic;
    }

    public readonly static string CardName = "围魏救赵";

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
                    AIPriority = 100,
                    Condition = (PGame Game) => {
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        return Player.Equals(InjureTag.ToPlayer) && InjureTag.FromPlayer != null && InjureTag.Injure > 0;
                    },
                    AICondition = (PGame Game) => {
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        return Player.Money <= InjureTag.Injure || (InjureTag.Injure >= 3000 && InjureTag.FromPlayer.TeamIndex == Player.TeamIndex);
                    },
                    Effect = MakeNormalEffect(Player, Card, AIEmitTargets, AIEmitTargets,
                        (PGame Game, PPlayer User, PPlayer Target) => {
                            if (Game.PkPoint(User, Target) == 1) {
                                Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName).Injure = 0;
                            }
                        })
                };
            });
        }
    }
}