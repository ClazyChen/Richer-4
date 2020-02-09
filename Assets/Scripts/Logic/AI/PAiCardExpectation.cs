using System;
using System.Collections.Generic;

public class PAiCardExpectation {
    /// <summary>
    /// 牌堆里的牌（未进入弃牌堆或可见区域）的平均收益
    /// </summary>
    /// <param name="Game"></param>
    /// <param name="Player"></param>
    /// <returns></returns>
    public static int Expect(PGame Game, PPlayer Player) {
        double SingleExpect(PCard Card) {
            int Base = Card.Model.AIInHandExpectation(Game, Player);
            return Base;
        }

        double Sum = PMath.Sum(Game.CardManager.CardHeap.CardList.ConvertAll(SingleExpect));
        int Count = Game.CardManager.CardHeap.CardNumber;
        
        if (Count <= 10) {
            Sum += PMath.Sum(Game.CardManager.ThrownCardHeap.CardList.ConvertAll(SingleExpect));
            Count += Game.CardManager.ThrownCardHeap.CardNumber;
        }

        if (Count == 0) {
            return 0;
        } else {
            return (int)Sum / Count;
        }
    }

    /// <summary>
    /// 这个函数用来弃牌/转化牌，因此花木兰的装备value会被-2000计算
    /// </summary>
    /// <param name="Game"></param>
    /// <param name="Player">用来衡量价值的主视角</param>
    /// <param name="TargetPlayer">衡量对象区域的所有者</param>
    /// <param name="AllowHandCards"></param>
    /// <param name="AllowEquipment"></param>
    /// <param name="AllowAmbush"></param>
    /// <param name="CanSee"></param>
    /// <param name="Condition"></param>
    /// <returns></returns>
    public static KeyValuePair< PCard, int> FindLeastValuable(PGame Game, PPlayer Player, PPlayer TargetPlayer, bool AllowHandCards = true, bool AllowEquipment = true, bool AllowAmbush = false, bool CanSee = false, Predicate<PCard> Condition = null) {
        KeyValuePair<PCard, int> HandCardResult = AllowHandCards ?  PMath.Min(TargetPlayer.Area.HandCardArea.CardList.FindAll((PCard Card) => {
            return Condition == null || Condition(Card);
        }), (PCard Card) => {
            if (CanSee) {
                return Card.Model.AIInHandExpectation(Game, Player);
            } else {
                return 2000 + PMath.RandInt(-10,10);
            }
        }) : new KeyValuePair<PCard, int>(null, int.MaxValue);
        KeyValuePair<PCard, int> EquipResult = AllowEquipment ? PMath.Min(TargetPlayer.Area.EquipmentCardArea.CardList.FindAll((PCard Card) => {
            return Condition == null || Condition(Card);
        }), (PCard Card) => {
            int MulanCof = (TargetPlayer.General is P_HuaMulan ? 2000 : 0);
            if (CanSee) {
                int Current = Card.Model.AIInEquipExpectation(Game, TargetPlayer);
                int MaxEquip = PMath.Max(Player.Area.HandCardArea.CardList, (PCard _Card) => _Card.Model.AIInEquipExpectation(Game, Player)).Value;
                if (Current <= MaxEquip) {
                    return 500 - MulanCof;
                } else {
                    return Current - Math.Max(0, MaxEquip) - MulanCof;
                }
            } else {
                return Card.Model.AIInEquipExpectation(Game, Player) - MulanCof;
            }
        }) : new KeyValuePair<PCard, int>(null, int.MaxValue);
        KeyValuePair<PCard, int> AmbushResult = AllowAmbush ? PMath.Min(TargetPlayer.Area.AmbushCardArea.CardList.FindAll((PCard Card) => {
            return Condition == null || Condition(Card);
        }), (PCard Card) => {
            return Card.Model.AIInAmbushExpectation(Game, TargetPlayer);
        }) : new KeyValuePair<PCard, int>(null, int.MaxValue);
        KeyValuePair<PCard, int> Temp = HandCardResult.Value <= EquipResult.Value ? HandCardResult : EquipResult;
        Temp = Temp.Value <= AmbushResult.Value ? Temp : AmbushResult;
        if (Temp.Key == null) {
            return new KeyValuePair<PCard, int>(null, 0);
        }
        return Temp;
    }

