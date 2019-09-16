using System;
using System.Collections.Generic;
/// <summary>
/// 玉玺
/// </summary>
public class P_YooHsi : PEquipmentCardModel {

    public override int AIInEquipExpectation(PGame Game, PPlayer Player) {
        return 1500;
    }

    public readonly static string CardName = "玉玺";

    public P_YooHsi():base(CardName, PCardType.DefensorCard) {
        Point = 3;
        Index = 51;
        foreach (PTime Time in new PTime[] {
            PTime.Card.AfterBecomeTargetTime
        }) {
            MoveInEquipTriggerList.Add((PPlayer Player, PCard Card) => {
                return new PTrigger(CardName) {
                    IsLocked = true,
                    Player = Player,
                    Time = Time,
                    Condition = (PGame Game) => {
                        PUseCardTag UseCardTag = Game.TagManager.FindPeekTag<PUseCardTag>(PUseCardTag.TagName);
                        return UseCardTag.TargetList.Count >= 2 && UseCardTag.TargetList.Contains(Player) && UseCardTag.Card.Type.Equals(PCardType.SchemeCard);
                    },
                    Effect = (PGame Game ) => {
                        AnnouceUseEquipmentSkill(Player);
                        PUseCardTag UseCardTag = Game.TagManager.FindPeekTag<PUseCardTag>(PUseCardTag.TagName);
                        PLogger.Log("玉玺发动记录：" + UseCardTag.TargetList.Remove(Player));
                    }
                };
            });
        }
    }
}