
using System.Collections.Generic;
/// <summary>
/// 以逸待劳
/// </summary>
public class P_IITaiLao: PSchemeCardModel {

    private List<PPlayer> AIEmitTargets(PGame Game, PPlayer Player) {
        return Game.Teammates(Player);
    }

    public override int AIInHandExpectation(PGame Game, PPlayer Player) {
        return 0;
    }

    public readonly static string CardName = "以逸待劳";

    public P_IITaiLao():base(CardName) {
        Point = 1;
        Index = 4;
        foreach (PTime Time in new PTime[] {
            PPeriod.FirstFreeTime.During,
            PPeriod.SecondFreeTime.During
        }) {
            MoveInHandTriggerList.Add((PPlayer Player, PCard Card) => {
                return new PTrigger(CardName) {
                    IsLocked = false,
                    Player = Player,
                    Time = Time,
                    AIPriority = 21,
                    Condition = (PGame Game) => {
                        return Player.Equals(Game.NowPlayer) && (Player.IsAI || Game.Logic.WaitingForEndFreeTime());
                    },
                    AICondition = (PGame Game) => {
                        return PMath.Min(Player.Area.HandCardArea.CardList.FindAll((PCard _Card) => !_Card.Equals(Card)).ConvertAll((PCard _Card) => _Card.AIInHandExpectation(Game, Player))) <= 1000;
                    },
                    Effect = MakeMultiTargetNormalEffect(Player, Card, AIEmitTargets,
                        PTrigger.NoCondition,
                        (PGame Game, PPlayer User, PPlayer Target) => {
                            Game.GetCard(Target);
                            Game.ThrowCard(Target, Target);
                        })
                };
            });
        }
    }
}