
using System.Collections.Generic;
/// <summary>
/// 落凤弓
/// </summary>
public class P_LoFevngKung : PEquipmentCardModel {

    public override int AIInEquipExpectation(PGame Game, PPlayer Player) {
        return 500 + 2000 * (int)PMath.Sum(Game.Enemies(Player).ConvertAll((PPlayer _Player) => (double)_Player.Area.EquipmentCardArea.CardNumber));
    }

    public readonly static string CardName = "落凤弓";

    public P_LoFevngKung():base(CardName, PCardType.WeaponCard) {
        Point = 5;
        Index = 47;
        foreach (PTime Time in new PTime[] {
            PTime.Injure.EmitInjure
        }) {
            MoveInEquipTriggerList.Add((PPlayer Player, PCard Card) => {
                return new PTrigger(CardName) {
                    IsLocked = false,
                    Player = Player,
                    Time = Time,
                    Condition = (PGame Game) => {
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        return Player.Equals(InjureTag.FromPlayer) && InjureTag.Injure > 0 && InjureTag.ToPlayer != null && InjureTag.ToPlayer.Area.EquipmentCardArea.CardNumber > 0;
                    },
                    AICondition = (PGame Game) => {
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        return InjureTag.ToPlayer.TeamIndex != Player.TeamIndex;
                    },
                    Effect = (PGame Game) => {
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        Game.ThrowCard(Player, InjureTag.ToPlayer, false);
                    }
                };
            });
        }
    }
}