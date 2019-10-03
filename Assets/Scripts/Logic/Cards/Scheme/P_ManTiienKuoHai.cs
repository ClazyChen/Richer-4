using System;
using System.Collections.Generic;
/// <summary>
/// 瞒天过海
/// </summary>
public class P_ManTiienKuoHai: PSchemeCardModel {

    public List<PPlayer> AIEmitTargets(PGame Game, PPlayer Player) {
        int ExpectedMoney = 700;
        if (Player.General is P_LiuJi) {
            ExpectedMoney = 1200;
        }
        PPlayer Target = PAiTargetChooser.InjureTarget(Game, Player, Player, PTrigger.Except(Player), ExpectedMoney, Instantiate());
        return new List<PPlayer>() { Target };
    }

    public override int AIInHandExpectation(PGame Game, PPlayer Player) {
        int Basic = 1400;
        if (Player.General is P_LiuJi) {
            Basic = 2400;
        }
        int MinEnemyMoney = PMath.Min(Game.Enemies(Player), (PPlayer Test) =>  Test.Money).Value;
        if (MinEnemyMoney <= 1200 ) {
            Basic += 5000 * (7 - MinEnemyMoney / 200);
        }
        return Math.Max(Basic, base.AIInHandExpectation(Game, Player));
    }

    public readonly static string CardName = "瞒天过海";

    public P_ManTiienKuoHai():base(CardName) {
        Point = 1;
        Index = 1;
        foreach (PTime Time in new PTime[] {
            PPeriod.FirstFreeTime.During,
            PPeriod.SecondFreeTime.During
        }) {
            MoveInHandTriggerList.Add((PPlayer Player, PCard Card) => {
                return new PTrigger(CardName) {
                    IsLocked = false,
                    Player = Player,
                    Time = Time,
                    AIPriority = 170,
                    Condition = PTrigger.Initiative(Player),
                    AICondition = (PGame Game) => {
                        return AIEmitTargets(Game, Player)[0] != null && !Player.OutOfGame && P_PanYue.XianJuTest(Game,Player);
                    },
                    Effect = MakeNormalEffect(Player, Card, AIEmitTargets,
                        PTrigger.Except(Player),
                        (PGame Game, PPlayer User, PPlayer Target) => {
                            Game.Injure(User, Target, Game.Judge(User, 6) * 200, Card);
                        })
                };
            });
        }
    }
}