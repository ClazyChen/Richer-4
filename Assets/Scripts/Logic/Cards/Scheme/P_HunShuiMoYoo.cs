using System;
using System.Collections.Generic;
/// <summary>
/// 浑水摸鱼
/// </summary>
public class P_HunShuiMoYoo: PSchemeCardModel {

    private List<PPlayer> AIEmitTargets(PGame Game, PPlayer Player) {
        PUseCardTag UseCardTag = Game.TagManager.FindPeekTag<PUseCardTag>(PUseCardTag.TagName);
        return UseCardTag.TargetList;
    }

    public override int AIInHandExpectation(PGame Game, PPlayer Player) {
        int Basic = 1000;
        if (Game.AlivePlayerNumber <= 2) {
            return 0;
        }
        return Math.Max(Basic, base.AIInHandExpectation(Game, Player));
    }

    public readonly static string CardName = "浑水摸鱼";

    public P_HunShuiMoYoo():base(CardName) {
        Point = 4;
        Index = 20;
        foreach (PTime Time in new PTime[] {
            PTime.Card.EndSettleTime
        }) {
            MoveInHandTriggerList.Add((PPlayer Player, PCard Card) => {
                return new PTrigger(CardName) {
                    IsLocked = false,
                    Player = Player,
                    Time = Time,
                    AIPriority = 100,
                    Condition = (PGame Game) => {
                        PUseCardTag UseCardTag = Game.TagManager.FindPeekTag<PUseCardTag>(PUseCardTag.TagName);
                        return UseCardTag.TargetList.Count >= 2;
                    },
                    AICondition = (PGame Game) => {
                        int Value = 0;
                        PUseCardTag UseCardTag = Game.TagManager.FindPeekTag<PUseCardTag>(PUseCardTag.TagName);
                        UseCardTag.TargetList.ForEach((PPlayer _Player) => {
                            if (_Player.CanBeInjured && !(_Player.Defensor != null && _Player.Defensor.Model is P_YooHsi)) {
                                if (_Player.Money <= 500) {
                                    Value += 30000 * (Player.TeamIndex == _Player.TeamIndex ? -1 : 1);
                                } else {
                                    Value += 1000 * (Player.TeamIndex == _Player.TeamIndex ? 0 : 1);
                                }
                            }
                        });
                        return Value >= 1000 && !Player.OutOfGame && P_PanYue.XianJuTest(Game, Player);
                    },
                    Effect = MakeNormalEffect(Player, Card, AIEmitTargets, AIEmitTargets,
                        (PGame Game, PPlayer User, PPlayer Target) => {
                            Game.Injure(User, Target, 500, Card);
                        })
                };
            });
        }
    }
}