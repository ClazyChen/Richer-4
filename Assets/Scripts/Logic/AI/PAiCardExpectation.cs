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


    public static KeyValuePair< PCard, int> FindLeastValuable(PGame Game, PPlayer Player, PPlayer TargetPlayer, bool AllowHandCards = true, bool AllowEquipment = true, bool AllowJudge = false, bool CanSee = false) {
        // 装备和伏兵另外计算
        KeyValuePair<PCard, int> HandCardResult = AllowHandCards ?  PMath.Min(TargetPlayer.Area.HandCardArea.CardList, (PCard Card) => {
            if (CanSee) {
                return Card.Model.AIInHandExpectation(Game, Player);
            } else {
                return 2000 + PMath.RandInt(-10,10);
            }
        }) : new KeyValuePair<PCard, int>(null, int.MaxValue);
        KeyValuePair<PCard, int> EquipResult = AllowEquipment ? PMath.Min(TargetPlayer.Area.EquipmentCardArea.CardList, (PCard Card) => {
            if (CanSee) {
                int Current = Card.Model.AIInEquipExpectation(Game, TargetPlayer);
                int MaxEquip = PMath.Max(Player.Area.HandCardArea.CardList, (PCard _Card) => _Card.Model.AIInEquipExpectation(Game, Player)).Value;
                if (Current <= MaxEquip) {
                    return 500;
                } else {
                    return Current - Math.Max(0, MaxEquip);
                }
            } else {
                return Card.Model.AIInEquipExpectation(Game, Player);
            }
        }) : new KeyValuePair<PCard, int>(null, int.MaxValue);
        KeyValuePair<PCard, int> Temp = HandCardResult.Value <= EquipResult.Value ? HandCardResult : EquipResult;
        return Temp;
    }

    public static KeyValuePair<PCard, int> FindMostValuable(PGame Game, PPlayer Player, PPlayer TargetPlayer, bool AllowHandCards = true, bool AllowEquipment = true, bool AllowJudge = false, bool CanSee = false) {
        // 装备和伏兵另外计算
        KeyValuePair<PCard, int> HandCardResult = AllowHandCards ? PMath.Max(TargetPlayer.Area.HandCardArea.CardList, (PCard Card) => {
            if (CanSee) {
                return Card.Model.AIInHandExpectation(Game, Player);
            } else {
                return 2000 + PMath.RandInt(-10, 10);
            }
        }) : new KeyValuePair<PCard, int>(null, int.MinValue);
        KeyValuePair<PCard, int> EquipResult = AllowEquipment ? PMath.Max(TargetPlayer.Area.EquipmentCardArea.CardList, (PCard Card) => {
            return Card.Model.AIInEquipExpectation(Game, TargetPlayer);
        }) : new KeyValuePair<PCard, int>(null,int.MinValue);
        KeyValuePair<PCard, int> Temp = HandCardResult.Value >= EquipResult.Value ? HandCardResult : EquipResult;
        return Temp;
    }

    public static PPlayer MostValuableCardUser(PGame Game, List<PPlayer> PlayerList) {
        return PMath.Max(PlayerList, (PPlayer Player) => Expect(Game, Player) + PMath.RandInt(-10,10)).Key;
    }
}