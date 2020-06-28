using System;
using System.Collections.Generic;

public class P_ZhuQue : PGeneral {

    public P_ZhuQue() : base("朱雀") {
        Sex = PSex.Male;
        Age = PAge.Ancient;
        Index = 1003;
        Cost = 1;
        Tips = string.Empty;
        CanBeChoose = false;

        //PSkill ShenShou = new PSkill("神兽") {
        //    Lock = true
        //};
        //SkillList.Add(ShenShou
        //    .AddTrigger(
        //    (PPlayer Player, PSkill Skill) => {
        //        return new PTrigger(ShenShou.Name) {
        //            IsLocked = true,
        //            Player = Player,
        //            Time = PTime.StartGameTime,
        //            AIPriority = 100,
        //            Effect = (PGame Game) => {
        //                ShenShou.AnnouceUseSkill(Player);
        //                Game.GetMoney(Player, 30000);
        //            }
        //        };
        //    })
        //    .AddTrigger(
        //    (PPlayer Player, PSkill Skill) => {
        //        return new PTrigger(ShenShou.Name) {
        //            IsLocked = true,
        //            Player = Player,
        //            Time = PPeriod.StartTurn.During,
        //            AIPriority = 100,
        //            Condition = (PGame Game) => {
        //                return Player.Equals(Game.NowPlayer);
        //            },
        //            Effect = (PGame Game) => {
        //                ShenShou.AnnouceUseSkill(Player);
        //                Game.GetMoney(Player, 500);
        //            }
        //        };
        //    }));

        PSkill ZhuQue = new PSkill("朱雀") {
            Lock = true
        };
        SkillList.Add(ZhuQue
            .AddTrigger(
            (PPlayer Player, PSkill Skill) => {
                return new PTrigger(ZhuQue.Name) {
                    IsLocked = true,
                    Player = Player,
                    Time = PPeriod.StartTurn.During,
                    AIPriority = 250,
                    Condition = (PGame Game) => {
                        return Player.Equals(Game.NowPlayer);
                    },
                    Effect = (PGame Game) => {
                        ZhuQue.AnnouceUseSkill(Player);
                        Game.Traverse((PPlayer _Player) => {
                            if (!_Player.Equals(Player)) {
                                Game.LoseMoney(_Player, 300);
                            }
                        }, Player);
                    }
                };
            }));
    }

}