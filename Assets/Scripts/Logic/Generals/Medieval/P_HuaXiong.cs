using System;
using System.Collections.Generic;

public class P_HuaXiong : PGeneral {


    public P_HuaXiong() : base("华雄") {
        Sex = PSex.Male;
        Age = PAge.Medieval;
        Index = 10;
        Cost = 20;
        Tips = "定位：攻防兼备\n" +
            "难度：中等\n" +
            "史实：汉末董卓帐下都督，在《三国演义》中，华雄是一员猛将，曾击败孙坚，斩杀祖茂、俞涉、潘凤。\n" +
            "攻略：\n华雄拥有非常多的起始资金，使得其很不容易被速推，一般的武将要在20至30个回合之后，技能收益才能达到华雄在游戏开始的收益。\n【叫阵】可以消耗没用的装备，但拼点赢得概率5 / 12相对比较低，总体来说收益期望有限，稍微有用一点的装备都不宜拿来【叫阵】。同时，华雄只能用装备上的牌【叫阵】，当一身神装时，华雄身上的牌舍不得用，手里的牌发不了，容易进退两难。\n如果战线拖得很长，则对华雄不利。技能发动次数随时间线性增长的武将，如陈圆圆、杨玉环，会对华雄实现累积优势。";

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
                        if (PAiTargetChooser.InjureTarget(Game, Player, Player, PTrigger.Except(Player), 1000, JiaoZhen) == null) {
                            return false;
                        }
                        if (Game.Enemies(Player).Exists((PPlayer _Player) => _Player.Money <= 1000)) {
                            return true;
                        }
                        foreach (PCardType CardType in new PCardType[] {
                            PCardType.WeaponCard, PCardType.DefensorCard, PCardType.TrafficCard
                        }) {
                            KeyValuePair<PCard, int> MaxCard = PMath.Max(Player.Area.HandCardArea.CardList, (PCard _Card) => _Card.Model.AIInEquipExpectation(Game, Player));
                            PCard CurrentCard = Player.GetEquipment(CardType);
                            if (CurrentCard != null && MaxCard.Value > CurrentCard.Model.AIInEquipExpectation(Game, Player)) {
                                return true;
                            }
                        }
                        return false;
                    },
                    Effect = (PGame Game) => {
                        JiaoZhen.AnnouceUseSkill(Player);
                        PPlayer Target = null;
                        if (Player.IsAI) {
                            Target = PAiTargetChooser.InjureTarget(Game, Player, Player, PTrigger.Except(Player), 1000, JiaoZhen);
                        } else {
                            Target = PNetworkManager.NetworkServer.ChooseManager.AskForTargetPlayer(Player, PTrigger.Except(Player), JiaoZhen.Name, true);
                        }
                        if (Target != null) {
                            if (Game.PkPoint(Player, Target) > 0) {
                                Game.Injure(Player, Target, 1000, JiaoZhen);
                            } else {
                                Game.ThrowCard(Player, Player, false);
                            }
                        }
                    }
                };
            }));
    }

}