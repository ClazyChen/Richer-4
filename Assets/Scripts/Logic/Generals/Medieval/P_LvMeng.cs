using System;
using System.Collections.Generic;
using System.Linq;

public class P_LvMeng : PGeneral {
    public P_LvMeng() : base("吕蒙") {
        Sex = PSex.Male;
        Age = PAge.Medieval;
        Index = 26;
        Cost = 25;
        Tips = "定位：控制\n" +
            "难度：困难\n" +
            "史实：东汉末期东吴名将，“东吴四都督”之一。早年武勇过人，后勤奋读书，渐有国士之风，留有“士别三日当刮目相待”的佳话。\n" +
            "攻略：\n吕蒙是一名拥有强大控制和防御能力的武将。【勤学】能够极大加速牌堆的流动，同时检索出足量的装备，因此吕蒙对【诸葛连弩】的需求较高，而经过一定次数的【勤学】后又有极大概率找到【诸葛连弩】，因此吕蒙往往能够手握大量的装备牌。但吕蒙对房屋的需求很高，若无法获得【诸葛连弩】或建造公园，【勤学】的价值便下降很多。【勤学】对花木兰、关羽等需要装备牌的武将可以呈现明显克制，同时能够快速洗出新的【声东击西】、【借尸还魂】等关键牌，还可防止敌方拿到牌堆剩下的关键牌，因此对发动时机要求非常高。【白衣】能够与【勤学】形成高效的联动，因此吕蒙对赵云、关羽等通过过路费输出的武将拥有较高的防御力。吕蒙的防御力和加快牌堆流动的能力往往能将战局引导向对吕蒙有利的方向，但需要一定记忆牌堆的能力才能充分发挥其优势。";

        PSkill QinXue = new PSkill("勤学") {
            Initiative = true
        };
        const int QinXueParameter = 4;
        SkillList.Add(QinXue
            .AnnounceTurnOnce()
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
                        int CardCountExpectation = 0;
                        if (AllCardCount >= QinXueParameter) {
                            CardCountExpectation = PossibleEquipmentCount * QinXueParameter / AllCardCount;
                        }
                        if (Player.Area.EquipmentCardArea.CardNumber < 3) {
                            return CardCountExpectation * 2000 > MinBlock.Value;
                        } else {
                            return CardCountExpectation * 1500 > MinBlock.Value;
                        }
                    },
                    Effect = (PGame Game) => {
                        QinXue.AnnouceUseSkill(Player);
                        Game.ThrowHouse(Player, Player, QinXue.Name);
                        List<PCard> QinXueCardList = Game.GetCard(Player, QinXueParameter, true);
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
                            PNetworkManager.NetworkServer.TellClients(new PShowInformationOrder(Player.Name + "弃置了" + TargetCard.Name));
                            PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                            InjureTag.Injure = PMath.Percent(InjureTag.Injure, 50);
                        }
                    }
                };
            }));
    }

}