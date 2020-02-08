using System;
using System.Collections.Generic;

public class P_ZhouYu: PGeneral {

    public P_ZhouYu() : base("周瑜") {
        Sex = PSex.Male;
        Age = PAge.Medieval;
        Index = 22;
        Cost = 25;
        Tips = "定位：全能\n" +
            "难度：困难\n" +
            "史实：东汉末期东吴名将，“东吴四都督”之一。在赤壁之战中以少胜多，大破曹军，奠定了“三分天下”的基础。\n" +
            "攻略：\n周瑜是一名集续航、辅助、控制于一体的武将。【英姿】作为强大的续航技能，能够大大提高自己的生存能力。【纵火】是使用周瑜的难点所在，需要较强的局势判断能力。前期【纵火】的使用往往倾向于敌方，能够显著降低其输出能力，而后期经过一定房屋的积累或【诸葛连弩】等卡牌的使用，使得对己方使用【纵火】成为了一个选择，而这时也可以对【纵火】过的敌方土地做第二次无损的【纵火】。作为团队的重要辅助位置，周瑜的技能依赖房屋而非卡牌，团队商业用地选择城堡、购物中心或公园，能获取更高的【纵火】收益。";

        PSkill YingZi = new PSkill("英姿") {
            SoftLockOpen = true
        };
        SkillList.Add(YingZi
            .AddTimeTrigger(
            new PTime[] {
                PPeriod.StartTurn.During
            },
            (PTime Time, PPlayer Player, PSkill Skill) => {
                return new PTrigger(YingZi.Name) {
                    IsLocked = false,
                    Player = Player,
                    Time = Time,
                    AIPriority = 100,
                    Condition = (PGame Game) => {
                        return Player.Equals(Game.NowPlayer);
                    },
                    Effect = (PGame Game) => {
                        YingZi.AnnouceUseSkill(Player);
                        Game.GetMoney(Player, 200);
                    }
                };
            }));
        PSkill ZongHuo = new PSkill("纵火") {
            Initiative = true
        };
        SkillList.Add(ZongHuo
            .AnnouceTurnOnce()
            .AddTimeTrigger(
            new PTime[] {
                PPeriod.FirstFreeTime.During,
                PPeriod.SecondFreeTime.During
            },
            (PTime Time, PPlayer Player, PSkill Skill) => {
                return new PTrigger(ZongHuo.Name) {
                    IsLocked = false,
                    Player = Player,
                    Time = Time,
                    AIPriority = 180,
                    CanRepeat = true,
                    Condition = (PGame Game) => {
                        return Player.Equals(Game.NowPlayer) && (Player.IsAI || Game.Logic.WaitingForEndFreeTime()) && Player.RemainLimit(ZongHuo.Name) && Player.Position.HouseNumber > 0 && Player.Position.Lord != null;
                    },
                    AICondition = (PGame Game) => {
                        int CurrentToll = Player.Position.Toll;
                        int NewToll = PMath.Percent(Player.Position.Price + PMath.Percent(Player.Position.Price, 10), 20 + 40 * (Player.Position.HouseNumber - 1)) * (Player.Position.BusinessType.Equals(PBusinessType.ShoppingCenter) ? 2 : 1);
                        int Value = NewToll - CurrentToll;
                        if (Player.TeamIndex == Player.Position.Lord.TeamIndex) {
                            return Player.Position.Price == PMath.Percent(Player.Position.Price, 100) && -Value <= Player.Position.Lord.Money / 10;
                        } else {
                            return Value < 0;
                        }
                    },
                    Effect = (PGame Game) => {
                        ZongHuo.AnnouceUseSkill(Player);
                        Game.LoseHouse(Player.Position, 1);
                        Player.Position.Price += PMath.Percent(Player.Position.Price, 10);
                        PNetworkManager.NetworkServer.TellClients(new PRefreshBlockBasicOrder(Player.Position));
                        Player.Tags.FindPeekTag<PUsedTag>(PUsedTag.TagNamePrefix + ZongHuo.Name).Count++;
                    }
                };
            }));
    }

}