using System;
using System.Linq;
using System.Collections.Generic;

public class P_ChenSheng : PGeneral {


    public P_ChenSheng() : base("陈胜") {
        Sex = PSex.Male;
        Age = PAge.Classic;
        Index = 21;
        Tips = "定位：防御\n" +
            "难度：-\n" +
            "史实：秦朝末年农民起义的领袖，建立了张楚政权。其名言“王侯将相宁有种乎”、“燕雀安知鸿鹄之志”等广为人知。\n" +
            "攻略：\n暂无";

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