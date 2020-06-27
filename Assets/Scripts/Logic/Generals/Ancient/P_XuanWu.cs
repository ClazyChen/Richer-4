using System;
using System.Collections.Generic;

public class P_XuanWu : PGeneral {

    public P_XuanWu() : base("玄武") {
        Sex = PSex.NoSex;
        Age = PAge.Ancient;
        Index = 1004;
        Cost = 1;
        Tips = string.Empty;
        CanBeChoose = false;

        PSkill ShenShou = new PSkill("神兽") {
            Lock = true
        };
        SkillList.Add(ShenShou
            .AddTrigger(
            (PPlayer Player, PSkill Skill) => {
                return new PTrigger(ShenShou.Name) {
                    IsLocked = true,
                    Player = Player,
                    Time = PTime.StartGameTime,
                    AIPriority = 100,
                    Effect = (PGame Game) => {
                        ShenShou.AnnouceUseSkill(Player);
                        Game.GetMoney(Player, 30000);
                    }
                };
            })
            .AddTrigger(
            (PPlayer Player, PSkill Skill) => {
                return new PTrigger(ShenShou.Name) {
                    IsLocked = true,
                    Player = Player,
                    Time = PPeriod.StartTurn.During,
                    AIPriority = 100,
                    Condition = (PGame Game) => {
                        return Player.Equals(Game.NowPlayer);
                    },
                    Effect = (PGame Game) => {
                        ShenShou.AnnouceUseSkill(Player);
                        Game.GetMoney(Player, 500);
                    }
                };
            }));

        PSkill XuanWu = new PSkill("玄武") {
            Lock = true
        };
        SkillList.Add(XuanWu
            .AddTrigger(
            (PPlayer Player, PSkill Skill) => {
                return new PTrigger(XuanWu.Name) {
                    IsLocked = true,
                    Player = Player,
                    Time = PPeriod.EndTurn.During,
                    AIPriority = 250,
                    Condition = (PGame Game) => {
                        int MinMoney = PMath.Min(Game.PlayerList.FindAll((PPlayer _Player) => _Player.IsAlive), (PPlayer _Player) => _Player.Money).Value;
                        return Player.Equals(Game.NowPlayer) && Player.Money == MinMoney;
                    },
                    Effect = (PGame Game) => {
                        XuanWu.AnnouceUseSkill(Player);
                        Game.GetMoney(Player, 1000);
                        Game.ChangeFace(Player);
                    }
                };
            }));
    }

}