    /// <summary>
    /// 用于交给队友有价值的牌和弃置敌人的高价值牌
    /// </summary>
    /// <param name="Game"></param>
    /// <param name="Player"></param>
    /// <param name="TargetPlayer"></param>
    /// <param name="AllowHandCards"></param>
    /// <param name="AllowEquipment"></param>
    /// <param name="AllowAmbush"></param>
    /// <param name="CanSee"></param>
    /// <returns></returns>
    public static KeyValuePair<PCard, int> FindMostValuable(PGame Game, PPlayer Player, PPlayer TargetPlayer, bool AllowHandCards = true, bool AllowEquipment = true, bool AllowAmbush = false, bool CanSee = false) {
        int Cof = Player.TeamIndex == TargetPlayer.TeamIndex ? 1 : -1;
        KeyValuePair<PCard, int> HandCardResult = AllowHandCards ? PMath.Max(TargetPlayer.Area.HandCardArea.CardList, (PCard Card) => {
            if (CanSee) {
                return Card.Model.AIInHandExpectation(Game, Player);
            } else {
                return 2000 + PMath.RandInt(-10, 10);
            }
        }) : new KeyValuePair<PCard, int>(null, int.MinValue);
        KeyValuePair<PCard, int> EquipResult = AllowEquipment ? PMath.Max(TargetPlayer.Area.EquipmentCardArea.CardList, (PCard Card) => {
            return Card.Model.AIInEquipExpectation(Game, TargetPlayer) + (TargetPlayer.General is P_HuaMulan ? 2000 * Cof : 0);
        }) : new KeyValuePair<PCard, int>(null,int.MinValue);
        KeyValuePair<PCard, int> AmbushResult = AllowAmbush ? PMath.Max(TargetPlayer.Area.AmbushCardArea.CardList, (PCard Card) => {
            return Card.Model.AIInAmbushExpectation(Game, TargetPlayer);
        }) : new KeyValuePair<PCard, int>(null, int.MinValue);
        KeyValuePair<PCard, int> Temp = HandCardResult.Value >= EquipResult.Value ? HandCardResult : EquipResult;
        Temp = Temp.Value >= AmbushResult.Value ? Temp : AmbushResult;
        if (Temp.Key == null) {
            return new KeyValuePair<PCard, int>(null, 0);
        }
        return Temp;
    }

    public static KeyValuePair<PCard, int> FindMostValuableToGet(PGame Game, PPlayer Player, PPlayer TargetPlayer, bool AllowHandCards = true, bool AllowEquipment = true, bool AllowAmbush = false, bool CanSee = false) {
        int Cof = Player.TeamIndex == TargetPlayer.TeamIndex ? -1 : 1;
        int YangToowCof = TargetPlayer.Traffic != null && TargetPlayer.Traffic.Model is P_HsiYooYangToow && !Player.Age.Equals(TargetPlayer.Age) ? 0 : 1;
        KeyValuePair<PCard, int> HandCardResult = AllowHandCards ? PMath.Max(TargetPlayer.Area.HandCardArea.CardList, (PCard Card) => {
            if (CanSee) {
                return Card.Model.AIInHandExpectation(Game, Player) * YangToowCof + Cof * Card.Model.AIInHandExpectation(Game, TargetPlayer);
            } else {
                return Cof < 0 ? 0 : 2000 * YangToowCof + 2000 * Cof + PMath.RandInt(0, 10);
            }
        }) : new KeyValuePair<PCard, int>(null, int.MinValue);
        KeyValuePair<PCard, int> EquipResult = AllowEquipment ? PMath.Max(TargetPlayer.Area.EquipmentCardArea.CardList, (PCard Card) => {
            return Card.Model.AIInHandExpectation(Game, Player)+ Cof* Card.Model.AIInEquipExpectation(Game, TargetPlayer) - (TargetPlayer.General is P_HuaMulan ? 2000 * Cof : 0);
        }) : new KeyValuePair<PCard, int>(null, int.MinValue);
        KeyValuePair<PCard, int> AmbushResult = AllowAmbush ? PMath.Max(TargetPlayer.Area.AmbushCardArea.CardList, (PCard Card) => {
            return Card.Model.AIInHandExpectation(Game, Player) + Cof * Card.Model.AIInAmbushExpectation(Game, TargetPlayer);
        }) : new KeyValuePair<PCard, int>(null, int.MinValue);
        KeyValuePair<PCard, int> Temp = HandCardResult.Value >= EquipResult.Value ? HandCardResult : EquipResult;
        Temp = Temp.Value >= AmbushResult.Value ? Temp : AmbushResult;
        if (Temp.Key == null) {
            return new KeyValuePair<PCard, int>(null, 0);
        }
        return Temp;
    }

    public static PPlayer MostValuableCardUser(PGame Game, List<PPlayer> PlayerList) {
        return PMath.Max(PlayerList, (PPlayer Player) => Expect(Game, Player) + PMath.RandInt(-10,10)).Key;
    }
}