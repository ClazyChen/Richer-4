using System;
using System.Linq;
using System.Collections.Generic;

public class P_ChenSheng : PGeneral {


    public P_ChenSheng() : base("陈胜") {
        Sex = PSex.Male;
        Age = PAge.Classic;
        Index = 21;
        Cost = 20;
        Tips = "定位：防御\n" +
            "难度：中等\n" +
            "史实：秦朝末年农民起义的领袖，建立了张楚政权。其名言“王侯将相宁有种乎”、“燕雀安知鸿鹄之志”等广为人知。\n" +
            "攻略：\n陈胜是一名拥有不俗控制能力和防御能力的武将。【起义】作为控制技能，能够在前期翻面敌人，从而迅速拉开己方团队与敌方团队的土地数量差距，同时能够在后期翻面自己或队友，从而减少敌人的伤害。在前期，使用【起义】控制敌人，可以帮助己方抢占地盘，同时使被翻面无法购买土地的敌人保持最高现金，制造连续控制的机会。【鸿鹄】大多数情况下配合【起义】使用，也可通过【金蝉脱壳】、【反间计】等卡牌发动。【鸿鹄】可以回避伤害的特性进一步增强了陈胜的防御，在后期，优势情况下的陈胜如果成为现金最多者，就可以连续【起义】自己形成滚雪球优势。陈胜的技能需要大量卡牌发动，因此可以通过大量建造研究所为陈胜创造条件。";

        PSkill QiYi = new PSkill("起义") {
            Initiative = true
        };
        SkillList.Add(QiYi
            .AddTimeTrigger(
            new PTime[] {
                PPeriod.FirstFreeTime.During,
                PPeriod.SecondFreeTime.During
            },
            (PTime Time, PPlayer Player, PSkill Skill) => {
                return new PTrigger(QiYi.Name) {
                    IsLocked = false,
                    Player = Player,
                    Time = Time,
                    AIPriority = 200,
                    CanRepeat = true,
                    Condition = (PGame Game) => {
                        return Player.Equals(Game.NowPlayer) &&
                        (Player.IsAI || Game.Logic.WaitingForEndFreeTime()) &&
                        Player.Area.HandCardArea.CardList.Exists((PCard Card) => Card.Point %2 == 1);
                    },
                    AICondition = (PGame Game) => {
                        int LeastValue = PAiCardExpectation.FindLeastValuable(Game, Player, Player, true, false, false, true, (PCard Card) => Card.Point % 2 == 1).Value;
                        return P_ChiinTsevChiinWang.AIEmitTargets(Game, Player, LeastValue)[0] != null;
                    },
                    Effect = (PGame Game) => {
                        QiYi.AnnouceUseSkill(Player);
                        PCard TargetCard = null;
                        if (Player.IsAI) {
                            TargetCard = PAiCardExpectation.FindLeastValuable(Game, Player, Player, true, false, false, true, (PCard Card) => Card.Point % 2 == 1).Key;
                        } else {
                            List<PCard> Waiting = Player.Area.HandCardArea.CardList.FindAll((PCard Card) => Card.Point % 2 == 1);
                            int Result = PNetworkManager.NetworkServer.ChooseManager.Ask(Player, QiYi.Name, Waiting.ConvertAll((PCard Card) => Card.Name).Concat(new List<string> { "取消" }).ToArray());
                            if (Result >= 0 && Result < Waiting.Count) {
                                TargetCard = Waiting[Result];
                            }
                        }
                        if (TargetCard != null) {
                            TargetCard.Model = new P_ChiinTsevChiinWang();
                            PTrigger Trigger = TargetCard.Model.MoveInHandTriggerList.Find((Func<PPlayer, PCard, PTrigger> TriggerGenerator) => TriggerGenerator(Player, TargetCard).Time.Equals(PPeriod.FirstFreeTime.During))?.Invoke(Player, TargetCard);
                            if (Trigger != null) {
                                Game.Logic.StartSettle(new PSettle("起义[擒贼擒王]", Trigger.Effect));
                            }
                        }
                    }
                };
            }));
        PSkill HongHu = new PSkill("鸿鹄") {
            SoftLockOpen = true
        };
        SkillList.Add(HongHu
            .AddTrigger(
            (PPlayer Player, PSkill Skill) => {
                return new PTrigger(HongHu.Name) {
                    IsLocked = false,
                    Player = Player,
                    Time = PTime.ChangeFaceTime,
                    AIPriority = 100,
                    Condition = (PGame Game) => {
                        PChangeFaceTag ChangeFaceTag = Game.TagManager.FindPeekTag<PChangeFaceTag>(PChangeFaceTag.TagName);
                        return Player.Equals(ChangeFaceTag.Player);
                    },
                    Effect = (PGame Game) => {
                        HongHu.AnnouceUseSkill(Player);
                        Game.GetMoney(Player, 2000);
                        int Step = 1;
                        if (Player.IsAI) {
                            Step = PMath.Max(new List<int> { 1, 2, 3, 4, 5, 6 }, (int StepNumber) => PAiMapAnalyzer.StartFromExpect(Game, Player, Game.Map.NextStepBlock(Player.Position, StepNumber), 0, false)).Key;
                        } else {
                            Step = PNetworkManager.NetworkServer.ChooseManager.Ask1To6(Player, QiYi.Name);
                        }
                        PNetworkManager.NetworkServer.TellClients(new PShowInformationOrder(Player.Name + "选择向前移动" + Step + "步"));
                        Game.MoveForward(Player, Step);
                    }
                };
            }));
    }

}