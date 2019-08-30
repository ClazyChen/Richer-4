
using System;
using System.Collections.Generic;

public class PAiBusinessChooser {

    public static List<int> DirectionExpectations(PGame Game, PPlayer Player, PBlock Block) {
        /*
         * AI决策商业用地类型的机制：
         * 购物中心收益：2*（40%*max（1，20*建房次数上限/环长）+20%）*地价*敌方人数
         * 研究所收益  ：牌堆期望收益*2.5*己方人数
         * 公园收益    ：（max（1，20*建房次数上限/环长）*60%*建房次数上限+50%）*地价
         * 城堡收益    ：40%*赠送房屋数量*地价*敌方人数
         * 当铺收益    ：（max（自身牌堆期望收益-min队友牌堆期望收益，0）+1500）*己方人数
         */
        int RingLength = PAiMapAnalyzer.GetRingLength(Game, Block);
        int MaxOperationCount = 1;

        int ShoppingCenterExpectation = 2 * PMath.Percent(Block.Price, 40 * Math.Max(1, 20 * MaxOperationCount / RingLength) + 20) * Game.Enemies(Player).Count;
        int InsituteExpectation = Player.AiCardExpectation * 5 * Game.Teammates(Player).Count / 2;
        int ParkExpectation = PMath.Percent(Block.Price, 60 * Math.Max(1, 20 * MaxOperationCount / RingLength) + 50);
        int CastleExpectation = 2 * PMath.Percent(Block.Price, 40 * Game.GetBonusHouseNumberOfCastle(Player, Block)) * Game.Enemies(Player).Count;
        int PawnshopExpectation = (Math.Max(0, Player.AiCardExpectation - PMath.Min(Game.Teammates(Player, false).ConvertAll((PPlayer Teammate) => Teammate.AiCardExpectation))) + 1500) * Game.Teammates(Player).Count;
        List<int> ExpectationList = new List<int>() {
            ShoppingCenterExpectation,
            InsituteExpectation,
            ParkExpectation,
            CastleExpectation,
            PawnshopExpectation
        };

        return ExpectationList;
    }

    public static PBusinessType ChooseDirection(PGame Game, PPlayer Player, PBlock Block) {
        List<int> ExpectationList = DirectionExpectations(Game, Player, Block);
        List<double> Weights = ExpectationList.ConvertAll((int Raw) => Math.Pow(Math.E, (double)Raw / 1000));
        return new PBusinessType[] {
            PBusinessType.ShoppingCenter,
            PBusinessType.Institute,
            PBusinessType.Park,
            PBusinessType.Castle,
            PBusinessType.Pawnshop
        }[PMath.RandomIndex(Weights)];
    }
}