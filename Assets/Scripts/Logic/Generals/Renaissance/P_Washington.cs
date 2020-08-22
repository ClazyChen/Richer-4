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
            "难度：困难\n" +
            "史实：美国政治家、军事家、革命家，首任总统，美国开国元勋之一，在美国独立战争中任大陆军的总司令。华盛顿主持起草了《独立宣言》，制定美国宪法，领导创立并完善了共和政体。\n" +
            "攻略：\n华盛顿每局游戏可以发动两次无损技能，所以技能发动时机是关键。由于每名其他角色可以选择两个选项，所以华盛顿对对方只有两种可能性，一种是通过对方没有手牌来迫使对方承受高伤害，一种是通过对方有高伤害风险迫使对方承受所有手牌的损失。其中前者比较常见。虽然华盛顿的技能是全体范围，但发动的条件苛刻且转瞬即逝，通常如果能对其中一个敌人有巨大收益已经实属不易，所以第一次发动应该果断一些，不强求同时威胁多个敌人，第二次可以作为威慑适当保留。";
        NewGeneral = true;
        PSkill MinZhu = new PSkill("民主") {
            Initiative = true
        };
        const int MinZhuCof = 800;
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
                        int Sum = 0;
                        Game.AlivePlayers(Player).ForEach((PPlayer _Player) => {
                            int Cof = _Player.TeamIndex == Player.TeamIndex ? 1 : -1;
                            int Expect1 = 2000 * Cof + PAiTargetChooser.InjureExpect(Game, Player, Player, _Player, MinZhuCof * _Player.Position.HouseNumber, MinZhu);
                            int Expect2 = -2000 * _Player.Area.HandCardArea.CardNumber * Cof;
                            if (_Player.Area.HandCardArea.CardNumber >=1 && (Expect2 < Expect1 ^ Cof == 1)) {
                                Sum += Expect2;
                            } else {
                                Sum += Expect1;
                            }
                        });
                        return Sum >= 1300 * (Game.AlivePlayerNumber-1);
                    },
                    Effect = (PGame Game) => {
                        MinZhu.AnnouceUseSkill(Player);
                        Game.Traverse((PPlayer _Player) => {
                            if (!_Player.Equals(Player)) {
                                int ChosenResult = 0;
                                if (_Player.Area.HandCardArea.CardNumber >= 1) {
                                    if (_Player.IsAI) {
                                        int Expect1 = 2000 + PAiTargetChooser.InjureExpect(Game, _Player, Player, _Player, MinZhuCof * _Player.Position.HouseNumber, MinZhu);
                                        int Expect2 = 0;
                                        _Player.Area.HandCardArea.CardList.ForEach((PCard Card) => {
                                            Expect2 -= Card.Model.AIInHandExpectation(Game, _Player);
                                        });
                                        if (Expect2 >= Expect1) {
                                            ChosenResult = 1;
                                        }
                                    } else {
                                        ChosenResult = PNetworkManager.NetworkServer.ChooseManager.Ask(_Player, MinZhu.Name + "-选择一项", new string[] {
                                            "令" + Player.Name + "对你造成" + (MinZhuCof * _Player.Position.HouseNumber).ToString() + "点伤害，摸一张牌",
                                            "弃置所有手牌"
                                        });
                                    }
                                }
                                if (ChosenResult == 0) {
                                    Game.Injure(Player, _Player, MinZhuCof * _Player.Position.HouseNumber, MinZhu);
                                    if (_Player.IsAlive) {
                                        Game.GetCard(_Player);
                                    }
                                } else {
                                    for (int i = _Player.Area.HandCardArea.CardNumber - 1; i >= 0; --i) {
                                        Game.CardManager.MoveCard(_Player.Area.HandCardArea.CardList[i], _Player.Area.HandCardArea, Game.CardManager.ThrownCardHeap);
                                    }
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