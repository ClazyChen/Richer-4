using System;
using System.Collections.Generic;
/// <summary>
/// 打草惊蛇
/// </summary>
public class P_TaTsaaoChingShev: PSchemeCardModel {

    public List<PPlayer> AIEmitTargets(PGame Game, PPlayer Player) {
        return Game.ListPlayers((PPlayer _Player) => !_Player.Equals(Player) && _Player.HasHouse, Player);
    }

    public override int AIInHandExpectation(PGame Game, PPlayer Player) {
        int Basic = 0;
        bool MultiTarget = AIEmitTargets(Game, Player).Count > 1;
        Game.Teammates(Player, false).ForEach((PPlayer _Player) => {
            KeyValuePair<PBlock, int> Test = PAiMapAnalyzer.MinValueHouse(Game, _Player);
            if (Test.Key != null && !(_Player.Defensor != null && _Player.Defensor.Model is P_YooHsi && MultiTarget)) {
                Basic -= Test.Value;
            }
        });
        Game.Enemies(Player).ForEach((PPlayer _Player) => {
            KeyValuePair<PBlock, int> Test = PAiMapAnalyzer.MinValueHouse(Game, _Player);
            if (Test.Key != null && !(_Player.Defensor != null && _Player.Defensor.Model is P_YooHsi && MultiTarget)) {
                Basic += Test.Value;
            }
        });
        return Math.Max(Basic, 500);
    }

    public readonly static string CardName = "打草惊蛇";

    public P_TaTsaaoChingShev():base(CardName) {
        Point = 3;
        Index = 13;
        foreach (PTime Time in new PTime[] {
            PPeriod.FirstFreeTime.During,
            PPeriod.SecondFreeTime.During
        }) {
            MoveInHandTriggerList.Add((PPlayer Player, PCard Card) => {
                return new PTrigger(CardName) {
                    IsLocked = false,
                    Player = Player,
                    Time = Time,
                    AIPriority = 95,
                    Condition = (PGame Game) => {
                        return Player.Equals(Game.NowPlayer) && (Player.IsAI || Game.Logic.WaitingForEndFreeTime()) && AIEmitTargets(Game, Player).Count > 0;
                    },
                    AICondition = (PGame Game) => {
                        return AIInHandExpectation(Game, Player) > 900;
                    },
                    Effect = MakeNormalEffect(Player, Card, AIEmitTargets, AIEmitTargets,
                        (PGame Game, PPlayer User, PPlayer Target) => {
                            Game.ThrowHouse(Target, Target, CardName);
                        })
                };
            });
        }
    }
}