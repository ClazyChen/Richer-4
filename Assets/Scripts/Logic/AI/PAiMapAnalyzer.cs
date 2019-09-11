using System;
using System.Collections.Generic;

public class PAiMapAnalyzer {

    /// <summary>
    /// 将一个玩家移出游戏对其的收益
    /// </summary>
    /// <param name="Game"></param>
    /// <param name="Player"></param>
    /// <param name="Including">是否包含其行走阶段</param>
    /// <returns></returns>
    public static int OutOfGameExpect(PGame Game, PPlayer Player, bool Including = false, bool IncludingOnly = false) {
        if (Player.OutOfGame) {
            return 0;
        }
        int Sum = 0;
        if (!IncludingOnly) {
            for (PPlayer _Player = Game.NowPlayer; !_Player.Equals(Player); _Player = Game.GetNextPlayer(_Player)) {
                if (_Player.NoLadder) {
                    PBlock Block = _Player.Position;
                    if (Player.Equals(Block.Lord) && Player.TeamIndex != _Player.TeamIndex) {
                        Sum -= 12 * Block.Toll;
                        if (Player.Weapon != null && Player.Weapon.Model is P_KuTingTao && _Player.Area.HandCardArea.CardNumber == 0) {
                            Sum -= 12 * Block.Toll;
                        }
                    }
                } else {
                    PBlock Block = _Player.Position.NextBlock;
                    for (int i = 0; i < 6; ++i, Block = Block.NextBlock) {
                        if (Player.Equals(Block.Lord) && Player.TeamIndex != _Player.TeamIndex) {
                            Sum -= 2 * Block.Toll;
                            if (Player.Weapon != null && Player.Weapon.Model is P_KuTingTao && _Player.Area.HandCardArea.CardNumber == 0) {
                                Sum -= 2 * Block.Toll;
                            }
                        }
                    }
                }

            }
        }
        if (Including) {
            if (Player.NoLadder) {
                PBlock Block = Player.Position;
                if (Block.Lord != null && Player.TeamIndex != Block.Lord.TeamIndex) {
                    Sum += 12 * Block.Toll;
                    if (Block.Lord.Weapon != null && Block.Lord.Weapon.Model is P_KuTingTao && Player.Area.HandCardArea.CardNumber == 0) {
                        Sum += 12 * Block.Toll;
                    }
                }
            } else {
                PBlock Block = Player.Position.NextBlock;
                for (int i = 0; i < 6; ++i, Block = Block.NextBlock) {
                    if (Block.Lord != null && Player.TeamIndex != Block.Lord.TeamIndex) {
                        Sum += 2 * Block.Toll;
                        if (Block.Lord.Weapon != null && Block.Lord.Weapon.Model is P_KuTingTao && Player.Area.HandCardArea.CardNumber == 0) {
                            Sum += 2 * Block.Toll;
                        }
                    }
                }
            }
        }
        return Sum;
    }

    public static int ChangeFaceExpect(PGame Game, PPlayer Player, PBlock Start = null) {
        if (Start == null) {
            Start = Player.Position;
        }
        return StartFromExpect(Game, Player, Start) * (Player.BackFace ? 1 : -1) / 3;
    }

    public static int HouseValue(PGame Game, PPlayer Player, PBlock Block) {
        if (Block.Lord == null) {
            return 0;
        }
        int EnemyCount = Game.Enemies(Block.Lord).Count;
        int Value = PMath.Percent(Block.Price, 50 + 20 * EnemyCount * (Block.BusinessType.Equals(PBusinessType.ShoppingCenter) ? 2 : 1));
        if (Player.TeamIndex == Block.Lord.TeamIndex) {
            return Value;
        } else {
            return -Value;
        }
    }

    public static KeyValuePair< PBlock, int> MaxValueHouse(PGame Game, PPlayer Player, bool StartFromZero = false) {
        int EnemyCount = Game.Enemies(Player).Count;
        return PMath.Max(Game.Map.BlockList.FindAll((PBlock Block) => Player.Equals(Block.Lord) && ( StartFromZero || Block.HouseNumber > 0)), (PBlock Block) => {
            return PMath.Percent(Block.Price, 50 + 20 * EnemyCount * (Block.BusinessType.Equals(PBusinessType.ShoppingCenter) ? 2 : 1));
        });
    }

    public static KeyValuePair<PBlock, int> MinValueHouse(PGame Game, PPlayer Player) {
        int EnemyCount = Game.Enemies(Player).Count;
        return PMath.Min(Game.Map.BlockList.FindAll((PBlock Block) => Player.Equals(Block.Lord) && Block.HouseNumber > 0), (PBlock Block) => {
            return PMath.Percent(Block.Price, 50 + 20 * EnemyCount * (Block.BusinessType.Equals(PBusinessType.ShoppingCenter) ? 2 : 1));
        });
    }

    public static int StartFromExpect(PGame Game, PPlayer Player, PBlock Block) {
        if (Player.NoLadder) {
            return Expect(Game, Player, Block);
        }
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

    public static int Expect(PGame Game, PPlayer Player, PBlock Block, bool InPortal = false) {
        int DeltaMoney = 0;
        DeltaMoney += Block.GetMoneyStopSolid;
        DeltaMoney += PMath.Percent(Player.Money, Block.GetMoneyStopPercent);
        if (Block.Lord != null && Block.Lord.TeamIndex != Player.TeamIndex) {
            DeltaMoney -= Block.Toll;
            if (Block.Lord.Weapon != null && Block.Lord.Weapon.Model is P_KuTingTao && Player.Area.HandCardArea.CardNumber == 0) {
                DeltaMoney -= Block.Toll;
            } 
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
            int PurchaseLimit = Player.PurchaseLimit;
            LandValue += PMath.Percent(Block.Price, 20) * Game.Enemies(Player).Count * PurchaseLimit;
            if (Block.BusinessType.Equals(PBusinessType.Park)) {
                LandValue += PMath.Percent(Block.Price, 60) * PurchaseLimit;
            } else if (Block.BusinessType.Equals(PBusinessType.ShoppingCenter)) {
                LandValue += PMath.Percent(Block.Price, 20) * Game.Enemies(Player).Count * PurchaseLimit;
            }
        }
        if (Block.Lord != null && Block.Lord.TeamIndex == Player.TeamIndex) {
            if (Block.BusinessType.Equals(PBusinessType.Institute)) {
                LandValue += 2 * 2000;
            } else if (Block.BusinessType.Equals(PBusinessType.Pawnshop)) {
                LandValue += 2000;
            }
        }

        int PortalValue = 0;
        if (!InPortal && Block.PortalBlockList.Count > 0) {
            PortalValue = PMath.Max(Block.PortalBlockList, (PBlock _Block) => Expect(Game, Player, _Block, true)).Value;
        }
        return DeltaMoney + LandValue * 20 / GetRingLength(Game, Block) + PortalValue;
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