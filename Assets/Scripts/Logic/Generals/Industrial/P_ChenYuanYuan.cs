using System;
using System.Collections.Generic;

public class P_ChenYuanYuan: PGeneral {

    public P_ChenYuanYuan() : base("陈圆圆") {
        Sex = PSex.Female;
        Age = PAge.Industrial;
        Index = 4;
        Tips = "定位：防御\n" +
            "难度：简单\n" +
            "史实：明末清初戏曲演艺家，作为吴三桂向农民军的宣战理由而闻名于世。\n" +
            "攻略：\n陈圆圆是新手推荐的武将，使用简单，且具有一定的强度，过牌量较高，可以用来练习对牌的使用能力。\n【风云】的收益取决于在场上的存在感，而【楚楚】平均6回合摸一张牌，因此在人数少、回合频繁的1v1模式中，陈圆圆的强度非常高。但陈圆圆和队友的配合能力相对较差，在人数多、回合被稀释的4v4模式下，表现就不是很好了。";

        PSkill FengYun = new PSkill("风云") {
            SoftLockOpen = true
        };
        SkillList.Add(FengYun
             .AddTrigger(
            (PPlayer Player, PSkill Skill) => {
                return new PTrigger(FengYun.Name) {
                    IsLocked = false,
                    Player = Player,
                    Time = PTime.Injure.EndSettle,
                    AIPriority = 120,
                    Condition = (PGame Game) => {
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        return (Player.Equals(InjureTag.ToPlayer) || Player.Equals(InjureTag.FromPlayer)) && InjureTag.Injure > 0;
                    },
                    Effect = (PGame Game) => {
                        FengYun.AnnouceUseSkill(Player);
                        Game.GetMoney(Player, 200);
                    }
                };
            }));
        PSkill ChuChu = new PSkill("楚楚") {
            SoftLockOpen = true
        };
        SkillList.Add(ChuChu
            .AddTrigger(
            (PPlayer Player, PSkill Skill) => {
                return new PTrigger(ChuChu.Name) {
                    IsLocked = false,
                    Player = Player,
                    Time = PPeriod.EndTurnStage.During,
                    AIPriority = 50,
                    Condition = (PGame Game) => {
                        return Player.Equals(Game.NowPlayer);
                    },
                    Effect = (PGame Game) => {
                        ChuChu.AnnouceUseSkill(Player);
                        int X = Game.Judge(Player, 4);
                        if (X == 4) {
                            Game.GetCard(Player, 1);
                        }
                    }
                };
            }));
    }

}