using System;
using System.Collections.Generic;
/// <summary>
/// 笑里藏刀
/// </summary>
public class P_HsiaoLiTsaangTao : PSchemeCardModel {

    private KeyValuePair<PCard, int> TeammateValueCard(PGame Game, PPlayer Player, PPlayer _Player) {
        return PMath.Max(Player.Area.HandCardArea.CardList.FindAll((PCard Card) => Card.Type.IsEquipment()), (PCard Card) => {
            int New = Card.Model.AIInEquipExpectation(Game, _Player);
            PCard CurrentEquip = _Player.GetEquipment(Card.Type);
            int Current = CurrentEquip == null ? 0 : CurrentEquip.Model.AIInEquipExpectation(Game, _Player);
            int Delta = (_Player.Area.EquipmentCardArea.CardNumber + (CurrentEquip == null ? 1 : 0)) * 500;
            return New - Current - (Delta >= _Player.Money ? 30000 : 0) + Math.Max(0, PAiCardExpectation.FindMostValuableToGet(Game, Player, _Player).Value);
        });
    }

    private KeyValuePair<PCard, int> EnemyValueCard(PGame Game, PPlayer Player, PPlayer _Player) {
        return PMath.Max(Player.Area.HandCardArea.CardList.FindAll((PCard Card) => Card.Type.IsEquipment()), (PCard Card) => {
            int New = Card.Model.AIInEquipExpectation(Game, _Player);
            PCard CurrentEquip = _Player.GetEquipment(Card.Type);
            int Current = CurrentEquip == null ? 0 : CurrentEquip.Model.AIInEquipExpectation(Game, _Player);
            int Delta = (_Player.Area.EquipmentCardArea.CardNumber + (CurrentEquip == null ? 1 : 0)) * 500;
            return (Delta >= _Player.Money ? 30000 : 0) + 2 * Delta - (New - Current) + Math.Max(0, PAiCardExpectation.FindMostValuableToGet(Game, Player, _Player).Value);
        });
    }

    public List<PPlayer> AIEmitTargets(PGame Game, PPlayer Player) {
        // 情况1：选择队友，当自己有多余的装备可以放在队友的空装备栏时且队友不会死时
        // 情况2：选择敌人，
        //        A、当可以杀死敌人时
        //        B、当可以换上更差的装备时

        KeyValuePair<PPlayer, int> TeammateTarget = PMath.Max(Game.Teammates(Player), (PPlayer _Player) => {
            return TeammateValueCard(Game, Player, _Player).Value;
        }, true);

        KeyValuePair<PPlayer, int> EnemyTarget = PMath.Max(Game.Enemies(Player), (PPlayer _Player) => {
            return EnemyValueCard(Game, Player, _Player).Value;
        }, true);

        PPlayer Target = TeammateTarget.Value > EnemyTarget.Value ? TeammateTarget.Key : EnemyTarget.Key;

        return new List<PPlayer>() { Target };
    }

    public override int AIInHandExpectation(PGame Game, PPlayer Player) {
        int Basic = Player.Area.HandCardArea.CardList.Exists((PCard Card) => Card.Type.IsEquipment()) ? 2000 : 0;
        int Test = 1000 * PMath.Max(Game.Enemies(Player), (PPlayer _Player) => _Player.Area.EquipmentCardArea.CardNumber).Value;
        return Math.Max(Basic, Test);
    }

    public readonly static string CardName = "笑里藏刀";

    public P_HsiaoLiTsaangTao():base(CardName) {
        Point = 2;
        Index = 10;
        foreach (PTime Time in new PTime[] {
            PPeriod.FirstFreeTime.During,
            PPeriod.SecondFreeTime.During
        }) {
            MoveInHandTriggerList.Add((PPlayer Player, PCard Card) => {
                return new PTrigger(CardName) {
                    IsLocked = false,
                    Player = Player,
                    Time = Time,
                    AIPriority = 145,
                    Condition = (PGame Game) => {
                        return Player.Equals(Game.NowPlayer) && (Player.IsAI || Game.Logic.WaitingForEndFreeTime()) && Player.Area.HandCardArea.CardList.Exists((PCard _Card) => _Card.Type.IsEquipment());
                    },
                    AICondition = (PGame Game) => {
                        return AIEmitTargets(Game, Player)[0] != null;
                    },
                    Effect = MakeNormalEffect(Player, Card, AIEmitTargets, PTrigger.Except(Player),
                        (PGame Game, PPlayer User, PPlayer Target) => {
                            PCard TargetCard = null;
                            if (User.IsAI) {
                                if (User.TeamIndex == Target.TeamIndex) {
                                    TargetCard = TeammateValueCard(Game, User, Target).Key;
                                } else {
                                    TargetCard = EnemyValueCard(Game, User, Target).Key;
                                }
                            } else {
                                do {
                                    TargetCard = PNetworkManager.NetworkServer.ChooseManager.AskToChooseOwnCard(User, CardName + "[选择一张手牌中的装备牌]", true, false);
                                } while (!TargetCard.Type.IsEquipment());
                            }
                            if (TargetCard != null) {
                                Game.CardManager.MoveCard(TargetCard, User.Area.HandCardArea, Target.Area.EquipmentCardArea);
                                Game.Injure(User, Target, 500 * Target.Area.EquipmentCardArea.CardNumber, Card);
                            }
                        })
                };
            });
        }
    }
}