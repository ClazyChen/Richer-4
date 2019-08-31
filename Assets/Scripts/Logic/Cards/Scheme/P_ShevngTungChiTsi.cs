
using System.Collections.Generic;
/// <summary>
/// 声东击西
/// </summary>
public class P_ShevngTungChiHsi: PSchemeCardModel {

    private List<PPlayer> AIEmitTargets(PGame Game, PPlayer Player) {
        return new List<PPlayer>() {  };
    }

    public override int AIInHandExpectation(PGame Game, PPlayer Player) {
        return 3000;
    }

    public readonly static string CardName = "声东击西";

    public P_ShevngTungChiHsi():base(CardName) {
        Point = 1;
        Index = 6;
        foreach (PTime Time in new PTime[] {
            PTime.Card.AfterEmitTargetTime
        }) {
            MoveInHandTriggerList.Add((PPlayer Player, PCard Card) => {
                return new PTrigger(CardName) {
                    IsLocked = false,
                    Player = Player,
                    Time = Time,
                    AIPriority = 100,
                    Condition = (PGame Game) => {
                        PUseCardTag UseCardTag = Game.TagManager.FindPeekTag<PUseCardTag>(PUseCardTag.TagName);
                        return UseCardTag.TargetList.Count == 1;
                    },
                    AICondition = (PGame Game) => {
                        PUseCardTag UseCardTag = Game.TagManager.FindPeekTag<PUseCardTag>(PUseCardTag.TagName);
                        if (UseCardTag.Card.Name.Equals(P_ManTiienKuoHai.CardName) ||
                            UseCardTag.Card.Name.Equals(P_WeiWeiChiuChao.CardName) ||
                            UseCardTag.Card.Name.Equals(P_CheevnHuoTaChieh.CardName)) {
                            return UseCardTag.TargetList[0].TeamIndex == Player.TeamIndex && UseCardTag.User.TeamIndex != Player.TeamIndex;
                        }
                        if (UseCardTag.Card.Name.Equals(P_WuChungShevngYou.CardName)) {
                            return UseCardTag.TargetList[0].TeamIndex != Player.TeamIndex;
                        }
                        return false;
                    },
                    Effect = (PGame Game) => {
                        List<PPlayer> Targets = new List<PPlayer>();
                        Game.Monitor.CallTime(PTime.Card.AfterEmitTargetTime, new PUseCardTag(Card, Player, Targets));
                        Game.CardManager.MoveCard(Card, Player.Area.HandCardArea, Game.CardManager.SettlingArea);
                        Game.Monitor.CallTime(PTime.Card.StartSettleTime, new PUseCardTag(Card, Player, Targets));
                        PUseCardTag UseCardTag = Game.TagManager.FindPeekTag<PUseCardTag>(PUseCardTag.TagName);
                        PPlayer Target = null;
                        if (Player.IsUser) {
                            List<PPlayer> TargetList = new List<PPlayer>() { null };
                            TargetList.AddRange(Game.PlayerList.FindAll((PPlayer _Player) => !_Player.Equals(UseCardTag.TargetList[0]) && _Player.IsAlive));
                            List<string> TargetNameList = TargetList.ConvertAll((PPlayer _Player) => {
                                if (_Player == null) {
                                    return "令该计策牌无效";
                                } else {
                                    return "转移给：" + _Player.Name;
                                }
                            });
                            Target = TargetList[PNetworkManager.NetworkServer.ChooseManager.Ask(Player, "选择一项", TargetNameList.ToArray())];
                        } else {
                            if (UseCardTag.Card.Name.Equals(P_ManTiienKuoHai.CardName)) {
                                Target = ((P_ManTiienKuoHai)(UseCardTag.Card.Model)).AIEmitTargets(Game, Player)[0];
                            }
                            if (UseCardTag.Card.Name.Equals(P_WuChungShevngYou.CardName)) {
                                Target = PAiCardExpectation.MostValuableCardUser(Game, Game.Teammates(Player));
                            }
                        }
                        if (Target == null || Target == UseCardTag.TargetList[0]) {
                            PNetworkManager.NetworkServer.TellClients(new PShowInformationOrder("声东击西：无效"));
                            UseCardTag.TargetList.Clear();
                        } else {
                            PNetworkManager.NetworkServer.TellClients(new PShowInformationOrder("声东击西：转移给" + Target.Name));
                            UseCardTag.TargetList[0] = Target;
                        }
                        Game.CardManager.MoveCard(Card, Game.CardManager.SettlingArea, Game.CardManager.ThrownCardHeap);
                        Game.Monitor.CallTime(PTime.Card.EndSettleTime, new PUseCardTag(Card, Player, Targets));
                    }
            };
            });
        }
    }
}