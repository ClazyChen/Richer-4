
using System.Collections.Generic;
/// <summary>
/// 诸葛连弩
/// </summary>
public class P_ChuKevLienNu : PEquipmentCardModel {

    public override int AIInEquipExpectation(PGame Game, PPlayer Player) {
        if (Player.Money <= 5000) {
            return 500;
        } else if (Player.Money <= 10000) {
            return 1000;
        } else {
            return 1200 * Game.Enemies(Player).Count;
        }
    }

    public readonly static string CardName = "诸葛连弩";

    public P_ChuKevLienNu():base(CardName, PCardType.WeaponCard) {
        Point = 1;
        Index = 43;
        foreach (PTime Time in new PTime[] {
            PPeriod.SettleStage.Start
        }) {
            MoveInEquipTriggerList.Add((PPlayer Player, PCard Card) => {
                return new PTrigger(CardName) {
                    IsLocked = true,
                    Player = Player,
                    Time = Time,
                    Condition = (PGame Game) => {
                        return Player.Equals(Game.NowPlayer);
                    },
                    Effect = (PGame Game ) => {
                        Game.TagManager.FindPeekTag<PPurchaseTag>(PPurchaseTag.TagName).Limit += 3;
                    }
                };
            });
        }
    }
}