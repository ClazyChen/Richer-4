
using System;
using System.Collections.Generic;

public class PAiBusinessChooser {

    public static PBusinessType ChooseDirection(PGame Game, PPlayer Player, PBlock Block) {
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

        int ShoppingCenterExpectation = 2 * PMath.Percent(Block.Price, 40 * Math.Max(1, 20 * MaxOperationCount/ RingLength)  + 20) * Game.Enemies(Player).Count;
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

        int MinExpectation = Math.Max(1, PMath.Min(ExpectationList));
        PLogger.Log("AI-商业用地方向决策权重:" + string.Join(" ", ExpectationList.ConvertAll((int x) => x.ToString())));
        ExpectationList = ExpectationList.ConvertAll((int Raw) => Raw / MinExpectation);
        PLogger.Log("AI-商业用地方向决策权重:" + string.Join(" ", ExpectationList.ConvertAll((int x) => x.ToString())));
        ExpectationList = ExpectationList.ConvertAll((int Raw) => Raw * Raw * Raw);
        PLogger.Log("AI-商业用地方向决策权重:" + string.Join(" ", ExpectationList.ConvertAll((int x) => x.ToString())));
        if (PMath.RandTest((double)ShoppingCenterExpectation / PMath.Sum(ExpectationList))) {
            return PBusinessType.ShoppingCenter;
        }
        if (PMath.RandTest((double)InsituteExpectation / PMath.Sum(ExpectationList.GetRange(1,4)))) {
            return PBusinessType.Institute;
        }
        if (PMath.RandTest((double)ParkExpectation / PMath.Sum(ExpectationList.GetRange(2,3)))) {
            return PBusinessType.Park;
        }
        if (PMath.RandTest((double)CastleExpectation / PMath.Sum(ExpectationList.GetRange(3,2)))) {
            return PBusinessType.Castle;
        }
        return PBusinessType.Pawnshop;
    }
}