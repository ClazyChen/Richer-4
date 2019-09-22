using System;
using System.Linq;
using System.Collections.Generic;

public class P_WangXu : PGeneral {


    public P_WangXu() : base("王诩") {
        Sex = PSex.Male;
        Age = PAge.Classic;
        Index = 5;
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
                            return Card.Point %3 == 0 &&  Card.Model.AIInHandExpectation(Game, Player) < 2500;
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