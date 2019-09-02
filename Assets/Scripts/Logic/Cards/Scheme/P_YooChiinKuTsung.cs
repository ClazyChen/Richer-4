
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
        return Basic;
    }

    public readonly static string CardName = "欲擒故纵";

    public P_YooChiinKuTsung():base(CardName) {
        Point = 3;
        Index = 15;
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
                            return InjureTag.Injure >= InjureTag.ToPlayer.Money || InjureTag.Injure >= 6000;
                        } else {
                            return InjureTag.Injure < InjureTag.ToPlayer.Money && InjureTag.Injure <= 1000;
                        }
                    },
                    Effect = MakeNormalEffect(Player, Card, AIEmitTargets, AIEmitTargets,
                        (PGame Game, PPlayer User, PPlayer Target) => {
                            PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                            PNetworkManager.NetworkServer.TellClients(new PPushTextOrder(User.Index.ToString(), "欲擒故纵：防止伤害", PPushType.Information.Name));
                            InjureTag.Injure = 0;
                            for (int i = 0; i < 2; ++ i) {
                                if (Target.Area.HandCardArea.CardNumber > 0) {
                                    Game.GetCardFrom(User, Target, false);
                                }
                            }
                        })
                };
            });
        }
    }
}