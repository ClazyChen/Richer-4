using System;
using System.Collections.Generic;

public class P_Washington: PGeneral {

    public class PElectronTag : PNumberedTag
    {
        public static string TagName = "选举";
        public PElectronTag(): base(TagName, 4) { }
    }

    public P_Washington() : base("华盛顿") {
        Sex = PSex.Male;
        Age = PAge.Renaissance;
        Index = 27;
        Cost = 30;
        Tips = "定位：攻击\n" +
            "难度：待定\n" +
            "史实：美国政治家、军事家、革命家，首任总统，美国开国元勋之一，在美国独立战争中任大陆军的总司令。华盛顿主持起草了《独立宣言》，制定美国宪法，领导创立并完善了共和政体。\n" +
            "攻略：\n暂无";
        NewGeneral = true;
        PSkill MinZhu = new PSkill("民主") {
            Initiative = true
        };
        SkillList.Add(MinZhu
            .AnnounceGameTimes(2)
            .AddTimeTrigger(
            new PTime[] {
                PPeriod.FirstFreeTime.During,
                PPeriod.SecondFreeTime.During
            },
            (PTime Time, PPlayer Player, PSkill Skill) => {
                return new PTrigger(MinZhu.Name) {
                    IsLocked = false,
                    Player = Player,
                    Time = Time,
                    AIPriority = 20,
                    CanRepeat = true,
                    Condition = (PGame Game) => {
                        return Player.Equals(Game.NowPlayer) && (Player.IsAI || Game.Logic.WaitingForEndFreeTime()) && Player.RemainLimit(MinZhu.Name) && !Player.Tags.ExistTag(PElectronTag.TagName);
                    },
                    AICondition = (PGame Game) => {
                        /*
                         * 民主的发动条件：
                         * 收益大于2000*敌方人数，即敌方无法通过交给手牌降低亏损，且己方可以通过交给手牌收益（翻面正收益）
                         */
                        int Sum = 0;
                        foreach (PPlayer _Player in Game.AlivePlayers(Player)) {
                            int Cof = _Player.TeamIndex == Player.TeamIndex ? 1 : -1;
                            int Expect1 = 1000 * (1 - Cof);
                            int Expect2 = 0;
                            if (_Player.HandCardNumber > 0) {
                                Expect2 = PAiMapAnalyzer.ChangeFaceExpect(Game, _Player) * Cof;
                                if (Cof == 1) {
                                    PCard Test = PAiCardExpectation.FindMostValuable(Game, Player, _Player, true, false, false, true).Key;
                                    Expect2 += Test.Model.AIInHandExpectation(Game, Player) - Test.Model.AIInHandExpectation(Game, _Player);
                                } else {
                                    PCard Test = PAiCardExpectation.FindLeastValuable(Game, Player, _Player, true, false, false, true).Key;
                                    Expect2 += Test.Model.AIInHandExpectation(Game, Player) + Test.Model.AIInHandExpectation(Game, _Player);
                                }
                            } else {
                                Expect2 = Expect1;
                            }
                            int MaxExpect = Cof == 1 ? Math.Max(Expect1, Expect2) : Math.Min(Expect1, Expect2);
                            Sum += MaxExpect;
                        }
                        return Sum >= 2000 * Game.Enemies(Player).Count;
                    },
                    Effect = (PGame Game) => {
                        MinZhu.AnnouceUseSkill(Player);
                        Game.Traverse((PPlayer _Player) => {
                            if (!_Player.Equals(Player)) {
                                int ChosenResult = 0;
                                if (_Player.HandCardNumber > 0) {
                                    if (_Player.IsAI) {
                                        int Cof = _Player.TeamIndex == Player.TeamIndex ? 1 : -1;
                                        int Expect1 = 1000 * (1 - Cof);
                                        int Expect2 = PAiMapAnalyzer.ChangeFaceExpect(Game, _Player) * Cof;
                                        if (Cof == 1) {
                                            PCard Test = PAiCardExpectation.FindMostValuable(Game, Player, _Player, true, false, false, true).Key;
                                            Expect2 += Test.Model.AIInHandExpectation(Game, Player) - Test.Model.AIInHandExpectation(Game, _Player);
                                        } else {
                                            PCard Test = PAiCardExpectation.FindLeastValuable(Game, Player, _Player, true, false, false, true).Key;
                                            Expect2 += Test.Model.AIInHandExpectation(Game, Player) + Test.Model.AIInHandExpectation(Game, _Player);
                                        }
                                        if ((Expect1 > Expect2) ^ (Cof == 1)) {
                                            ChosenResult = 1;
                                        }
                                    } else {
                                        ChosenResult = PNetworkManager.NetworkServer.ChooseManager.Ask(_Player, MinZhu.Name + "-选择一项", new string[] {
                                            "令" + Player.Name + "对你造成1000点伤害",
                                            "交给" + Player.Name + "1张手牌并翻面"
                                        });
                                    }
                                }
                                if (ChosenResult == 0) {
                                    Game.Injure(Player, _Player, 1000, MinZhu);
                                } else {
                                    Game.GiveCardTo(_Player, Player, true, false);
                                    Game.ChangeFace(_Player);
                                }
                            }
                        }, Player);
                        MinZhu.DeclareUse(Player);
                        if (Player.RemainLimit(MinZhu.Name)) {
                            Player.Tags.CreateTag(new PElectronTag());
                        }
                    }
                };
            })
            .AddTrigger((PPlayer Player, PSkill Skill) => {
                return new PTrigger(MinZhu.Name) {
                    IsLocked = true,
                    Player = Player,
                    Time = PPeriod.EndTurn.During,
                    Condition = (PGame Game) => {
                        return Player.Equals(Game.NowPlayer) && Player.Tags.ExistTag(PElectronTag.TagName);
                    },
                    Effect = (PGame Game) => {
                        if (--Player.Tags.FindPeekTag<PElectronTag>(PElectronTag.TagName).Value <= 0) {
                            Player.Tags.PopTag<PElectronTag>(PElectronTag.TagName);
                        }
                        PNetworkManager.NetworkServer.TellClients(new PRefreshMarkStringOrder(Player));
                    }
                };
            }));
    }

}