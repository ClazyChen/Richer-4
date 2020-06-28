using System;
using System.Collections.Generic;

public class P_BaiHu : PGeneral {

    public P_BaiHu() : base("白虎") {
        Sex = PSex.NoSex;
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
                    Time = PTime.Injure.EmitInjure,
                    AIPriority = 250,
                    Condition = (PGame Game) => {
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        PPlayer ToPlayer = InjureTag.ToPlayer;
                        return InjureTag.Injure > 0 && Player.Equals(InjureTag.FromPlayer) && Game.AlivePlayers(Player).Exists((PPlayer _Player) => !_Player.Equals(ToPlayer));
                    },
                    Effect = (PGame Game) => {
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        PPlayer ToPlayer = InjureTag.ToPlayer;
                        BaiHu.AnnouceUseSkill(Player);
                        PPlayer Another = Player;
                        if (Game.Enemies(Player).Exists((PPlayer _Player) => !_Player.Equals(ToPlayer))) {
                            // another enemy
                            Another = PMath.Min(Game.Enemies(Player).FindAll((PPlayer _Player) => !_Player.Equals(ToPlayer)), (PPlayer _Player) => _Player.Money).Key;
                        } else {
                            Another = PMath.Max(Game.Teammates(Player).FindAll((PPlayer _Player) => !_Player.Equals(ToPlayer) && !_Player.Equals(Player)), (PPlayer _Player) => _Player.Money).Key;
                        }
                        if (!Another.Equals(Player)) {
                            Game.LoseMoney(Another, InjureTag.Injure);
                        }
                    }
                };
            }));
    }

}