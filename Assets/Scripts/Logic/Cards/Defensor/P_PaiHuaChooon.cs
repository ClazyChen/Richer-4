using System;
using System.Collections.Generic;
/// <summary>
/// 百花裙
/// </summary>
public class P_PaiHuaChooon : PEquipmentCardModel {

    public override int AIInEquipExpectation(PGame Game, PPlayer Player) {
        return 6000 * Game.Enemies(Player).FindAll((PPlayer _Player ) => !_Player.Sex.Equals(Player.Sex)).Count / Math.Min(1,Game.Enemies(Player).Count);
    }

    public readonly static string CardName = "百花裙";

    public P_PaiHuaChooon():base(CardName, PCardType.DefensorCard) {
        Point = 2;
        Index = 50;
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
                        return Player.Equals(InjureTag.ToPlayer) && InjureTag.Injure > 0 && InjureTag.FromPlayer != null && !InjureTag.FromPlayer.Sex.Equals(Player.Sex);
                    },
                    Effect = (PGame Game ) => {
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        InjureTag.Injure = PMath.Percent(InjureTag.Injure, 50);
                    }
                };
            });
        }
    }
}