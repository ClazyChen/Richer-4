using System;
using System.Linq;
using System.Collections.Generic;

public class P_WangXu : PGeneral {


    public P_WangXu() : base("王诩") {
        Sex = PSex.Male;
        Age = PAge.Classic;
        Index = 5;
        Tips = "定位：全能\n" +
            "难度：困难\n" +
            "史实：战国时期思想家、纵横家，号“鬼谷子”。\n" +
            "攻略：\n王诩具有两个技能，在免费武将里是一个较为难用的武将，建议对游戏有一定了解之后使用。\n【纵横】可以将牌转化成【远交近攻】，但3和6两个点数里有不少高价值的牌，如【借尸还魂】、【欲擒故纵】、【抛砖引玉】、【走为上计】等，是否将这些牌转化成显性1000收益、隐性收益视队友状况而定的【远交近攻】，需要结合场上形势进行把握。\n【隐居】提高了王诩的生存能力，但【隐居】本身是负收益技能，移出游戏的王诩失去了收取过路费的能力，还会亏一座房屋，如果每回合都使用，必定只能苟延残喘片刻。所以，【隐居】的目的地需要给自己留出几个回合较为安全的空间，让自己在【隐居】的间隙可以造成输出。\n对于新手而言，也可以选择无脑【纵横】给大神队友补牌，后期保持【隐居】牵制敌人的打法。";

        PSkill ZongHeng = new PSkill("纵横") {
            Initiative = true
        };
        SkillList.Add(ZongHeng
            .AddTimeTrigger(
            new PTime[] {
                PPeriod.FirstFreeTime.During,
                PPeriod.SecondFreeTime.During
            },
            (PTime Time, PPlayer Player, PSkill Skill) => {
                return new PTrigger(ZongHeng.Name) {
                    IsLocked = false,
                    Player = Player,
                    Time = Time,
                    AIPriority = 200,
                    CanRepeat = true,
                    Condition = (PGame Game) => {
                        return Player.Equals(Game.NowPlayer) &&
                        (Player.IsAI || Game.Logic.WaitingForEndFreeTime()) &&
                        Player.Area.HandCardArea.CardList.Exists((PCard Card) => Card.Point %3 == 0);
                    },
                    AICondition = (PGame Game) => {
                        return Player.Area.HandCardArea.CardList.Exists((PCard Card) => {
                            return Card.Point %3 == 0 &&  Card.Model.AIInHandExpectation(Game, Player) < 3000;
                        }) && P_YooenChiaoChinKung.AIEmitTargets(Game, Player)[0] != null;
                    },
                    Effect = (PGame Game) => {
                        ZongHeng.AnnouceUseSkill(Player);
                        PCard TargetCard = null;
                        if (Player.IsAI) {
                            TargetCard = PAiCardExpectation.FindLeastValuable(Game, Player, Player, true, false, false, true, (PCard Card) => Card.Point % 3 == 0).Key;
                        } else {
                            List<PCard> Waiting = Player.Area.HandCardArea.CardList.FindAll((PCard Card) => Card.Point % 3 == 0);
                            int Result = PNetworkManager.NetworkServer.ChooseManager.Ask(Player, ZongHeng.Name, Waiting.ConvertAll((PCard Card) => Card.Name).Concat(new List<string> { "取消" }).ToArray());
                            if (Result >= 0 && Result < Waiting.Count) {
                                TargetCard = Waiting[Result];
                            }
                        }
                        if (TargetCard != null) {
                            TargetCard.Model = new P_YooenChiaoChinKung();
                            PTrigger Trigger = TargetCard.Model.MoveInHandTriggerList.Find((Func<PPlayer, PCard, PTrigger> TriggerGenerator) => TriggerGenerator(Player, TargetCard).Time.Equals(PPeriod.FirstFreeTime.During))?.Invoke(Player, TargetCard);
                            if (Trigger != null) {
                                Game.Logic.StartSettle(new PSettle("纵横[远交近攻]", Trigger.Effect));
                            }
                        }
                    }
                };
            }));
        PSkill YinJu = new PSkill("隐居") {
            Initiative = true
        };
        SkillList.Add(YinJu
            .AnnouceTurnOnce()
            .AddTimeTrigger(
            new PTime[] {
                PPeriod.FirstFreeTime.During,
                PPeriod.SecondFreeTime.During
            },
            (PTime Time, PPlayer Player, PSkill Skill) => {
                return new PTrigger(YinJu.Name) {
                    IsLocked = false,
                    Player = Player,
                    Time = Time,
                    AIPriority = 10,
                    CanRepeat = true,
                    Condition = (PGame Game) => {
                        return Player.Equals(Game.NowPlayer) && (Player.IsAI || Game.Logic.WaitingForEndFreeTime()) && Player.RemainLimit(YinJu.Name) && Player.HasHouse;
                    },
                    AICondition = (PGame Game) => {
                        if (Game.NowPeriod.Equals(PPeriod.FirstFreeTime)) {
                            bool CanGo = false;
                            PAiMapAnalyzer.NextBlocks(Game, Player).ForEach((PBlock Block) => {
                                if (Block.Lord != null && Block.Lord.TeamIndex != Player.TeamIndex && Block.Toll >= Player.Money) {
                                    CanGo = true;
                                }
                                if (Block.GetMoneyStopSolid < 0 && -Block.GetMoneyStopSolid >= Player.Money) {
                                    CanGo = true;
                                }
                                if (Block.GetMoneyStopPercent < 0 && PMath.Percent(Player.Money, -Block.GetMoneyStopPercent) >= Player.Money) {
                                    CanGo = true;
                                }
                            });
                            return CanGo || PAiMapAnalyzer.OutOfGameExpect(Game, Player, true) - PAiMapAnalyzer.MinValueHouse(Game, Player).Value > 0;
                        } else {
                            return false;
                        }
                    },
                    Effect = (PGame Game) => {
                        YinJu.AnnouceUseSkill(Player);
                        PBlock Block = null;
                        if (Player.IsAI) {
                            Block = PAiMapAnalyzer.MinValueHouse(Game, Player).Key;
                        } else {
                            Block = PNetworkManager.NetworkServer.ChooseManager.AskToChooseBlock(Player, YinJu.Name, (PBlock _Block) => {
                                return Player.Equals(_Block.Lord) && _Block.HouseNumber > 0;
                            });
                        }
                        if (Block != null) {
                            Game.MovePosition(Player, Player.Position, Block);
                            PNetworkManager.NetworkServer.TellClients(new PHighlightBlockOrder(Block.Index.ToString()));
                            Game.LoseHouse(Block, 1);
                            Player.Tags.CreateTag(PTag.OutOfGameTag);
                            Player.Tags.FindPeekTag<PUsedTag>(PUsedTag.TagNamePrefix + YinJu.Name).Count++;
                        }
                    }
                };
            }));
    }

}