using System;
using System.Collections.Generic;
using System.Linq;

public class P_Gryu : PGeneral {
    public P_Gryu() : base("关羽") {
        Sex = PSex.Male;
        Age = PAge.Medieval;
        Index = 18;
        Cost = 20;
        Tips = "定位：攻辅一体\n" +
            "难度：简单\n" +
            "史实：三国时期蜀汉名将，“五虎上将”之一。曾有在万军之中斩杀上将颜良，水淹七军威震华夏等壮举。被尊为“武圣”。\n" +
            "攻略：\n关羽是一个较为稳定的武将，操作较为简单，【武圣】提供了一种类似诸葛连弩的输出方式，偶数牌的数量保证了技能的发动频率，因而购物中心和研究所是关羽优先选择的方向。即使没有高地价土地，关羽也可以利用废牌增加输出能力。\n" +
            "【怒斩】是一个稳定的输出技能，在破坏装备卡牌缺乏的环境中容易建立起持久的优势，并可在一定程度上威慑华雄【叫阵】及其他消耗装备牌技能的发动，同时给关羽一个较强的续航能力。";

        PSkill WuSheng = new PSkill("武圣") {
            Initiative = true
        };
        SkillList.Add(WuSheng
            .AddTimeTrigger(
            new PTime[] {
                PPeriod.FirstFreeTime.During,
                PPeriod.SecondFreeTime.During
            },
            (PTime Time, PPlayer Player, PSkill Skill) => {
                return new PTrigger(WuSheng.Name) {
                    IsLocked = false,
                    Player = Player,
                    Time = Time,
                    AIPriority = 190,
                    CanRepeat = true,
                    Condition = (PGame Game) => {
                        return Player.Equals(Game.NowPlayer) &&
                        (Player.IsAI || Game.Logic.WaitingForEndFreeTime()) &&
                        (Player.Area.HandCardArea.CardList.Exists((PCard Card) => Card.Point % 2 == 0) ||
                        Player.Area.EquipmentCardArea.CardList.Exists((PCard Card) => Card.Point % 2 == 0));
                    },
                    AICondition = (PGame Game) => {
                        int MaxHouseValue = PMath.Max(Game.Teammates(Player), (PPlayer _Player) => {
                            return PAiMapAnalyzer.MaxValueHouse(Game, _Player, true).Value;
                        }).Value;
                        return PAiCardExpectation.FindLeastValuable(Game, Player, Player, true, true, false, true, (PCard Card) => Card.Point % 2 == 0).Value < MaxHouseValue - 500 && P_ShuShangKaaiHua.AIEmitTarget(Game, Player) != null;
                    },
                    Effect = (PGame Game) => {
                        WuSheng.AnnouceUseSkill(Player);
                        PCard TargetCard = null;
                        if (Player.IsAI) {
                            TargetCard = PAiCardExpectation.FindLeastValuable(Game, Player, Player, true, true, false, true, (PCard Card) => Card.Point % 2 == 0).Key;
                        } else {
                            List<PCard> Waiting = Player.Area.HandCardArea.CardList.FindAll((PCard Card) => Card.Point % 2 == 0);
                            List<PCard> WaitingEquipments = Player.Area.EquipmentCardArea.CardList.FindAll((PCard Card) => Card.Point % 2 == 0);
                            int Result = PNetworkManager.NetworkServer.ChooseManager.Ask(Player, WuSheng.Name, Waiting.ConvertAll((PCard Card) => Card.Name).Concat(WaitingEquipments.ConvertAll((PCard Card) => Card.Name + "(已装备)")).Concat(new List<string> { "取消" }).ToArray());
                            if (Result >= 0 && Result < Waiting.Count) {
                                TargetCard = Waiting[Result];
                            }
                        }
                        if (TargetCard != null) {
                            TargetCard.Model = new P_ShuShangKaaiHua();
                            PTrigger Trigger = TargetCard.Model.MoveInHandTriggerList.Find((Func<PPlayer, PCard, PTrigger> TriggerGenerator) => TriggerGenerator(Player, TargetCard).Time.Equals(PPeriod.FirstFreeTime.During))?.Invoke(Player, TargetCard);
                            if (Trigger != null) {
                                Game.Logic.StartSettle(new PSettle("武圣[树上开花]", Trigger.Effect));
                            }
                        }
                    }
                };
            }));
        PSkill Nuvj = new PSkill("怒斩") {
            Lock = true
        };
        SkillList.Add(Nuvj
            .AddTrigger(
            (PPlayer Player, PSkill Skill) => {
                return new PTrigger(Nuvj.Name) {
                    IsLocked = true,
                    Player = Player,
                    Time = PTime.Injure.EmitInjure,
                    AIPriority = 200,
                    Condition = (PGame Game) => {
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        return InjureTag.Injure > 0 && InjureTag.ToPlayer != null &&
                        Player.Equals(InjureTag.FromPlayer) &&
                        Player.Area.EquipmentCardArea.CardNumber > 
                        InjureTag.ToPlayer.Area.EquipmentCardArea.CardNumber && InjureTag.InjureSource is PBlock;
                    },
                    Effect = (PGame Game) => {
                        Nuvj.AnnouceUseSkill(Player);
                        Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName).Injure += 600;
                    }
                };
            }));
    }

}