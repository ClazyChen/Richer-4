
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

    public static PCard FindLeastValuable(PGame Game, PPlayer Player) {
        return PMath.Min(Player.Area.HandCardArea.CardList, (PCard Card) => {
            return Card.Model.AIInHandExpectation(Game, Player);
        });
    }
}