using System;
using System.Collections.Generic;
/// <summary>
/// 关门捉贼
/// </summary>
public class P_KuanMevnChoTsev: PSchemeCardModel {

    public List<PPlayer> AIEmitTargets(PGame Game, PPlayer Player) {
        return Game.ListPlayers((PPlayer _Player) => _Player.Position.HouseNumber == 0 && _Player.IsAlive && !_Player.Equals(Player), Player);
    }

    public override int AIInHandExpectation(PGame Game, PPlayer Player) {
        int Basic = 2000;
        return Math.Max(Basic, base.AIInHandExpectation(Game, Player));
    }

    public readonly static string CardName = "关门捉贼";

    public P_KuanMevnChoTsev():base(CardName) {
        Point = 4;
        Index = 22;
        foreach (PTime Time in new PTime[] {
            PPeriod.FirstFreeTime.During,
            PPeriod.SecondFreeTime.During
        }) {
            MoveInHandTriggerList.Add((PPlayer Player, PCard Card) => {
                return new PTrigger(CardName) {
                    IsLocked = false,
                    Player = Player,
                    Time = Time,
                    AIPriority = 80,
                    Condition = (PGame Game) => {
                        return Player.Equals(Game.NowPlayer) && (Player.IsAI || Game.Logic.WaitingForEndFreeTime()) && AIEmitTargets(Game, Player).Count > 0;
                    },
                    AICondition = (PGame Game) => {
                        List<PPlayer> Targets = AIEmitTargets(Game, Player);
                        bool MultiTarget = Targets.Count > 1;
                        Targets = Targets.FindAll((PPlayer _Player) => !(_Player.HasEquipment<P_YooHsi>() && MultiTarget));
                        return PMath.Sum(Targets.ConvertAll((PPlayer Target) => (double)PAiTargetChooser.InjureExpect(Game, Player, Player, Target, 1000, Card ))) >= 2000 &&
                        P_PanYue.XianJuTest(Game, Player);
                    },
                    Effect = MakeNormalEffect(Player, Card, AIEmitTargets, AIEmitTargets,
                        (PGame Game, PPlayer User, PPlayer Target) => {
                            Game.Injure(User, Target, 1000, Card);
                        })
                };
            });
        }
    }
}