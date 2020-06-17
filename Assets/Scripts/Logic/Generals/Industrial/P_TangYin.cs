using System;
using System.Linq;
using System.Collections.Generic;

public class P_TangYin: PGeneral {

    public static KeyValuePair<int, int> LangZiBannedNumber(PGame Game, PPlayer Player) {
        int Original = PAiMapAnalyzer.StartFromExpect(Game, Player, Player.Position);
        int Answer = 0;
        int AnswerValue = 0;
        for (int i = 1; i < 6; ++i) {
            int New = PAiMapAnalyzer.StartFromExpect(Game, Player, Player.Position, i);
            if (New - Original > AnswerValue) {
                Answer = i;
                AnswerValue = New - Original;
            }
        }
        return new KeyValuePair<int, int>(Answer, AnswerValue);
    }

    public P_TangYin() : base("唐寅") {
        Sex = PSex.Male;
        Age = PAge.Industrial;
        Index = 20;
        Cost = 25;
        Tips = "定位：攻击\n" +
            "难度：简单\n" +
            "史实：明代画家、书法家、诗人，“明四家”和“吴中四才子”之一。以风流之名传于世，后代有“唐伯虎点秋香”等传说。\n" +
            "攻略：\n唐寅是一个上手简单的攻击型武将，并且具有一定的防御能力。【风流】作为主要输出技能，能够保持前期的经济压制和后期的装备压制，并可通过伤害类计策触发，因此【浑水摸鱼】【关门捉贼】等牌都是关键牌。【浪子】作为防御技能，能够在后期积累了一定装备的基础上避开关键区域，防止崩盘，因此唐寅对装备牌的需求也较大。唐寅可通过对队友造成伤害以获得装备；反之，【风流】的输出也会受到装备的限制，对手可以通过给唐寅没用的装备，替换唐寅原本的装备，降低【风流】的收益甚至令其变为负收益。";

        PSkill LangZi = new PSkill("浪子");
        SkillList.Add(LangZi
            .AddTimeTrigger(
            new PTime[] {
                PPeriod.DiceStage.Start
            },
            (PTime Time, PPlayer Player, PSkill Skill) => {
                return new PTrigger(LangZi.Name) {
                    IsLocked = false,
                    Player = Player,
                    Time = Time,
                    AIPriority = 10,
                    Condition = (PGame Game) => {
                        return Player.Equals(Game.NowPlayer) &&
                        Player.HasEquipInArea();
                    },
                    AICondition = (PGame Game) => {
                        KeyValuePair<PCard, int> CardValue = PAiCardExpectation.EquipToThrow(Game, Player);
                        KeyValuePair<int, int> SkillValue = LangZiBannedNumber(Game, Player);
                        return CardValue.Key != null && SkillValue.Key > 0 && SkillValue.Value >= 100 && SkillValue.Value + CardValue.Value >= 300;
                    },
                    Effect = (PGame Game) => {
                        LangZi.AnnouceUseSkill(Player);
                        PCard TargetCard = null;
                        if (Player.IsAI) {
                            TargetCard = PAiCardExpectation.EquipToThrow(Game, Player).Key;
                        } else {
                            do {
                                TargetCard = PNetworkManager.NetworkServer.ChooseManager.AskToChooseOwnCard(Player, LangZi.Name + "[选择一张装备牌]", true, true);
                            } while (!TargetCard.Type.IsEquipment());
                        }
                        if (TargetCard != null) {
                            int BannedNumber = 0;
                            if (Player.IsAI) {
                                BannedNumber = LangZiBannedNumber(Game, Player).Key;
                            } else {
                                BannedNumber = PNetworkManager.NetworkServer.ChooseManager.Ask1To6(Player, LangZi.Name + "[选择不会被掷出的数字]");
                            }
                            if (BannedNumber > 0) {
                                Game.CardManager.MoveCard(TargetCard, Player.Area.HandCardArea.CardList.Contains(TargetCard) ? Player.Area.HandCardArea : Player.Area.EquipmentCardArea, Game.CardManager.ThrownCardHeap);
                                Player.Tags.CreateTag(new PNumberedTag(LangZi.Name, BannedNumber));
                            }
                        }
                    }
                };
            })
            .AddTrigger((PPlayer Player, PSkill Skill) => {
                return new PTrigger(LangZi.Name + "[掷骰无效触发]") {
                    IsLocked = true,
                    Player = Player,
                    Time = PPeriod.DiceStage.During,
                    Condition = (PGame Game) => {
                        return Player.Equals(Game.NowPlayer) && Player.Tags.ExistTag(LangZi.Name);
                    },
                    Effect = (PGame Game) => {
                        int BannedNumber = Player.Tags.PopTag<PNumberedTag>(LangZi.Name).Value;
                        PDiceResultTag DiceResult = Game.TagManager.FindPeekTag<PDiceResultTag>(PDiceResultTag.TagName);
                        if (BannedNumber == DiceResult.DiceResult) {
                            LangZi.AnnouceUseSkill(Player);
                            int NewNumber = BannedNumber;
                            while (NewNumber == BannedNumber) {
                                NewNumber = PMath.RandInt(1, 6);
                            }
                            PNetworkManager.NetworkServer.TellClients(new PShowInformationOrder("掷骰结果更改为" + NewNumber.ToString()));
                            DiceResult.DiceResult = NewNumber;
                        }
                    }
                };
            })
        );
        PSkill FengLiu = new PSkill("风流");
        const int FengLiuInjure = 800;
        SkillList.Add(FengLiu
            .AddTimeTrigger(
            new PTime[] {
                PTime.Injure.EmitInjure
            },
            (PTime Time, PPlayer Player, PSkill Skill) => {
                return new PTrigger(FengLiu.Name) {
                    IsLocked = false,
                    Player = Player,
                    Time = Time,
                    AIPriority = 150,
                    Condition = (PGame Game) => {
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        return InjureTag.Injure > 0 && Player.Equals(InjureTag.FromPlayer) && InjureTag.ToPlayer != null && !Player.Equals(InjureTag.ToPlayer);
                    },
                    AICondition = (PGame Game) => {
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        PPlayer ToPlayer = InjureTag.ToPlayer;
                        if (ToPlayer.TeamIndex == Player.TeamIndex) {
                            if (ToPlayer.Money > FengLiuInjure + InjureTag.Injure && ToPlayer.Money > Player.Money) {
                                return true;
                            } else if (ToPlayer.Money <= InjureTag.Injure) {
                                return true;
                            } else if (ToPlayer.Area.EquipmentCardArea.CardNumber == 0) {
                                return false;
                            }
                        }
                        foreach (PCardType CardType in new PCardType[] {
                            PCardType.WeaponCard, PCardType.DefensorCard, PCardType.TrafficCard
                        }) {
                            PCard CurrentCard = Player.GetEquipment(CardType);
                            PCard TestCard = ToPlayer.GetEquipment(CardType);
                            if (ToPlayer.TeamIndex == Player.TeamIndex) {
                                if (CurrentCard == null && TestCard != null && TestCard.Model.AIInEquipExpectation(Game, Player) > TestCard.Model.AIInEquipExpectation(Game, ToPlayer)) {
                                    return true;
                                }
                            } else if (CurrentCard != null && TestCard != null && CurrentCard.Model.AIInEquipExpectation(Game, Player) >= TestCard.Model.AIInEquipExpectation(Game, Player) + TestCard.Model.AIInEquipExpectation(Game, ToPlayer)) {
                                return false;
                            }
                        }
                        return ToPlayer.TeamIndex != Player.TeamIndex;
                    },
                    Effect = (PGame Game) => {
                        FengLiu.AnnouceUseSkill(Player);
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        PPlayer ToPlayer = InjureTag.ToPlayer;
                        int Answer = 0;
                        PCard TargetCard = null;
                        if (ToPlayer.Area.EquipmentCardArea.CardNumber == 0) {
                            Answer = 1;
                        } else {
                            if (ToPlayer.IsAI) {
                                if (ToPlayer.TeamIndex == Player.TeamIndex) {
                                    if (ToPlayer.Money <= InjureTag.Injure) {
                                        Answer = 1;
                                    } else if (PAiCardExpectation.FindMostValuable(Game, Player, ToPlayer, false, true, false, true).Value > 0) {
                                        Answer = 1;
                                    }
                                } else {
                                    int Value = FengLiuInjure * 2;
                                    if (ToPlayer.Money <= InjureTag.Injure) {
                                        Value -= FengLiuInjure;
                                    } else if (ToPlayer.Money <= InjureTag.Injure + FengLiuInjure) {
                                        Value += 30000;
                                    }
                                    
                                    foreach (PCard TestCard in ToPlayer.Area.EquipmentCardArea.CardList) {
                                        int NowValue = TestCard.Model.AIInEquipExpectation(Game, ToPlayer);
                                        int GiveValue = TestCard.Model.AIInEquipExpectation(Game, Player);
                                        int OverrideValue = 0;
                                        if (Player.GetEquipment(TestCard.Type) != null) {
                                            OverrideValue = Player.GetEquipment(TestCard.Type).Model.AIInEquipExpectation(Game, Player);
                                        }
                                        int ExtraValue = ToPlayer.General is P_HuaMulan ? 2000 : 0;
                                        if (Value > NowValue + GiveValue - OverrideValue - ExtraValue) {
                                            Value = NowValue + GiveValue - OverrideValue - ExtraValue;
                                            TargetCard = TestCard;
                                        }
                                    }
                                    if (TargetCard == null) {
                                        Answer = 1;
                                    }
                                }
                            } else {
                                Answer = PNetworkManager.NetworkServer.ChooseManager.Ask(ToPlayer, FengLiu.Name, new string[] { "交给" + Player.Name + "一件装备", "受到的伤害+" + FengLiuInjure.ToString() });
                            }
                        }
                        if (Answer == 0) {
                            Game.GiveCardTo(ToPlayer, Player, false, true, false, true);
                        } else {
                            InjureTag.Injure += FengLiuInjure;
                        }
                    }
                };
            })
        );
    }
}