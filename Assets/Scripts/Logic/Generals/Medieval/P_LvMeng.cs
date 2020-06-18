using System;
using System.Collections.Generic;
using System.Linq;

public class P_LvMeng : PGeneral {
    public P_LvMeng() : base("吕蒙") {
        Sex = PSex.Male;
        Age = PAge.Medieval;
        Index = 26;
        Cost = 25;
        NewGeneral = true;
        Tips = "定位：防御\n" +
            "难度：-\n" +
            "史实：东汉末期东吴名将，“东吴四都督”之一。早年武勇过人，后勤奋读书，渐有国士之风，留有“士别三日当刮目相待”的佳话。\n" +
            "攻略：\n暂无";

        PSkill QinXue = new PSkill("勤学") {
            Initiative = true
        };
        const int QinXueParameter = 4;
        SkillList.Add(QinXue
            .AnnouceTurnOnce()
            .AddTimeTrigger(
            new PTime[] {
                PPeriod.FirstFreeTime.During,
                PPeriod.SecondFreeTime.During
            },
            (PTime Time, PPlayer Player, PSkill Skill) => {
                return new PTrigger(QinXue.Name) {
                    IsLocked = false,
                    Player = Player,
                    Time = Time,
                    AIPriority = 185,
                    CanRepeat = true,
                    Condition = (PGame Game) => {
                        return Player.Equals(Game.NowPlayer) &&
                        (Player.IsAI || Game.Logic.WaitingForEndFreeTime()) &&
                        Player.HasHouse && Player.RemainLimit(QinXue.Name);
                    },
                    AICondition = (PGame Game) => {
                        KeyValuePair<PBlock, int> MinBlock = PAiMapAnalyzer.MinValueHouse(Game, Player, false, true);
                        if (MinBlock.Key == null) {
                            return false;
                        }
                        int PossibleEquipmentCount = Game.CardManager.CardHeap.CardList.FindAll((PCard _Card) => _Card.Type.IsEquipment()).Count;
                        int AllCardCount = Game.CardManager.CardHeap.CardNumber;
                        int CardCountExpectation = PossibleEquipmentCount * QinXueParameter / AllCardCount;
                        if (Player.Area.EquipmentCardArea.CardNumber < 3) {
                            return CardCountExpectation * 2000 > MinBlock.Value;
                        } else {
                            return CardCountExpectation * 1500 > MinBlock.Value;
                        }
                    },
                    Effect = (PGame Game) => {
                        QinXue.AnnouceUseSkill(Player);
                        Game.ThrowHouse(Player, Player, QinXue.Name);
                        List<PCard> QinXueCardList = new List<PCard>();
                        for (int i = 0; i < QinXueParameter; ++ i) {
                            PCard Card = Game.CardManager.CardHeap.TopCard;
                            PNetworkManager.NetworkServer.TellClients(new PShowInformationOrder(Player.Name + "展示了" + Card.Name));
                            QinXueCardList.Add(Card);
                            Game.CardManager.MoveCard(Card, Game.CardManager.CardHeap, Game.CardManager.SettlingArea);
                        }
                        foreach (PCard Card in QinXueCardList) {
                            if (Card.Type.IsEquipment()) {
                                Game.CardManager.MoveCard(Card, Game.CardManager.SettlingArea, Player.Area.HandCardArea);
                            } else {
                                Game.CardManager.MoveCard(Card, Game.CardManager.SettlingArea, Game.CardManager.ThrownCardHeap);
                            }
                        }
                        QinXue.DeclareUse(Player);
                    }
                };
            }));

        bool BaiYiCondition(PGame Game, PPlayer Player, PPlayer Source, int BaseInjure) {
            if (BaseInjure >= Player.Money && PMath.Percent(BaseInjure, 50) < Player.Money) {
                return true;
            } else {
                int Profit = (BaseInjure - PMath.Percent(BaseInjure, 50)) * (Source == null ? 1 : (Source.TeamIndex == Player.TeamIndex ? 0 : 2));
                int Value = PAiCardExpectation.FindLeastValuable(Game, Player, Player, true, true, false, true, (PCard Card) => Card.Type.IsEquipment()).Value;
                return Profit > Value + 1000;
            }
        }
        PSkill BaiYi = new PSkill("白衣");
        SkillList.Add(BaiYi
            .AddTrigger(
            (PPlayer Player, PSkill Skill) => {
                return new PTrigger(BaiYi.Name) {
                    IsLocked = false,
                    Player = Player,
                    Time = PTime.Injure.AcceptInjure,
                    AIPriority = 50,
                    Condition = (PGame Game) => {
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        return InjureTag.Injure > 0 && Player.Equals(InjureTag.ToPlayer) && InjureTag.InjureSource is PBlock && Player.HasEquipInArea();
                    },
                    AICondition = (PGame Game) => {
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        return BaiYiCondition(Game, Player, InjureTag.FromPlayer, InjureTag.Injure);
                    },
                    Effect = (PGame Game) => {
                        BaiYi.AnnouceUseSkill(Player);
                        PCard TargetCard = null;
                        if (Player.IsAI) {
                            TargetCard = PAiCardExpectation.FindLeastValuable(Game, Player, Player, true, true, false, true, (PCard Card) => Card.Type.IsEquipment()).Key;
                        } else {
                            do {
                                TargetCard = PNetworkManager.NetworkServer.ChooseManager.AskToChooseOwnCard(Player, BaiYi.Name + "[选择一张装备牌]", true, true);
                            } while (!TargetCard.Type.IsEquipment());
                        }
                        if (TargetCard != null) {
                            Game.CardManager.MoveCard(TargetCard, Player.Area.HandCardArea.CardList.Contains(TargetCard) ? Player.Area.HandCardArea : Player.Area.EquipmentCardArea, Game.CardManager.ThrownCardHeap);
                            PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                            InjureTag.Injure = PMath.Percent(InjureTag.Injure, 50);
                        }
                    }
                };
            }));
    }

}