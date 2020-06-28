using System;
using System.Collections.Generic;

public class P_QingLong : PGeneral {

    public P_QingLong() : base("青龙") {
        Sex = PSex.NoSex;
        Age = PAge.Ancient;
        Index = 1001;
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

        PSkill QingLong = new PSkill("青龙") {
            Lock = true
        };
        SkillList.Add(QingLong
            .AddTrigger(
            (PPlayer Player, PSkill Skill) => {
                return new PTrigger(QingLong.Name) {
                    IsLocked = true,
                    Player = Player,
                    Time = PTime.Injure.AcceptInjure,
                    AIPriority = 250,
                    Condition = (PGame Game) => {
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        PPlayer FromPlayer = InjureTag.FromPlayer;
                        return InjureTag.Injure > 0 && Player.Equals(InjureTag.ToPlayer) && FromPlayer != null && FromPlayer.Area.OwnerCardNumber > 0;
                    },
                    Effect = (PGame Game) => {
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        PPlayer FromPlayer = InjureTag.FromPlayer;
                        QingLong.AnnouceUseSkill(Player);
                        Game.GetCardFrom(Player, FromPlayer);
                    }
                };
            }));
    }

}