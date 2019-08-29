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

    public void Clear() {
        HandCardArea.CardList.Clear();
        EquipmentCardArea.CardList.Clear();
        JudgeCardArea.CardList.Clear();
    }
}