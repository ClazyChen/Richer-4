
using System.Collections.Generic;
/// <summary>
/// 南蛮象
/// </summary>
public class P_NanManHsiang : PEquipmentCardModel {

    public override int AIInEquipExpectation(PGame Game, PPlayer Player) {
        return 3000;
    }

    public readonly static string CardName = "南蛮象";

    public P_NanManHsiang():base(CardName, PCardType.TrafficCard) {
        Point = 5;
        Index = 59;
        foreach (PTime Time in new PTime[] {
            PTime.Injure.StartSettle
        }) {
            MoveInEquipTriggerList.Add((PPlayer Player, PCard Card) => {
                return new PTrigger(CardName) {
                    IsLocked = true,
                    Player = Player,
                    Time = Time,
                    AIPriority = 100,
                    Condition = (PGame Game) => {
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        return Player.Equals(InjureTag.ToPlayer) && InjureTag.Injure > 0 && InjureTag.Injure <= 1000;
                    },
                    Effect = (PGame Game ) => {
                        AnnouceUseEquipmentSkill(Player);
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        InjureTag.Injure = 0;
                    }
                };
            });
        }
    }
}