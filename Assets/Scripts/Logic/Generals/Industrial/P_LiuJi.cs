using System;
using System.Collections.Generic;

public class P_LiuJi: PGeneral {

    public P_LiuJi() : base("刘基") {
        Sex = PSex.Male;
        Age = PAge.Industrial;
        Index = 12;
        Cost = 10;
        Tips = "定位：防御\n" +
            "难度：简单\n" +
            "史实：明代政治家、文学家，“明初诗文三大家”之一，在平定张士诚、陈友谅和北伐中作为参谋，以神机妙算著称。\n" +
            "攻略：\n刘基作为军事大师，两个技能均和判定有关，可以称作是使用伏兵的专家。刘基可以从【闪电】、【瘟疫】、【陷阱】这类会轮转的牌中获益，而对于【乐不思蜀】、【兵粮寸断】有天然的免疫能力。\n【八卦阵】可以说是刘基的神器，当刘基持有【八卦阵】时，每次受到伤害不但减半，而且反而可以摸1000，配合陈圆圆，队友之间互相伤害可以形成共赢的效果。";

        PSkill MiaoSuan = new PSkill("妙算") {
            SoftLockOpen = true
        };
        SkillList.Add(MiaoSuan
             .AddTrigger(
            (PPlayer Player, PSkill Skill) => {
                return new PTrigger(MiaoSuan.Name) {
                    IsLocked = false,
                    Player = Player,
                    Time = PTime.Judge.JudgeTime,
                    AIPriority = 100,
                    Condition = (PGame Game) => {
                        PJudgeTag JudgeTag = Game.TagManager.FindPeekTag<PJudgeTag>(PJudgeTag.TagName);
                        return JudgeTag.Player.Equals(Player);
                    },
                    Effect = (PGame Game) => {
                        PJudgeTag JudgeTag = Game.TagManager.FindPeekTag<PJudgeTag>(PJudgeTag.TagName);
                        MiaoSuan.AnnouceUseSkill(Player);
                        int ChosenResult = 1;
                        if (Player.IsAI) {
                            ChosenResult = JudgeTag.AdvisedResult;
                        } else {
                            ChosenResult = PNetworkManager.NetworkServer.ChooseManager.Ask1To6(Player, MiaoSuan.Name);
                        }
                        JudgeTag.Result = ChosenResult;
                    }
                };
            }));

        PSkill TianDu = new PSkill("天妒") {
            SoftLockOpen = true
        };
        SkillList.Add(TianDu
             .AddTrigger(
            (PPlayer Player, PSkill Skill) => {
                return new PTrigger(TianDu.Name) {
                    IsLocked = false,
                    Player = Player,
                    Time = PTime.Judge.AfterJudgeTime,
                    AIPriority = 100,
                    Condition = (PGame Game) => {
                        PJudgeTag JudgeTag = Game.TagManager.FindPeekTag<PJudgeTag>(PJudgeTag.TagName);
                        return JudgeTag.Player.Equals(Player);
                    },
                    Effect = (PGame Game) => {
                        PJudgeTag JudgeTag = Game.TagManager.FindPeekTag<PJudgeTag>(PJudgeTag.TagName);
                        TianDu.AnnouceUseSkill(Player);
                        Game.GetMoney(Player, 200 * JudgeTag.Result);
                    }
                };
            }));
    }

}