
using System.Collections.Generic;
/// <summary>
/// 古锭刀
/// </summary>
public class P_KuTingTao : PEquipmentCardModel {

    public override int AIInEquipExpectation(PGame Game, PPlayer Player) {
        return 500 + 4000 * Game.Enemies(Player).FindAll((PPlayer _Player) => _Player.Area.HandCardArea.CardNumber == 0).Count;
    }

    public readonly static string CardName = "古锭刀";

    public P_KuTingTao():base(CardName, PCardType.WeaponCard) {
        Point = 2;
        Index = 44;
        foreach (PTime Time in new PTime[] {
            PTime.Injure.EmitInjure
        }) {
            MoveInEquipTriggerList.Add((PPlayer Player, PCard Card) => {
                return new PTrigger(CardName) {
                    IsLocked = true,
                    Player = Player,
                    Time = Time,
                    Condition = (PGame Game) => {
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        return Player.Equals(InjureTag.FromPlayer) && InjureTag.Injure > 0 && InjureTag.ToPlayer != null && InjureTag.ToPlayer.Area.HandCardArea.CardNumber == 0;
                    },
                    Effect = (PGame Game ) => {
                        AnnouceUseEquipmentSkill(Player);
                        Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName).Injure *= 2;
                    }
                };
            });
        }
    }
}