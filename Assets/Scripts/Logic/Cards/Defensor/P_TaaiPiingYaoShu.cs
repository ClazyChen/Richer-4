using System;
using System.Collections.Generic;
/// <summary>
/// 太平要术
/// </summary>
public class P_TaaiPiingYaoShu : PEquipmentCardModel {

    public override int AIInEquipExpectation(PGame Game, PPlayer Player) {
        return 2000;
    }

    public readonly static string CardName = "太平要术";

    public P_TaaiPiingYaoShu():base(CardName, PCardType.DefensorCard) {
        Point = 5;
        Index = 53;
        foreach (PTime Time in new PTime[] {
            PPeriod.StartTurn.During
        }) {
            MoveInEquipTriggerList.Add((PPlayer Player, PCard Card) => {
                return new PTrigger(CardName) {
                    Player = Player,
                    Time = Time,
                    Condition = (PGame Game) => {
                        return Player.Equals(Game.NowPlayer);
                    },
                    Effect = (PGame Game ) => {
                        AnnouceUseEquipmentSkill(Player);
                        Game.GetMoney(Player, 200);
                    }
                };
            });
        }
    }
}