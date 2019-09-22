
using System.Collections.Generic;
/// <summary>
/// 无中生有
/// </summary>
public class P_WuChungShevngYou: PSchemeCardModel {

    public List<PPlayer> AIEmitTargets(PGame Game, PPlayer Player) {
        return new List<PPlayer> { Player };
    }

    public override int AIInHandExpectation(PGame Game, PPlayer Player) {
        int Basic = 4000;
        return Basic;
    }

    public readonly static string CardName = "无中生有";

    public P_WuChungShevngYou():base(CardName) {
        Point = 2;
        Index = 7;
        foreach (PTime Time in new PTime[] {
            PPeriod.FirstFreeTime.During,
            PPeriod.SecondFreeTime.During
        }) {
            MoveInHandTriggerList.Add((PPlayer Player, PCard Card) => {
                return new PTrigger(CardName) {
                    IsLocked = false,
                    Player = Player,
                    Time = Time,
                    AIPriority = 140,
                    Condition = (PGame Game) => {
                        return Player.Equals(Game.NowPlayer) && (Player.IsAI || Game.Logic.WaitingForEndFreeTime());
                    },
                    AICondition = (PGame Game) => {
                        return AIEmitTargets(Game, Player)[0] != null;
                    },
                    Effect = MakeNormalEffect(Player, Card, AIEmitTargets, AIEmitTargets,
                        (PGame Game, PPlayer User, PPlayer Target) => {
                            List<PCard> Gots = Game.GetCard(Target, 2);
                            #region 成就：鸿运当头
                            if (Gots.Exists((PCard Got) => Got.Model is P_WuChungShevngYou && User.Area.HandCardArea.CardList.Contains(Got))) {
                                PArch.Announce(Game, User, "鸿运当头");
                            }
                            #endregion
                        })
                };
            });
        }
    }
}