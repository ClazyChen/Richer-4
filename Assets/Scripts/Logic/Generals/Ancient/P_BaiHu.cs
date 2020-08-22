using System;
using System.Collections.Generic;

public class P_BaiHu : PGeneral
{

    public P_BaiHu() : base("白虎") {
        Sex = PSex.Male;
        Age = PAge.Ancient;
        Index = 1002;
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

        PSkill BaiHu = new PSkill("白虎") {
            Lock = true
        };
        SkillList.Add(BaiHu
            .AddTrigger(
            (PPlayer Player, PSkill Skill) => {
                return new PTrigger(BaiHu.Name) {
                    IsLocked = true,
                    Player = Player,
                    Time = PPeriod.EndTurn.During,
                    AIPriority = 250,
                    Condition = (PGame Game) => {
                        PChiaTaoFaKuoTag ChiaTaoFaKuoTag = Game.TagManager.FindPeekTag<PChiaTaoFaKuoTag>(PChiaTaoFaKuoTag.TagName);
                        return Player.Equals(Game.NowPlayer) && ChiaTaoFaKuoTag.LordList.Count >= 1;
                    },
                    Effect = (PGame Game) => {
                        BaiHu.AnnouceUseSkill(Player);
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        PChiaTaoFaKuoTag ChiaTaoFaKuoTag = Game.TagManager.FindPeekTag<PChiaTaoFaKuoTag>(PChiaTaoFaKuoTag.TagName);
                        ChiaTaoFaKuoTag.LordList.ForEach((PPlayer _Player) => {
                            if (!_Player.Tags.ExistTag(PTag.LockedTag.Name)) {
                                _Player.Tags.CreateTag(PTag.LockedTag);
                            }
                        });
                    }
                };
            }));
    }

}