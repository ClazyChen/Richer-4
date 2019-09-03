using System;
using System.Collections.Generic;
/// <summary>
/// 走为上计
/// </summary>
public class P_TsouWeiShangChi : PSchemeCardModel {

    public List<PPlayer> AIEmitTargets(PGame Game, PPlayer Player) {
        return new List<PPlayer> { Player };
    }

    public override int AIInHandExpectation(PGame Game, PPlayer Player) {
        int Basic = 5000;
        return Basic;
    }

    public readonly static string CardName = "走为上计";

    public P_TsouWeiShangChi():base(CardName) {
        Point = 6;
        Index = 36;
        foreach (PTime Time in new PTime[] {
            PTime.EnterDyingTime
        }) {
            MoveInHandTriggerList.Add((PPlayer Player, PCard Card) => {
                return new PTrigger(CardName) {
                    IsLocked = false,
                    Player = Player,
                    Time = Time,
                    AIPriority = 20,
                    Condition = (PGame Game) => {
                        return Time.Equals(PTime.EnterDyingTime) && Game.TagManager.FindPeekTag<PDyingTag>(PDyingTag.TagName).Player.Equals(Player);
                    },
                    Effect = MakeNormalEffect(Player, Card, AIEmitTargets, AIEmitTargets,
                        (PGame Game, PPlayer User, PPlayer Target) => {
                            Game.GetMoney(Target, 5000);
                            Target.Tags.CreateTag(PTag.OutOfGameTag);
                        })
                };
            });
        }
    }
}