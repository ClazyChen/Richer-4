using System;
using System.Collections.Generic;


/// <summary>
/// PEquipmentCardModel：装备牌的模型基类
/// </summary>
public abstract class PEquipmentCardModel : PCardModel {

    public override int AIInHandExpectation(PGame Game, PPlayer Player) {
        PCard Current = Player.GetEquipment(Type);
        int Exp = AIInEquipExpectation(Game, Player);
        if (Current != null && Exp <= Current.Model.AIInEquipExpectation(Game, Player)) {
            return 500;
        } else {
            return Exp;
        }
    }

    protected void AnnouceOnce(string CardName) {
        MoveInEquipTriggerList.Add((PPlayer Player, PCard Card) => {
            return new PTrigger(CardName) {
                IsLocked = true,
                Player = Player,
                Time = PPeriod.StartTurn.Start,
                Condition = (PGame Game) => {
                    return Player.Equals(Game.NowPlayer);
                },
                Effect = (PGame Game) => {
                    Player.Tags.CreateTag(new PUsedTag(CardName, 1));
                }
            };
        });
    }

    protected void AnnouceUseEquipmentSkill(PPlayer Player) {
        PNetworkManager.NetworkServer.TellClients(new PShowInformationOrder(Player.Name + "发动了" + Name));
    }

    protected PEquipmentCardModel(string _Name, PCardType CardType):base(_Name) {
        Type = CardType;
        foreach (PTime Time in new PTime[] {
            PPeriod.FirstFreeTime.During,
            PPeriod.SecondFreeTime.During
        }) {
            MoveInHandTriggerList.Add((PPlayer Player, PCard Card) => {
                return new PTrigger("挂上装备") {
                    IsLocked = false,
                    Player = Player,
                    Time = Time,
                    AIPriority = 180,
                    Condition = (PGame Game) => {
                        return Player.Equals(Game.NowPlayer) && (Player.IsAI || Game.Logic.WaitingForEndFreeTime());
                    },
                    AICondition = (PGame Game) => {
                        KeyValuePair<PCard, int> MaxCard = PMath.Max(Player.Area.HandCardArea.CardList, (PCard _Card) => _Card.Model.AIInEquipExpectation(Game, Player));
                        PCard CurrentCard = Player.GetEquipment(CardType);
                        return Card.Equals(MaxCard.Key) && (CurrentCard == null || MaxCard.Value > CurrentCard.Model.AIInEquipExpectation(Game, Player));
                    },
                    Effect = (PGame Game) => {
                        List<PPlayer> Targets = new List<PPlayer> { Player };
                        Game.Monitor.CallTime(PTime.Card.AfterEmitTargetTime, new PUseCardTag(Card, Player, Targets));
                        Game.Monitor.CallTime(PTime.Card.AfterBecomeTargetTime, new PUseCardTag(Card, Player, Targets));
                        if (Targets.Count > 0) {
                            Game.CardManager.MoveCard(Card, Player.Area.HandCardArea, Targets[0].Area.EquipmentCardArea);
                        } else {
                            Game.CardManager.MoveCard(Card, Player.Area.HandCardArea, Game.CardManager.ThrownCardHeap);
                        }
                        Game.Monitor.CallTime(PTime.Card.EndSettleTime, new PUseCardTag(Card, Player, Targets));
                    }
                };
            });
        }
    }
}