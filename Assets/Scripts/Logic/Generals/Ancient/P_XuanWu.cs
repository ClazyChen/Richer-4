using System;
using System.Collections.Generic;

public class P_XuanWu : PGeneral {

    public P_XuanWu() : base("玄武") {
        Sex = PSex.Male;
        Age = PAge.Ancient;
        Index = 1004;
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

        PSkill XuanWu = new PSkill("玄武") {
            Lock = true
        };
        SkillList.Add(XuanWu
            .AddTrigger(
            (PPlayer Player, PSkill Skill) => {
                return new PTrigger(XuanWu.Name) {
                    IsLocked = true,
                    Player = Player,
                    Time = PTime.Injure.AcceptInjure,
                    AIPriority = 250,
                    Condition = (PGame Game) => {
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        return InjureTag.Injure > 0 && Player.Equals(InjureTag.ToPlayer) && InjureTag.InjureSource is PBlock && Player.LandNumber > 0;
                    },
                    Effect = (PGame Game) => {
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        XuanWu.AnnouceUseSkill(Player);
                        int MinHouse = PMath.Min(Game.Map.FindBlock(Player), (PBlock Block) => Block.HouseNumber).Value;
                        PBlock Target = PMath.Max(Game.Map.FindBlock(Player).FindAll((PBlock Block) => Block.HouseNumber == MinHouse), (PBlock Block) => PAiMapAnalyzer.HouseValue(Game, Player, Block)).Key;
                        Game.GetHouse(Target, 1);
                    }
                };
            }));
    }

}