using System;
using System.Collections.Generic;

public class P_LianPo: PGeneral {


    public P_LianPo() : base("廉颇") {
        Sex = PSex.Male;
        Age = PAge.Classic;
        Index = 1;
        Tips = "定位：辅助\n" +
            "难度：中等\n" +
            "史实：战国时期赵国名将，被誉为“战国四大名将”之一，曾击退燕国入侵，斩杀栗腹。\n" +
            "攻略：\n廉颇可以把自己的现金转移给队友，同时让队友摸牌。和王诩、虞姬等技能依赖于手牌的武将组队时，廉颇是一个不错的选择，提高队友的技能发动频率。\n" +
            "新人推荐使用廉颇，给大神队友更多的牌，让队友有更大的发挥空间。\n" +
            "因为廉颇的现金可以给队友提供每回合2000（一张牌）的收益，廉颇很容易成为敌人的集火对象，在这种情况下，可以减少房屋的建造（除购物中心外，建房收益均不及负荆收益），让自己的现金发挥更大的价值。";

        PSkill FuJing = new PSkill("负荆") {
            Initiative = true
        };
        SkillList.Add(FuJing
            .AnnounceTurnOnce()
            .AddTimeTrigger(
            new PTime[] {
                PPeriod.FirstFreeTime.During,
                PPeriod.SecondFreeTime.During
            },
            (PTime Time, PPlayer Player, PSkill Skill) => {
                return new PTrigger(FuJing.Name) {
                    IsLocked = false,
                    Player = Player,
                    Time = Time,
                    AIPriority = 180,
                    CanRepeat = true,
                    Condition = (PGame Game) => {
                        return Player.Equals(Game.NowPlayer) && (Player.IsAI || Game.Logic.WaitingForEndFreeTime()) && Player.RemainLimit(FuJing.Name) ;
                    },
                    AICondition = (PGame Game) => {
                        if (Game.Teammates(Player, false).Count == 0 || Player.Money < 6000) {
                            return false;
                        }
                        if (Player.Money >= 15000 || !Player.CanBeInjured) {
                            return true;
                        } else {
                            bool CanGo = true;
                            PAiMapAnalyzer.NextBlocks(Game, Player).ForEach((PBlock Block) => {
                                if (Block.Lord == null && Block.CanPurchase && Block.Price >= Player.Money - 8000) {
                                    CanGo = false;
                                }
                                if (Block.Lord != null && Block.Lord.TeamIndex != Player.TeamIndex && Block.Toll >= Player.Money - 8000) {
                                    CanGo = false;
                                }
                            });
                            return CanGo;
                        }
                    },
                    Effect = (PGame Game) => {
                        FuJing.AnnouceUseSkill(Player);
                        PPlayer Target = null;
                        if (Player.IsAI) {
                            Target = PMath.Max(Game.Teammates(Player, false), (PPlayer _Player) => _Player.AiCardExpectation + PMath.RandInt(0,5) + ((Player.Defensor != null && Player.Defensor.Model is P_PaiHuaChooon && !Player.Sex.Equals(_Player.Sex)) ? 1500 : 0 )).Key;
                        } else {
                            Target = PNetworkManager.NetworkServer.ChooseManager.AskForTargetPlayer(Player, PTrigger.Except(Player), FuJing.Name, true);
                        }
                        if (Target != null) {
                            Game.Injure(Target, Player, 3000, FuJing);
                            Game.GetCard(Target, 1);
                            FuJing.DeclareUse(Player);
                        }
                    }
                };
            }));
    }

}