using System;
using System.Collections.Generic;

public class P_ZhangSanFeng : PGeneral {

    public static PTag PYinTag = new PTag("阴");
    public static PTag PYangTag = new PTag("阳");

    public P_ZhangSanFeng() : base("张三丰") {
        Sex = PSex.Male;
        Age = PAge.Industrial;
        Index = 8;
        PSkill TaiJi = new PSkill("太极") {
            Lock = true
        };
        SkillList.Add(TaiJi
             .AddTrigger(
            (PPlayer Player, PSkill Skill) => {
                return new PTrigger(TaiJi.Name) {
                    IsLocked = true,
                    Player = Player,
                    Time = PPeriod.StartTurn.During,
                    AIPriority = 200,
                    Condition = (PGame Game) => {
                        return Game.NowPlayer.Equals(Player);
                    },
                    Effect = (PGame Game) => {
                        TaiJi.AnnouceUseSkill(Player);
                        Player.Tags.PopTag<PTag>(PYinTag.Name);
                        Player.Tags.PopTag<PTag>(PYangTag.Name);
                        int ChooseResult = 0;
                        if (Player.IsAI) {
                            int Yin = -PAiMapAnalyzer.OutOfGameExpect(Game, Player, false);
                            int Yang = PAiMapAnalyzer.OutOfGameExpect(Game, Player, true);
                            ChooseResult = (Yin >= Yang ? 0 : 1);
                        } else {
                            ChooseResult = PNetworkManager.NetworkServer.ChooseManager.Ask(Player, TaiJi.Name, new string[] { "阴", "阳" },
                                new string[] { "造成的伤害+20%", "受到的伤害-20%" });
                        }
                        if (ChooseResult == 0) {
                            Player.Tags.CreateTag(PYinTag);
                        } else {
                            Player.Tags.CreateTag(PYangTag);
                        }
                    }
                };
            })
            .AddTrigger(
            (PPlayer Player, PSkill Skill) => {
                return new PTrigger(TaiJi.Name + "[阴]") {
                    IsLocked = true,
                    Player = Player,
                    Time = PTime.Injure.EmitInjure,
                    AIPriority = 100,
                    Condition = (PGame Game) => {
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        return Player.Tags.ExistTag(PYinTag.Name) && InjureTag.Injure > 0 && InjureTag.ToPlayer != null && Player.Equals(InjureTag.FromPlayer);
                    },
                    Effect = (PGame Game) => {
                        TaiJi.AnnouceUseSkill(Player);
                        Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName).Injure += PMath.Percent(Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName).Injure, 20);
                    }
                };
            })
            .AddTrigger(
            (PPlayer Player, PSkill Skill) => {
                return new PTrigger(TaiJi.Name + "[阳]") {
                    IsLocked = true,
                    Player = Player,
                    Time = PTime.Injure.AcceptInjure,
                    AIPriority = 100,
                    Condition = (PGame Game) => {
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        return Player.Tags.ExistTag(PYangTag.Name) && InjureTag.Injure > 0 && Player.Equals(InjureTag.ToPlayer);
                    },
                    Effect = (PGame Game) => {
                        TaiJi.AnnouceUseSkill(Player);
                        Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName).Injure -= PMath.Percent(Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName).Injure, 20);
                    }
                };
            }));
    }

}