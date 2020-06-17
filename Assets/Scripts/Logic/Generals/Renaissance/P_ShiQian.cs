using System;
using System.Linq;
using System.Collections.Generic;

public class P_ShiQian: PGeneral {

    public P_ShiQian() : base("时迁") {
        Sex = PSex.Male;
        Age = PAge.Renaissance;
        Index = 7;
        Tips = "定位：控制\n" +
            "难度：简单\n" +
            "史实：《水浒传》中人物，地贼星，绰号“鼓上蚤”，擅长偷盗，多次利用专业技能为梁山立功。\n" +
            "攻略：\n时迁的使用方法非常简单，只要使用【顺手牵羊】即可。【顺手牵羊】的时机把握是比较重要的问题，手中存留一张【顺手牵羊】，将其用于关键的【乐不思蜀】、【草木皆兵】或者装备上，会比使用【顺手牵羊】直到没有【顺手牵羊】为止效果更好。";

        PSkill FeiZei = new PSkill("飞贼") {
            Lock = true
        };
        SkillList.Add(FeiZei
            .AddTimeTrigger(
            new PTime[] {
                PTime.Card.AfterBecomeTargetTime
            },
            (PTime Time, PPlayer Player, PSkill Skill) => {
                return new PTrigger(FeiZei.Name) {
                    IsLocked = true,
                    Player = Player,
                    Time = Time,
                    AIPriority = 200,
                    Condition = (PGame Game) => {
                        PUseCardTag UseCardTag = Game.TagManager.FindPeekTag<PUseCardTag>(PUseCardTag.TagName);
                        return UseCardTag.TargetList.Contains(Player) && UseCardTag.Card.Model is P_ShunShouChiienYang;
                    },
                    Effect = (PGame Game) => {
                        FeiZei.AnnouceUseSkill(Player);
                        Game.TagManager.FindPeekTag<PUseCardTag>(PUseCardTag.TagName).TargetList.Remove(Player);
                    }
                };
            }));
        PSkill QingMin = new PSkill("轻敏") {
            Initiative = true
        };
        SkillList.Add(QingMin
            .AddTimeTrigger(
            new PTime[] {
                PPeriod.FirstFreeTime.During,
                PPeriod.SecondFreeTime.During
            },
            (PTime Time, PPlayer Player, PSkill Skill) => {
                return new PTrigger(QingMin.Name) {
                    IsLocked = false,
                    Player = Player,
                    Time = Time,
                    AIPriority = 200,
                    CanRepeat = true,
                    Condition = (PGame Game) => {
                        return Player.Equals(Game.NowPlayer) && 
                        (Player.IsAI || Game.Logic.WaitingForEndFreeTime()) && 
                        Game.PlayerList.Exists((PPlayer _Player) => _Player.Area.CardNumber > 0 && !_Player.Equals(Player) && _Player.IsAlive) && 
                        Player.Area.HandCardArea.CardList.Exists((PCard Card) => Card.Point == 1);
                    },
                    AICondition = (PGame Game) => {
                        return Player.Area.HandCardArea.CardList.Exists((PCard Card) => {
                            return Card.Point == 1 && P_ShunShouChiienYang.AIBaseEmitTargets(Game, Player, Card.Model.AIInHandExpectation(Game, Player) + 100)[0] != null;
                        });
                    },
                    Effect = (PGame Game) => {
                        QingMin.AnnouceUseSkill(Player);
                        PCard TargetCard = null;
                        if (Player.IsAI) {
                            TargetCard = PAiCardExpectation.FindLeastValuable(Game, Player, Player, true, false, false, true, (PCard Card) => Card.Point==1).Key;
                        } else {
                            List<PCard> Waiting = Player.Area.HandCardArea.CardList.FindAll((PCard Card) => Card.Point == 1);
                            int Result = PNetworkManager.NetworkServer.ChooseManager.Ask(Player, QingMin.Name, Waiting.ConvertAll((PCard Card) => Card.Name).Concat(new List<string> { "取消" }).ToArray());
                            if (Result >= 0 && Result < Waiting.Count) {
                                TargetCard = Waiting[Result];
                            }
                        }
                        if (TargetCard != null) {
                            TargetCard.Model = new P_ShunShouChiienYang();
                            PTrigger Trigger = TargetCard.Model.MoveInHandTriggerList.Find((Func<PPlayer, PCard, PTrigger> TriggerGenerator) => TriggerGenerator(Player, TargetCard).Time.Equals(PPeriod.FirstFreeTime.During))?.Invoke(Player, TargetCard);
                            if (Trigger != null) {
                                Game.Logic.StartSettle(new PSettle("轻敏[顺手牵羊]", Trigger.Effect));
                            }
                            Game.GetCard(Player);
                        }
                    }
                };
            }));
    }

}