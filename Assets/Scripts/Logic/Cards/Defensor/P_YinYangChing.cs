
using System.Collections.Generic;
/// <summary>
/// 阴阳镜
/// </summary>
public class P_YinYangChing : PEquipmentCardModel {

    public override int AIInEquipExpectation(PGame Game, PPlayer Player) {
        return 3000;
    }

    public readonly static string CardName = "阴阳镜";

    public P_YinYangChing():base(CardName, PCardType.DefensorCard) {
        Point = 6;
        Index = 54;
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
                        return Player.Equals(InjureTag.ToPlayer) && InjureTag.Injure > 0 && InjureTag.FromPlayer == null;
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