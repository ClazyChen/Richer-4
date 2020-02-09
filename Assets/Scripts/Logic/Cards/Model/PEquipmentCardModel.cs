using System;
using System.Collections.Generic;


/// <summary>
/// PEquipmentCardModel：装备牌的模型基类
/// </summary>
public abstract class PEquipmentCardModel : PCardModel {

    public override int AIInHandExpectation(PGame Game, PPlayer Player) {
        PCard Current = Player.GetEquipment(Type);
        int Exp = AIInEquipExpectation(Game, Player);
        int Base = 0;
        int Basic = 0;
        if (Player.General is P_HuaXiong) {
            Base += 1000;
        }
        if (Player.General is P_TangYin) {
            Base += 500;
        }
        if (Player.General is P_HuaMulan) {
            Base += 2000;
        }
        if (Current != null && Exp <= Current.Model.AIInEquipExpectation(Game, Player)) {
            Basic = 500 + Base;
        } else {
            Basic = Exp + Base;
        }
        return Math.Max(Basic, base.AIInHandExpectation(Game, Player));
    }

    protected void AnnouceOnce(string CardName) {
        MoveInEquipTriggerList.Add((PPlayer Player, PCard Card) => {
            return new PTrigger(CardName) {
                IsLocked = true,
                Player = null,
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
                        KeyValuePair<PCard, int> MinCard = PMath.Min(Player.Area.HandCardArea.CardList.FindAll((PCard _Card) => _Card.Type.IsEquipment()),
                            (PCard _Card) => _Card.Model.AIInEquipExpectation(Game, Player));
                        PCard CurrentCard = Player.GetEquipment(CardType);
                        if (Player.General is P_HuaXiong) {
                            return CurrentCard == null && Card.Equals(MinCard.Key);
                        }
                        if (Player.General is P_TangYin) {
                            return CurrentCard == null;
                        }
                        int HuaMulanCof = Player.General is P_HuaMulan ? 2000 : 0;
                        return Card.Equals(MaxCard.Key) && (CurrentCard == null || MaxCard.Value + HuaMulanCof > CurrentCard.Model.AIInEquipExpectation(Game, Player)) && MaxCard.Value > 0;
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