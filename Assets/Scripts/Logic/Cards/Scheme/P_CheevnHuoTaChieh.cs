
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
        return Basic;
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
                        return !Player.Equals(InjureTag.ToPlayer) && InjureTag.Injure > 0 && InjureTag.ToPlayer.Area.HandCardArea.CardNumber + InjureTag.ToPlayer.Area.EquipmentCardArea.CardNumber > 0;
                    },
                    AICondition = (PGame Game) => {
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        return Player.TeamIndex != InjureTag.ToPlayer.TeamIndex;
                    },
                    Effect = MakeNormalEffect(Player, Card, AIEmitTargets, AIEmitTargets,
                        (PGame Game, PPlayer User, PPlayer Target) => {
                            Game.GetCardFrom(User, Target);
                        })
                };
            });
        }
    }
}