
using System.Collections.Generic;

public class PAiMapAnalyzer {

    public static PBlock MaxValueHouse(PGame Game, PPlayer Player) {
        int EnemyCount = Game.Enemies(Player).Count;
        return PMath.Max(Game.Map.BlockList.FindAll((PBlock Block) => Player.Equals(Block.Lord) && Block.HouseNumber > 0), (PBlock Block) => {
            return PMath.Percent(Block.Price, 50 + 20 * EnemyCount * (Block.BusinessType.Equals(PBusinessType.ShoppingCenter) ? 2 : 1));
        }).Key;
    }

    public static int MaxHouseValue(PGame Game, PPlayer Player) {
        int MaxValue = int.MinValue;
        int EnemyCount = Game.Enemies(Player).Count;
        Game.Map.BlockList.ForEach((PBlock Block) => {
            if (Player.Equals(Block.Lord) && Block.HouseNumber > 0) {
                int Value = PMath.Percent(Block.Price, 50 + 20 * EnemyCount * (Block.BusinessType.Equals(PBusinessType.ShoppingCenter) ? 2 : 1));
                if (Value > MaxValue) {
                    MaxValue = Value;
                }
            }
        });
        return MaxValue;
    }

    public static int StartFromExpect(PGame Game, PPlayer Player, PBlock Block) {
        PBlock CurrentBlock = Block.NextBlock;
        int Expectation = 0;
        for (int i = 6; i >= 1; -- i) {
            Expectation += Expect(Game, Player, CurrentBlock);
            if (CurrentBlock.GetMoneyPassSolid != 0) {
                Expectation += i * CurrentBlock.GetMoneyPassSolid;
            }
            if (CurrentBlock.GetMoneyPassPercent != 0) {
                Expectation += i * PMath.Percent(Player.Money, CurrentBlock.GetMoneyPassPercent);
            }
            if (CurrentBlock.GetCardPass != 0) {
                Expectation += i * 2000 * CurrentBlock.GetCardPass;
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
        DeltaMoney += 2000 * Block.GetCardStop;
        int LandValue = 0;
        if (Block.Lord == null && Block.Price < Player.Money) {
            LandValue = PMath.Percent(Block.Price, 10) * Game.Enemies(Player).Count;
            if (Block.IsBusinessLand) {
                LandValue += PMath.Max(PAiBusinessChooser.DirectionExpectations(Game, Player, Block));
            }
        } else if (Player.Equals(Block.Lord)) {
            LandValue += PMath.Percent(Block.Price, 20) * Game.Enemies(Player).Count;
            if (Block.BusinessType.Equals(PBusinessType.Park)) {
                LandValue += PMath.Percent(Block.Price, 60);
            } else if (Block.BusinessType.Equals(PBusinessType.ShoppingCenter)) {
                LandValue += PMath.Percent(Block.Price, 20) * Game.Enemies(Player).Count;
            }
        }
        if (Block.Lord != null && Block.Lord.TeamIndex == Player.TeamIndex) {
            if (Block.BusinessType.Equals(PBusinessType.Institute)) {
                LandValue += 2 * 2000;
            } else if (Block.BusinessType.Equals(PBusinessType.Pawnshop)) {
                LandValue += 2000;
            }
        }
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