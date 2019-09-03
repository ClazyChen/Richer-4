using System;
using System.Collections.Generic;
/// <summary>
/// 声东击西
/// </summary>
public class P_ShevngTungChiHsi: PSchemeCardModel {

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
                        return UseCardTag.TargetList.Count == 1 && UseCardTag.Card.Type.Equals(PCardType.SchemeCard) && !UseCardTag.Card.Name.Equals(P_ChinChaanToowChiiao.CardName);
                    },
                    AICondition = (PGame Game) => {
                        PUseCardTag UseCardTag = Game.TagManager.FindPeekTag<PUseCardTag>(PUseCardTag.TagName);
                        if (UseCardTag.Card.Name.Equals(P_ManTiienKuoHai.CardName) ||
                            UseCardTag.Card.Name.Equals(P_WeiWeiChiuChao.CardName) ||
                            UseCardTag.Card.Name.Equals(P_CheevnHuoTaChieh.CardName) ||
                            UseCardTag.Card.Name.Equals(P_LiTaiTaaoChiang.CardName) ||
                            UseCardTag.Card.Name.Equals(P_ShunShouChiienYang.CardName) ||
                            UseCardTag.Card.Name.Equals(P_TaTsaaoChingShev.CardName) ||
                            UseCardTag.Card.Name.Equals(P_KuanMevnChoTsev.CardName) ||
                            UseCardTag.Card.Name.Equals(P_ChihSangMaHuai.CardName) ||
                            UseCardTag.Card.Name.Equals(P_FanChienChi.CardName)) {
                            return UseCardTag.TargetList[0].TeamIndex == Player.TeamIndex && UseCardTag.User.TeamIndex != Player.TeamIndex;
                        } else if (UseCardTag.Card.Name.Equals(P_WuChungShevngYou.CardName) ||
                            UseCardTag.Card.Name.Equals(P_AnTuCheevnTsaang.CardName) ||
                            UseCardTag.Card.Name.Equals(P_ChiehShihHuanHun.CardName) ||
                            UseCardTag.Card.Name.Equals(P_YooenChiaoChinKung.CardName) ||
                            UseCardTag.Card.Name.Equals(P_TsouWeiShangChi.CardName)) {
                            return UseCardTag.TargetList[0].TeamIndex != Player.TeamIndex;
                        } else if (UseCardTag.Card.Name.Equals(P_YooChiinKuTsung.CardName)) {
                            return UseCardTag.TargetList[0].TeamIndex == Player.TeamIndex && Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName).Injure <= 3000;
                        } else if (UseCardTag.Card.Name.Equals(P_ChiinTsevChiinWang.CardName)) {
                            return UseCardTag.User.TeamIndex != Player.TeamIndex && PMath.Max(Game.PlayerList, (PPlayer _Player) => {
                                if (Player.TeamIndex == _Player.TeamIndex) {
                                    return PAiMapAnalyzer.ChangeFaceExpect(Game, _Player);
                                } else {
                                    return -PAiMapAnalyzer.ChangeFaceExpect(Game, _Player);
                                }
                            }).Value - PAiMapAnalyzer.ChangeFaceExpect(Game, UseCardTag.TargetList[0]) * (UseCardTag.TargetList[0].TeamIndex == Player.TeamIndex ? 1 : -1) >= 3000;
                        } else if (UseCardTag.Card.Name.Equals(P_ShangWuChoouTii.CardName)) {
                            int NowValue = PAiMapAnalyzer.Expect(Game, UseCardTag.TargetList[0], UseCardTag.TargetList[0].Position) * (UseCardTag.TargetList[0].TeamIndex == Player.TeamIndex ? 1 : -1);
                            int MaxValue = PMath.Max(Game.PlayerList, (PPlayer _Player) => {
                                if (_Player.TeamIndex == Player.TeamIndex) {
                                    return PAiMapAnalyzer.Expect(Game, _Player, _Player.Position);
                                } else {
                                    return -PAiMapAnalyzer.Expect(Game, _Player, _Player.Position);
                                }
                            }, true).Value;
                            return MaxValue - NowValue >= 3000;
                        } else if (UseCardTag.Card.Name.Equals(P_KuungCheevngChi.CardName)) {
                            KeyValuePair<PPlayer,int> Target = PMath.Max(Game.Enemies(Player), (PPlayer _Player) => _Player.Area.HandCardArea.CardNumber);
                            return Target.Value >= 3 && !UseCardTag.TargetList[0].Equals(Target.Key);
                        }
                        return false;
                    },
                    Effect = (PGame Game) => {
                        List<PPlayer> Targets = new List<PPlayer>();
                        Game.Monitor.CallTime(PTime.Card.AfterEmitTargetTime, new PUseCardTag(Card, Player, Targets));
                        Game.CardManager.MoveCard(Card, Player.Area.HandCardArea, Game.CardManager.SettlingArea);
                        Game.Monitor.CallTime(PTime.Card.AfterBecomeTargetTime, new PUseCardTag(Card, Player, Targets));
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
                                Target = PAiTargetChooser.InjureTarget(Game, Player, PTrigger.Except(Player), 700);
                            } else if (UseCardTag.Card.Name.Equals(P_KuanMevnChoTsev.CardName)) {
                                Target = PAiTargetChooser.InjureTarget(Game, Player, PTrigger.Except(Player), 1000);
                            } else if (UseCardTag.Card.Name.Equals(P_ChihSangMaHuai.CardName)) {
                                Target = PAiTargetChooser.InjureTarget(Game, Player, PTrigger.Except(Player), Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName).Injure);
                            } else if (UseCardTag.Card.Name.Equals(P_ChiinTsevChiinWang.CardName)) {
                                Target = PMath.Max(Game.PlayerList, (PPlayer _Player) => {
                                    if (Player.TeamIndex == _Player.TeamIndex) {
                                        return PAiMapAnalyzer.ChangeFaceExpect(Game, _Player);
                                    } else {
                                        return -PAiMapAnalyzer.ChangeFaceExpect(Game, _Player);
                                    }
                                }).Key;
                            } else if (UseCardTag.Card.Name.Equals(P_TaTsaaoChingShev.CardName)) {
                                List<PPlayer> PossibleEnemies = Game.Enemies(Player).FindAll((PPlayer _Player) => Player.HasHouse);
                                if (PossibleEnemies.Count > 0) {
                                    Target = PossibleEnemies[PMath.RandInt(0, PossibleEnemies.Count - 1)];
                                }
                            } else if (UseCardTag.Card.Name.Equals(P_ChiehShihHuanHun.CardName)) {
                                int MaxMoney = PMath.Max(Game.PlayerList, (PPlayer _Player) => {
                                    return _Player.Money;
                                }).Value;
                                Target = PMath.Max(Game.PlayerList, (PPlayer _Player) => {
                                    int Delta = Math.Min(10000, MaxMoney - _Player.Money) - 2000 * _Player.Area.HandCardArea.CardNumber;
                                    // 装备待定
                                    if (_Player.TeamIndex == Player.TeamIndex) {
                                        return Delta;
                                    } else {
                                        return -Delta;
                                    }
                                }).Key;
                            } else if (UseCardTag.Card.Name.Equals(P_WuChungShevngYou.CardName) ||
                                UseCardTag.Card.Name.Equals(P_AnTuCheevnTsaang.CardName)) {
                                Target = PAiCardExpectation.MostValuableCardUser(Game, Game.Teammates(Player));
                            } else if (UseCardTag.Card.Name.Equals(P_ShangWuChoouTii.CardName)) {
                                Target = PMath.Max(Game.PlayerList, (PPlayer _Player) => {
                                    if (_Player.TeamIndex == Player.TeamIndex) {
                                        return PAiMapAnalyzer.Expect(Game, _Player, _Player.Position);
                                    } else {
                                        return -PAiMapAnalyzer.Expect(Game, _Player, _Player.Position);
                                    }
                                }, true).Key;
                            } else if (UseCardTag.Card.Name.Equals(P_KuungCheevngChi.CardName)) {
                                Target = PMath.Max(Game.Enemies(Player), (PPlayer _Player) => _Player.Area.HandCardArea.CardNumber).Key;
                            } else if (UseCardTag.Card.Name.Equals(P_TsouWeiShangChi.CardName)) {
                                Target = PMath.Max(Game.Teammates(Player), (PPlayer _Player) => PAiMapAnalyzer.OutOfGameExpect(Game, Player)).Key;
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