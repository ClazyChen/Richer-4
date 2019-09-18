using System;
using System.Collections.Generic;

public class P_PanYue: PGeneral {

    public static bool XianJuTest(PGame Game, PPlayer Player) {
        string XianJu = "闲居";

        // 依据闲居，优化使用伤害类计策牌的条件
        if (Player.General is P_PanYue && Game.NowPlayer.Equals(Player)) {
            if (!Game.NowPeriod.IsAfter(PPeriod.WalkingStage)) {
                List<PBlock> NextBlocks = PAiMapAnalyzer.NextBlocks(Game, Player);
                return NextBlocks.Exists((PBlock Block) => Block.Lord != null && Block.Lord.TeamIndex != Player.TeamIndex && Block.Toll >= Player.Money) || !NextBlocks.Exists((PBlock Block) => Block.Lord != null && Block.Lord.TeamIndex == Player.TeamIndex) || !Player.RemainLimit(XianJu);
            } else {
                return Player.Position.Lord == null || Player.Position.Lord.TeamIndex != Player.TeamIndex || Player.Money <= 2000 || !Player.RemainLimit(XianJu);
            }
        }
        return true;
    }

    public P_PanYue() : base("潘岳") {
        Sex = PSex.Male;
        Age = PAge.Medieval;
        Index = 2;
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
        PSkill XianJu = new PSkill("闲居");
        SkillList.Add(XianJu
            // 用每回合限一次这个模板控制条件
            .AnnouceTurnOnce()
            // 回合结束时触发技能
            .AddTrigger(
            (PPlayer Player, PSkill Skill) => {
                return new PTrigger(XianJu.Name) {
                    IsLocked = false,
                    Player = Player,
                    Time = PPeriod.EndTurn.During,
                    AIPriority = 100,
                    Condition = (PGame Game) => {
                        return Player.Equals(Game.NowPlayer) && Player.Position.Lord != null && !Player.Equals(Player.Position.Lord) && Player.RemainLimit(XianJu.Name);
                    },
                    AICondition = (PGame Game) => {
                        return Player.Position.Lord.TeamIndex == Player.TeamIndex;
                    },
                    Effect = (PGame Game) => {
                        XianJu.AnnouceUseSkill(Player);
                        Game.GetHouse(Player.Position, 1);
                    }
                };
            })

            // 伤害结算结束时，如果造成或受到伤害，将每回合限一次的模板打上标记
            .AddTrigger(
            (PPlayer Player, PSkill Skill) => {
                return new PTrigger(XianJu.Name  + "[条件记录]") {
                    IsLocked = true,
                    Player = Player,
                    Time = PTime.Injure.EndSettle,
                    Condition = (PGame Game) => {
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        return Player.Equals(Game.NowPlayer) && (Player.Equals(InjureTag.ToPlayer) || Player.Equals(InjureTag.FromPlayer)) && InjureTag.Injure > 0;
                    },
                    Effect = (PGame Game) => {
                        Player.Tags.FindPeekTag<PUsedTag>(PUsedTag.TagNamePrefix + XianJu.Name).Count++;
                    }
                };
            }));
    }

}