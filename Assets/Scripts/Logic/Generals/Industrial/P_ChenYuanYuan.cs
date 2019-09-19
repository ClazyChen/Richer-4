using System;
using System.Collections.Generic;

public class P_ChenYuanYuan: PGeneral {

    public P_ChenYuanYuan() : base("陈圆圆") {
        Sex = PSex.Female;
        Age = PAge.Industrial;
        Index = 4;
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
                        int X = Game.Judge(Player);
                        if (X == 4) {
                            Game.GetCard(Player, 1);
                        }
                    }
                };
            }));
    }

}