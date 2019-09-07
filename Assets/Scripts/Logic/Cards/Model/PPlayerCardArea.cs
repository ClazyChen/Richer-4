using System.Collections.Generic;

/// <summary>
/// PPlayerCardArea：一个玩家所有的牌的区域
/// </summary>
public class PPlayerCardArea : PObject {
    public PCardArea HandCardArea;
    public PCardArea EquipmentCardArea;
    public PCardArea JudgeCardArea;
    public PPlayer Owner;

    public PPlayerCardArea(PPlayer _Owner) {
        Owner = _Owner;
        Name = Owner.Name + "的区域";
        HandCardArea = new PCardArea(Owner.Name + "的手牌", Owner);
        EquipmentCardArea = new PCardArea(Owner.Name + "的装备区", Owner);
        JudgeCardArea = new PCardArea(Owner.Name + "的判定区", Owner);
    }

    public int CardNumber {
        get {
            return HandCardArea.CardNumber + EquipmentCardArea.CardNumber + JudgeCardArea.CardNumber;
        }
    }

    public int OwnerCardNumber {
        get {
            return HandCardArea.CardNumber + EquipmentCardArea.CardNumber;
        }
    }

    /// <summary>
    /// 0~999为手牌，1000起装备区，2000起判定区
    /// </summary>
    /// <param name="Index"></param>
    /// <returns></returns>
    public PCard GetCard(int Index, bool AllowHandCards = true, bool AllowEquipment = true, bool AllowJudge = false) {
        if (Index < 1000 && AllowHandCards) {
            return HandCardArea.GetCard(Index);
        } else if (Index < 2000 && AllowEquipment) {
            return EquipmentCardArea.GetCard(Index - 1000);
        } else if (AllowJudge) {
            return JudgeCardArea.GetCard(Index - 2000);
        }
        return null;
    }

    public void Clear() {
        HandCardArea.CardList.Clear();
        EquipmentCardArea.CardList.Clear();
        JudgeCardArea.CardList.Clear();
    }
}