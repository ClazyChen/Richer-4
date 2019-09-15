
using System.Collections.Generic;
/// <summary>
/// 赤兔
/// </summary>
public class P_ChiihTuu : PEquipmentCardModel {

    public override int AIInEquipExpectation(PGame Game, PPlayer Player) {
        return 2000;
    }

    public readonly static string CardName = "赤兔";


    // 赤兔的生效在上屋抽梯之后

    public P_ChiihTuu():base(CardName, PCardType.TrafficCard) {
        Point = 1;
        Index = 55;
        foreach (PTime Time in new PTime[] {
            PPeriod.WalkingStage.Start
        }) {
            MoveInEquipTriggerList.Add((PPlayer Player, PCard Card) => {
                return new PTrigger(CardName) {
                    IsLocked = true,
                    Player = Player,
                    Time = Time,
                    AIPriority = 100,
                    Condition = (PGame Game) => {
                        return Player.Equals(Game.NowPlayer);
                    },
                    Effect = (PGame Game ) => {
                        AnnouceUseEquipmentSkill(Player);
                        Game.TagManager.FindPeekTag<PStepCountTag>(PStepCountTag.TagName).StepCount ++;
                    }
                };
            });
        }
    }
}