
using System.Collections.Generic;
/// <summary>
/// 八卦阵
/// </summary>
public class P_PaKuaChevn : PEquipmentCardModel {

    public override int AIInEquipExpectation(PGame Game, PPlayer Player) {
        return 3000;
    }

    public readonly static string CardName = "八卦阵";

    public P_PaKuaChevn():base(CardName, PCardType.DefensorCard) {
        Point = 1;
        Index = 49;
        foreach (PTime Time in new PTime[] {
            PTime.Injure.AcceptInjure
        }) {
            MoveInEquipTriggerList.Add((PPlayer Player, PCard Card) => {
                return new PTrigger(CardName) {
                    IsLocked = false,
                    Player = Player,
                    Time = Time,
                    AIPriority = 150,
                    Condition = (PGame Game) => {
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        return Player.Equals(InjureTag.ToPlayer) && InjureTag.Injure > 0;
                    },
                    Effect = (PGame Game ) => {
                        int Result = Game.Judge(Player);
                        if (Result % 2 == 1) {
                            PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                            InjureTag.Injure = PMath.Percent(InjureTag.Injure, 50);
                        }
                    }
                };
            });
        }
    }
}