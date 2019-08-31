
using System.Collections.Generic;

public class PAiCardExpectation {
    /// <summary>
    /// 牌堆里的牌（未进入弃牌堆或可见区域）的平均收益
    /// </summary>
    /// <param name="Game"></param>
    /// <param name="Player"></param>
    /// <returns></returns>
    public static int Expect(PGame Game, PPlayer Player) {
        double Sum = PMath.Sum(Game.CardManager.CardHeap.CardList.ConvertAll((PCard Card) => (double)Card.Model.AIInHandExpectation(Game, Player)));
        int Count = Game.CardManager.CardHeap.CardNumber;
        
        if (Count <= 10) {
            Sum += PMath.Sum(Game.CardManager.ThrownCardHeap.CardList.ConvertAll((PCard Card) => (double)Card.Model.AIInHandExpectation(Game, Player)));
            Count += Game.CardManager.ThrownCardHeap.CardNumber;
        }

        if (Count == 0) {
            return 0;
        } else {
            return (int)Sum / Count;
        }
    }

    public static PCard FindLeastValuable(PGame Game, PPlayer Player, PPlayer TargetPlayer, bool AllowEquipment = true, bool AllowJudge = false) {
        // 装备和伏兵另外计算
        return PMath.Min(TargetPlayer.Area.HandCardArea.CardList, (PCard Card) => {
            if (Player.Equals(TargetPlayer)) {
                return Card.Model.AIInHandExpectation(Game, TargetPlayer);
            } else {
                return 1500 + PMath.RandInt(-10,10);
            }
        });
    }

    public static PCard FindMostValuable(PGame Game, PPlayer Player, PPlayer TargetPlayer, bool AllowEquipment = true, bool AllowJudge = false) {
        // 装备和伏兵另外计算
        return PMath.Max(TargetPlayer.Area.HandCardArea.CardList, (PCard Card) => {
            if (Player.Equals(TargetPlayer)) {
                return Card.Model.AIInHandExpectation(Game, TargetPlayer);
            } else {
                return 1500 + PMath.RandInt(-10, 10);
            }
        });
    }

    public static PPlayer MostValuableCardUser(PGame Game, List<PPlayer> PlayerList) {
        return PMath.Max(PlayerList, (PPlayer Player) => Expect(Game, Player) + PMath.RandInt(-10,10));
    }
}