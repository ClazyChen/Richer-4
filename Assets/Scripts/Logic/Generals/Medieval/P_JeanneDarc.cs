using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

public class P_JeanneDarc : PGeneral {
    public P_JeanneDarc() : base("贞德") {
        Sex = PSex.Female;
        Age = PAge.Medieval;
        Index = 30;
        Cost = 40;
        Tips = "定位：辅助\n" +
            "难度：简单\n" +
            "史实：法国民族英雄，天主教圣人，被称为“奥尔良的圣女”。在英法百年战争中，她带领法国军队对抗英军的入侵，屡战屡胜，扭转了战争的局势。\n" +
            "攻略：\n-";

        PSkill ShengNv = new PSkill("圣女");
        SkillList.Add(ShengNv
            .AddTrigger(
            (PPlayer Player, PSkill Skill) => {
                return new PTrigger(ShengNv.Name) {
                    IsLocked = true,
                    Player = Player,
                    Time = PTime.EnterDyingTime,
                    AIPriority = 5,
                    Condition = (PGame Game) => {
                        return Player.Area.OwnerCardNumber > 0;
                    },
                    AICondition = (PGame Game) => {
                        PDyingTag DyingTag = Game.TagManager.FindPeekTag<PDyingTag>(PDyingTag.TagName);
                        if (DyingTag.Killer.General is P_LvZhi) {
                            return false;
                        }
                        if (DyingTag.Player.TeamIndex == Player.TeamIndex) {
                            if (Player.Equals(DyingTag.Player)) {
                                return true;
                            } else {
                                if (DyingTag.Player.Money <= -10000) {
                                    return false;
                                }
                                if (Player.Money > 5000) {
                                    return true;
                                } else {
                                    return Player.Area.OwnerCardNumber <= 3;
                                }
                            }
                        } else {
                            return false;
                        }
                    },
                    Effect = (PGame Game) => {
                        ShengNv.AnnouceUseSkill(Player);
                        PDyingTag DyingTag = Game.TagManager.FindPeekTag<PDyingTag>(PDyingTag.TagName);
                        Game.CardManager.ThrowAll(Player.Area);
                        PCard Card = new P_ChiehTaoShaJevn().Instantiate();
                        Card.Point = 0;
                        PTrigger Trigger = Card.Model.MoveInHandTriggerList.Find((Func<PPlayer, PCard, PTrigger> TriggerGenerator) => TriggerGenerator(DyingTag.Player, Card).Time.Equals(PTime.EnterDyingTime))?.Invoke(DyingTag.Player, Card);
                        if (Trigger != null) {
                            Game.Logic.StartSettle(new PSettle("圣女[借尸还魂]", Trigger.Effect));
                            Game.LoseMoney(Player, 5000);
                            if (!Player.Equals(DyingTag.Player)) {
                                Game.GetCard(DyingTag.Player);
                            }
                        }
                    }
                };
            }));
    }

}