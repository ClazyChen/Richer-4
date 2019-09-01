
using System.Collections.Generic;
/// <summary>
/// 瞒天过海
/// </summary>
public class P_ManTiienKuoHai: PSchemeCardModel {

    public List<PPlayer> AIEmitTargets(PGame Game, PPlayer Player) {
        int ExpectedMoney = 700;
        PPlayer Target = PAiTargetChooser.InjureTarget(Game, Player, PTrigger.Except(Player), ExpectedMoney);
        return new List<PPlayer>() { Target };
    }

    public override int AIInHandExpectation(PGame Game, PPlayer Player) {
        int Basic = 1400;
        int MinEnemyMoney = PMath.Min(Game.Enemies(Player), (PPlayer Test) =>  Test.Money).Value;
        if (MinEnemyMoney <= 1200 ) {
            Basic += 5000 * (7 - MinEnemyMoney / 200);
        }
        return Basic;
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
                    Condition = (PGame Game) => {
                        return Player.Equals(Game.NowPlayer) && (Player.IsAI || Game.Logic.WaitingForEndFreeTime());
                    },
                    AICondition = (PGame Game) => {
                        return AIEmitTargets(Game, Player)[0] != null;
                    },
                    Effect = MakeNormalEffect(Player, Card, AIEmitTargets,
                        PTrigger.Except(Player),
                        (PGame Game, PPlayer User, PPlayer Target) => {
                            Game.Injure(User, Target, Game.Judge(User) * 200);
                        })
                };
            });
        }
    }
}