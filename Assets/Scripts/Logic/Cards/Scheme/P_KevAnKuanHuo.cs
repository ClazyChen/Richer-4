
using System.Collections.Generic;
/// <summary>
/// 隔岸观火
/// </summary>
public class P_KevAnKuanHuo: PSchemeCardModel {
    public override int AIInHandExpectation(PGame Game, PPlayer Player) {
        return 1000;
    }

    public readonly static string CardName = "隔岸观火";

    public P_KevAnKuanHuo():base(CardName) {
        Point = 2;
        Index = 9;
        foreach (PTime Time in new PTime[] {
            PTime.Card.AfterBecomeTargetTime
        }) {
            MoveInHandTriggerList.Add((PPlayer Player, PCard Card) => {
                return new PTrigger(CardName) {
                    IsLocked = false,
                    Player = Player,
                    Time = Time,
                    AIPriority = 100,
                    Condition = (PGame Game) => {
                        PUseCardTag UseCardTag = Game.TagManager.FindPeekTag<PUseCardTag>(PUseCardTag.TagName);
                        return UseCardTag.TargetList.Count >= 2 && UseCardTag.TargetList.Contains(Player) && UseCardTag.Card.Type.Equals(PCardType.SchemeCard);
                    },
                    Effect = (PGame Game) => {
                        List<PPlayer> Targets = new List<PPlayer>();
                        Game.Monitor.CallTime(PTime.Card.AfterEmitTargetTime, new PUseCardTag(Card, Player, Targets));
                        Game.CardManager.MoveCard(Card, Player.Area.HandCardArea, Game.CardManager.SettlingArea);
                        Game.Monitor.CallTime(PTime.Card.AfterBecomeTargetTime, new PUseCardTag(Card, Player, Targets));
                        PUseCardTag UseCardTag = Game.TagManager.FindPeekTag<PUseCardTag>(PUseCardTag.TagName);
                        UseCardTag.TargetList.Remove(Player);
                        PTrigger GetMoneyTrigger = null;
                        GetMoneyTrigger = new PTrigger(CardName + "-摸500") {
                            IsLocked = true,
                            Player = Player,
                            Time = PTime.Card.EndSettleTime,
                            Condition = (PGame _Game) => {
                                PUseCardTag _UseCardTag = _Game.TagManager.FindPeekTag<PUseCardTag>(PUseCardTag.TagName);
                                return _UseCardTag.Card.Equals(UseCardTag.Card);
                            },
                            Effect = (PGame _Game) => {
                                _Game.GetMoney(Player, 500);
                                Game.Monitor.RemoveTrigger(GetMoneyTrigger);
                            }
                        };
                        Game.Monitor.AddTrigger(GetMoneyTrigger);
                        Game.CardManager.MoveCard(Card, Game.CardManager.SettlingArea, Game.CardManager.ThrownCardHeap);
                        Game.Monitor.CallTime(PTime.Card.EndSettleTime, new PUseCardTag(Card, Player, Targets));
                    }
            };
            });
        }
    }
}