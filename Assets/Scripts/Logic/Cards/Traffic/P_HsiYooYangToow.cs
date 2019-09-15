
using System.Collections.Generic;
/// <summary>
/// 西域羊驼
/// </summary>
public class P_HsiYooYangToow : PEquipmentCardModel {

    public override int AIInEquipExpectation(PGame Game, PPlayer Player) {
        return 2000 * Game.Enemies(Player).FindAll((PPlayer _Player) => !_Player.Age.Equals(Player.Age)).Count;
    }

    public readonly static string CardName = "西域羊驼";

    public P_HsiYooYangToow():base(CardName, PCardType.TrafficCard) {
        Point = 4;
        Index = 58;
        foreach (PTime Time in new PTime[] {
            PTime.Card.LeaveAreaTime
        }) {
            MoveInEquipTriggerList.Add((PPlayer Player, PCard Card) => {
                return new PTrigger(CardName) {
                    IsLocked = true,
                    Player = Player,
                    Time = Time,
                    AIPriority = 50,
                    Condition = (PGame Game) => {
                        PMoveCardTag MoveCardTag = Game.TagManager.FindPeekTag<PMoveCardTag>(PMoveCardTag.TagName);
                        PPlayer SourceOwner = MoveCardTag.Source.Owner;
                        PPlayer DestinationOwner = MoveCardTag.Destination.Owner;
                        return Player.Equals(SourceOwner) && DestinationOwner!= null && !Player.Age.Equals(DestinationOwner.Age);
                    },
                    Effect = (PGame Game ) => {
                        AnnouceUseEquipmentSkill(Player);
                        PMoveCardTag MoveCardTag = Game.TagManager.FindPeekTag<PMoveCardTag>(PMoveCardTag.TagName);
                        MoveCardTag.Destination = Game.CardManager.ThrownCardHeap;
                    }
                };
            });
        }
    }
}