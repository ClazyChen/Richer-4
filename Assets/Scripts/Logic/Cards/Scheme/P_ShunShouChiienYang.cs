
using System.Collections.Generic;
/// <summary>
/// 顺手牵羊
/// </summary>
public class P_ShunShouChiienYang: PSchemeCardModel {

    public List<PPlayer> AIEmitTargets(PGame Game, PPlayer Player) {

        return new List<PPlayer> { PMath.Max(Game.PlayerList.FindAll((PPlayer TargetPlayer) => TargetPlayer.IsAlive && !TargetPlayer.Equals(Player)), (PPlayer TargetPlayer) => {
            if (TargetPlayer.TeamIndex == Player.TeamIndex) {
                KeyValuePair<PCard, int> Test = PAiCardExpectation.FindLeastValuable(Game, TargetPlayer, TargetPlayer, true, true, true);
                if (Test.Key == null) {
                    return -4000;
                }
                int Least = Test.Value;
                int ForMe = Test.Key.Model.AIInHandExpectation(Game, Player);
                return ForMe - Least - 3000;
            } else {
                KeyValuePair<PCard, int> Test = PAiCardExpectation.FindMostValuable(Game, TargetPlayer, TargetPlayer, true, true, true);
                if (Test.Key == null) {
                    return -4000;
                }
                int Most = Test.Value;
                int ForMe = Test.Key.Model.AIInHandExpectation(Game, Player);
                return ForMe + Most - 3000;
            }
        }, true).Key };
    }

    public override int AIInHandExpectation(PGame Game, PPlayer Player) {
        int Basic = 4000;
        // 装备和伏兵另算
        return Basic;
    }

    public readonly static string CardName = "顺手牵羊";

    public P_ShunShouChiienYang():base(CardName) {
        Point = 2;
        Index = 12;
        foreach (PTime Time in new PTime[] {
            PPeriod.FirstFreeTime.During,
            PPeriod.SecondFreeTime.During
        }) {
            MoveInHandTriggerList.Add((PPlayer Player, PCard Card) => {
                return new PTrigger(CardName) {
                    IsLocked = false,
                    Player = Player,
                    Time = Time,
                    AIPriority = 120,
                    Condition = (PGame Game) => {
                        return Player.Equals(Game.NowPlayer) && (Player.IsAI || Game.Logic.WaitingForEndFreeTime()) && Game.PlayerList.Find((PPlayer _Player) => _Player.Area.CardNumber > 0 && !_Player.Equals(Player)) != null;
                    },
                    AICondition = (PGame Game) => {
                        return AIEmitTargets(Game, Player)[0] != null;
                    },
                    Effect = MakeNormalEffect(Player, Card, AIEmitTargets, (PGame Game, PPlayer _Player) => {
                        return _Player.Area.CardNumber > 0 && !_Player.Equals(Player);
                    },
                        (PGame Game, PPlayer User, PPlayer Target) => {
                            Game.GetCardFrom(User, Target, true, true, true);
                        })
                };
            });
        }
    }
}