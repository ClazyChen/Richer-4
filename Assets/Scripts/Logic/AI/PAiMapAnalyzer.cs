
using System.Collections.Generic;

public class PAiMapAnalyzer {

    public static int StartFromExpect(PGame Game, PPlayer Player, PBlock Block) {
        PBlock CurrentBlock = Block.NextBlock;
        int Expectation = 0;
        for (int i = 5; i >= 0; -- i) {
            Expectation += Expect(Game, Player, Block);
            if (CurrentBlock.GetMoneyPassSolid != 0) {
                Expectation += i * CurrentBlock.GetMoneyPassSolid;
            }
            if (CurrentBlock.GetMoneyPassPercent != 0) {
                Expectation += i * PMath.Percent(Player.Money, CurrentBlock.GetMoneyPassPercent);
            }
            CurrentBlock = Block.NextBlock;
        }
        return Expectation / 6;
    }

    public static int Expect(PGame Game, PPlayer Player, PBlock Block) {
        int DeltaMoney = 0;
        DeltaMoney += Block.GetMoneyStopSolid;
        DeltaMoney += PMath.Percent(Player.Money, Block.GetMoneyStopPercent);
        if (Block.Lord != null && Block.Lord.TeamIndex != Player.TeamIndex) {
            DeltaMoney -= Block.Toll;
        }
        if (Player.Money + DeltaMoney <= 0) {
            return -30000 + DeltaMoney * 2;
        } else if (Player.Money <= 3000) {
            DeltaMoney *= 2;
        }
        int LandValue = 0;
        if (Block.Lord == null && Block.Price < Player.Money) {
            LandValue = PMath.Percent(Block.Price, 60);
            if (Block.IsBusinessLand) {
                LandValue += PMath.Max(PAiBusinessChooser.DirectionExpectations(Game, Player, Block));
            }
        } else if (Player.Equals(Block.Lord)) {
            LandValue += PMath.Percent(Block.Price, 40);
            if (Block.BusinessType.Equals(PBusinessType.Park)) {
                LandValue += PMath.Percent(Block.Price, 60);
            } else if (Block.BusinessType.Equals(PBusinessType.ShoppingCenter)) {
                LandValue += PMath.Percent(Block.Price, 40);
            }
        }
        if (Block.Lord != null && Block.Lord.TeamIndex == Player.TeamIndex) {
            if (Block.BusinessType.Equals(PBusinessType.Institute)) {
                LandValue += 2 * PAiCardExpectation.Expect(Game, Player);
            } else if (Block.BusinessType.Equals(PBusinessType.Pawnshop)) {
                LandValue += 2000;
            }
        }
        // 注：现在不进行关于当铺收益的计算
        return DeltaMoney + LandValue * 20 / GetRingLength(Game, Block);
    }

    public static int GetRingLength(PGame Game, PBlock Block) {
        Queue<PBlock> q = new Queue<PBlock>();
        q.Enqueue(Block);
        int[] Visited = new int[Game.Map.BlockList.Count];
        for (int i = 0; i < Visited.Length; ++i) {
            Visited[i] = int.MaxValue;
        }
        while (q.Count > 0) {
            PBlock Front = q.Dequeue();
            if (Front != null) {
                int v = Visited[Front.Index];
                if (Front.Equals(Block)) {
                    if (v != int.MaxValue) {
                        break;
                    } else {
                        v = 0;
                    }
                }
                Front.NextBlockList.ForEach((PBlock NextBlock) => {
                    if (Visited[NextBlock.Index] == int.MaxValue) {
                        Visited[NextBlock.Index] = v + 1;
                        q.Enqueue(NextBlock);
                    }
                });
            }
        }
        return Visited[Block.Index];
    }

}