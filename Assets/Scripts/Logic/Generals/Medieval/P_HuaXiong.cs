using System;
using System.Collections.Generic;

public class P_HuaXiong : PGeneral {

    public P_HuaXiong() : base("华雄") {
        Sex = PSex.Male;
        Age = PAge.Medieval;
        Index = 10;
        Cost = 20;
        Tips = "定位：辅助\n" +
            "难度：中等\n" +
            "史实：汉末董卓帐下都督，在《三国演义》中，华雄是一员猛将，曾击败孙坚，斩杀祖茂、俞涉、潘凤。\n" +
            "攻略：\n" +
            "1.华雄的资本是初始比其他人多出的15000资金，这让华雄成为了前期被擒贼擒王围堵的对象，降低了华雄的预期土地水平。因此耀武的实际收益小于15000。\n" +
            "2.耀武的收益是一次性的。和持续性收益的武将相比，节奏越快的战斗，华雄的优势越大。加快游戏节奏的诸葛连弩、草木皆兵，甚至闪电，都可以帮助扩大华雄的相对收益。\n" +
            "3.叫阵是一个期望收益为负的技能，除非有希望斩杀或者有不需要的装备，不应该随意发动叫阵。\n" +
            "4.叫阵是一个直击技能。如果存储一些装备牌，就可以在敌人现金较少的时候连续叫阵实现斩杀。因此，手牌数多的华雄可以限制敌人将现金转化成房屋等其他战斗资源的决策。\n" +
            "5.在团队里华雄主要充当一个奶妈角色，用其天然的高现金数，让队友收取过路费。同时华雄凭借叫阵也能承担一个直击斩杀手的功能。\n" +
            "6.到游戏后期，因为耀武的均回合收益已经下降到很低，华雄能为团队做出的贡献很少。";

        PSkill YaoWu = new PSkill("耀武") {
            Lock = true
        };
        SkillList.Add(YaoWu
            .AddTrigger(
            (PPlayer Player, PSkill Skill) => {
                return new PTrigger(YaoWu.Name) {
                    IsLocked = true,
                    Player = Player,
                    Time = PTime.StartGameTime,
                    AIPriority = 100,
                    Effect = (PGame Game) => {
                        YaoWu.AnnouceUseSkill(Player);
                        Player.Money = PMath.Percent(Player.Money, 150);
                        PNetworkManager.NetworkServer.TellClients(new PRefreshMoneyOrder(Player));
                    }
                };
            }));

        PSkill JiaoZhen = new PSkill("叫阵") {
            Initiative = true
        };

        SkillList.Add(JiaoZhen
            .AddTimeTrigger(
            new PTime[] {
                PPeriod.FirstFreeTime.During,
                PPeriod.SecondFreeTime.During
            },
            (PTime Time, PPlayer Player, PSkill Skill) => {
                return new PTrigger(JiaoZhen.Name) {
                    IsLocked = false,
                    Player = Player,
                    Time = Time,
                    AIPriority = 200,
                    CanRepeat = true,
                    Condition = (PGame Game) => {
                        return Player.Equals(Game.NowPlayer) && (Player.IsAI || Game.Logic.WaitingForEndFreeTime()) && Player.Area.EquipmentCardArea.CardNumber > 0;
                    },
                    AICondition = (PGame Game) => {
                        if (PAiTargetChooser.InjureTarget(Game, Player, Player, (PGame _Game, PPlayer _Player) => {
                            return _Player.IsAlive && !_Player.Equals(Player) && !(_Player.General is P_LiuJi);
                        }, 1000, JiaoZhen) == null || Player.Money <= 2000) {
                            return false;
                        }
                        if (Game.Enemies(Player).Exists((PPlayer _Player) => _Player.Money <= 2000)) {
                            return true;
                        }
                        foreach (PCardType CardType in new PCardType[] {
                            PCardType.WeaponCard, PCardType.DefensorCard, PCardType.TrafficCard
                        }) {
                            KeyValuePair<PCard, int> MaxCard = PMath.Max(
                                Player.Area.HandCardArea.CardList.FindAll((PCard _Card) =>
                                _Card.Type.Equals(CardType)),
                                (PCard _Card) => _Card.Model.AIInEquipExpectation(Game, Player));
                            PCard CurrentCard = Player.GetEquipment(CardType);
                            if (CurrentCard != null) {
                                int Expect = CurrentCard.Model.AIInEquipExpectation(Game, Player);
                                if (MaxCard.Value > Expect) {
                                    return true;
                                }
                                if (Expect <= 1000) {
                                    return true;
                                }
                            }
                        }
                        return false;
                    },
                    Effect = (PGame Game) => {
                        JiaoZhen.AnnouceUseSkill(Player);
                        PPlayer Target = null;
                        if (Player.IsAI) {
                            Target = PAiTargetChooser.InjureTarget(Game, Player, Player, (PGame _Game, PPlayer _Player) => {
                                return _Player.IsAlive && !_Player.Equals(Player) && !(_Player.General is P_LiuJi);
                            }, 1000, JiaoZhen);
                        } else {
                            Target = PNetworkManager.NetworkServer.ChooseManager.AskForTargetPlayer(Player, PTrigger.Except(Player), JiaoZhen.Name, true);
                        }
                        if (Target != null) {
                            if (Game.PkPoint(Player, Target) > 0) {
                                Game.Injure(Player, Target, 1000, JiaoZhen);
                            } else {
                                Game.ThrowCard(Player, Player, false);
                                Game.LoseMoney(Player, 1000);
                            }
                        }
                    }
                };
            }));
    }

}