using System;
using System.Collections.Generic;
/// <summary>
/// 借尸还魂
/// </summary>
public class P_ChiehShihHuanHun: PSchemeCardModel {

    public List<PPlayer> AIEmitTargets(PGame Game, PPlayer Player) {
        return new List<PPlayer> { Player };
    }

    public override int AIInHandExpectation(PGame Game, PPlayer Player) {
        int Basic = 3000;
        int Delta = PMath.Max(Game.PlayerList, (PPlayer _Player) => {
            return _Player.Money;
        }).Value - Player.Money;
        Delta = Math.Min(10000, Delta);
        if (Player.Money <= 2000) {
            Delta *= 3;
        } else if (Player.Money <= 5000) {
            Delta *= 2;
        }
        Delta -= 2000 * Player.Area.HandCardArea.CardNumber;
        Delta -= (int)PMath.Sum(Player.Area.EquipmentCardArea.CardList.ConvertAll((PCard _Card) => (double)_Card.Model.AIInEquipExpectation(Game, Player)));
        Basic = Math.Max(Delta, Basic);
        return Math.Max(Basic, base.AIInHandExpectation(Game, Player));
    }

    public readonly static string CardName = "借尸还魂";

    public P_ChiehShihHuanHun():base(CardName) {
        Point = 3;
        Index = 14;
        foreach (PTime Time in new PTime[] {
            PPeriod.FirstFreeTime.During,
            PPeriod.SecondFreeTime.During,
            PTime.EnterDyingTime
        }) {
            MoveInHandTriggerList.Add((PPlayer Player, PCard Card) => {
                return new PTrigger(CardName) {
                    IsLocked = false,
                    Player = Player,
                    Time = Time,
                    AIPriority = Time.Equals(PTime.EnterDyingTime) ? 10 : 90,
                    Condition = (PGame Game) => {
                        return Time.Equals(PTime.EnterDyingTime) ? Game.TagManager.FindPeekTag<PDyingTag>(PDyingTag.TagName).Player.Equals(Player) : Player.Equals(Game.NowPlayer) && (Player.IsAI || Game.Logic.WaitingForEndFreeTime());
                    },
                    AICondition = (PGame Game) => {
                        return Time.Equals(PTime.EnterDyingTime) || AIInHandExpectation(Game, Player) > 5000;
                    },
                    Effect = MakeNormalEffect(Player, Card, AIEmitTargets, AIEmitTargets,
                        (PGame Game, PPlayer User, PPlayer Target) => {
                            PPlayer Another = null;
                            if (Target.IsAI) {
                                Another = PMath.Max(Game.PlayerList.FindAll((PPlayer _Player) => !_Player.Equals(Player)), (PPlayer _Player) => {
                                    return _Player.Money;
                                }).Key;
                            } else {
                                Another = PNetworkManager.NetworkServer.ChooseManager.AskForTargetPlayer(Target, PTrigger.Except(Target), CardName + "[与其金钱相同]");
                            }
                            Game.CardManager.ThrowAll(Target.Area);
                            if (Another != null) {
                                if (Another.Money > Target.Money) {
                                    Game.GetMoney(Target, Math.Min(10000, Another.Money - Target.Money));
                                } else if (Another.Money < Target.Money) {
                                    Game.LoseMoney(Target, Target.Money - Another.Money);
                                }
                            }
                        })
                };
            });
        }
    }
}