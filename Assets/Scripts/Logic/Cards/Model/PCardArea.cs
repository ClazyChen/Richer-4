using System.Collections.Generic;
using System;

/// <summary>
/// PCardArea：牌的管理类
/// </summary>
public class PCardArea : PObject {
    public List<PCard> CardList;
    public PPlayer Owner = null;

    public PCardArea(string _Name, PPlayer _Owner = null) {
        Name = _Name;
        Owner = _Owner;
        CardList = new List<PCard>();
    }

    public bool IsHandCardArea() {
        return Owner != null && Equals(Owner.Area.HandCardArea);
    }

    public bool IsEquipmentArea() {
        return Owner != null && Equals(Owner.Area.EquipmentCardArea);
    }

    public bool IsAmbushArea() {
        return Owner != null && Equals(Owner.Area.AmbushCardArea);
    }

    /// <summary>
    /// 洗牌操作
    /// </summary>
    public void Wash() {
        for (int i = CardList.Count - 1; i > 0; --i) {
            int t1 = PMath.RandInt(0, i);
            int t2 = i;
            if (t1 != t2) {
                PCard Temp = CardList[t1];
                CardList[t1] = CardList[t2];
                CardList[t2] = Temp;
            }
        }
    }

    public PCard RandomCard() {
        if (CardNumber < 1) {
            return null;
        }
        return CardList[PMath.RandInt(0, CardList.Count - 1)];
    }

    public PCard TopCard {
        get {
            return CardList[0];
        }
    }

    public int CardNumber {
        get {
            return CardList.Count;
        }
    }

    public string[] ToStringArray() {
        string[] ans = new string[CardNumber];
        for (int i = 0; i < CardNumber; ++i) {
            ans[i] = CardList[i].Name;
        }
        return ans;
    }

    /// <summary>
    /// 按照type、点数整理
    /// </summary>
    public void Arrange() {
        CardList.Sort((PCard x, PCard y) => {
            if (x.Model.Type.Equals(y.Model.Type)) {
                return x.Model.Index - y.Model.Index;
            } else {
                return x.Model.Type.CompareWith(y.Model.Type);
            }
        });
    }

    /// <summary>
    /// 安全地获取一张牌
    /// </summary>
    /// <param name="Index"></param>
    /// <returns></returns>
    public PCard GetCard(int Index) {
        if (Index < 0 || Index >= CardList.Count) {
            return null; 
        } else {
            return CardList[Index];
        }
    }
}