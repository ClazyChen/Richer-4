using System;
using System.Collections.Generic;
/// <summary>
/// 七星袍
/// </summary>
public class P_ChiiHsingPaao : PEquipmentCardModel {

    public override int AIInEquipExpectation(PGame Game, PPlayer Player) {
        double Sum = PMath.Sum(Game.CardManager.CardHeap.CardList.ConvertAll((PCard Card) => (double)Card.Model.AIInAmbushExpectation(Game, Player)));
        int Count = Game.CardManager.CardHeap.CardNumber;

        if (Count <= 10) {
            Sum += PMath.Sum(Game.CardManager.ThrownCardHeap.CardList.ConvertAll((PCard Card) => (double)Card.Model.AIInAmbushExpectation(Game, Player)));
            Count += Game.CardManager.ThrownCardHeap.CardNumber;
        }

        if (Count == 0) {
            return 0;
        } else {
            return (int)(Sum / Count) * 6;
        }
    }

    public readonly static string CardName = "七星袍";

    public P_ChiiHsingPaao():base(CardName, PCardType.DefensorCard) {
        Point = 4;
        Index = 52;
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
                        return UseCardTag.TargetList.Contains(Player) && UseCardTag.Card.Type.Equals(PCardType.AmbushCard);
                    },
                    Effect = (PGame Game ) => {
                        AnnouceUseEquipmentSkill(Player);
                        PUseCardTag UseCardTag = Game.TagManager.FindPeekTag<PUseCardTag>(PUseCardTag.TagName);
                        UseCardTag.TargetList.Remove(Player);
                    }
                };
            });
        }
    }
}