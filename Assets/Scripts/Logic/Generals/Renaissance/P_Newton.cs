using System;
using System.Collections.Generic;

public class P_Newton: PGeneral {

    public static KeyValuePair< PBlock, int> Grx_Next(PGame Game, PBlock Block) {
        if (Block.HouseNumber > 0) {
            return new KeyValuePair<PBlock, int>(Block, 0);
        }
        PBlock Answer = Block.NextBlock;
        int Count = 1;
        for (; Count < 6 && !Game.AlivePlayers().Exists((PPlayer _Player) => _Player.Position.Equals(Answer)) && 
            Answer.HouseNumber == 0; ++ Count) {
            Answer = Answer.NextBlock;
        }
        return new KeyValuePair<PBlock, int>( Answer, Count);
    }

    public P_Newton() : base("牛顿") {
        Sex = PSex.Male;
        Age = PAge.Renaissance;
        Index = 19;
        Cost = 40;
        Tips = "定位：防御\n" +
            "难度：中等\n" +
            "史实：英国物理学家、数学家。主要贡献包括但不限于发现万有引力定律、牛顿运动定律、光的色散原理，提出微积分、牛顿迭代法、二项式定理，发明反射式望远镜等。代表作《自然哲学的数学原理》。\n" +
            "攻略：\n牛顿是一个趣味性十足的武将，技能应用十分灵活，【惯性】的存在使其拥有极强的移动能力，能够灵活规避各种危险，同时在前期迅速攫取土地，中后期能够更快经过奖励处，从而拥有一定的续航能力。牛顿可以配合队友的上屋抽梯或逃避敌人的【剑舞】或【霸王】，需要较强的预判力。";

        PSkill Grx_ = new PSkill("惯性");
        SkillList.Add(Grx_
            .AddTrigger(
            (PPlayer Player, PSkill Skill) => {
                return new PTrigger(Grx_.Name) {
                    IsLocked = false,
                    Player = Player,
                    Time = PPeriod.WalkingStage.Start,
                    AIPriority = 10,
                    Condition = (PGame Game) => {
                        return Game.NowPlayer.Equals(Player) && Player.Position.HouseNumber == 0 && 
                        Player.Money > 500;
                    },
                    AICondition = (PGame Game) => {
                        PStepCountTag StepCountTag = Game.TagManager.FindPeekTag<PStepCountTag>(PStepCountTag.TagName);
                        int Current = PAiMapAnalyzer.Expect(Game, Player, Game.Map.NextStepBlock(Player.Position, StepCountTag.StepCount));
                        PBlock NewtonTarget = Grx_Next(Game, Player.Position).Key;
                        int Possible = PAiMapAnalyzer.Expect(Game, Player, Game.Map.NextStepBlock(NewtonTarget, StepCountTag.StepCount));
                        return Possible - Current > 500;
                    },
                    Effect = (PGame Game) => {
                        Grx_.AnnouceUseSkill(Player);
                        Game.LoseMoney(Player, 500);
                        Game.MovePosition(Player, Player.Position, Grx_Next(Game, Player.Position).Key);
                    }
                };
            }));
    }

}