public class PCardType : PObject {
    private PCardType(string _Name) {
        Name = _Name;
    }
    public static PCardType NoType = new PCardType("无类型");
    public static PCardType SimpleCard = new PCardType("普通锦囊牌");
    public static PCardType DelayCard = new PCardType("延时锦囊牌");
    public static PCardType WeaponCard = new PCardType("武器牌");
    public static PCardType DefensorCard = new PCardType("防具牌");
    public static PCardType TrafficCard = new PCardType("坐骑牌");

    public bool IsScheme() {
        return Equals(SimpleCard) || Equals(DelayCard);
    }

    public bool IsEquipment() {
        return Equals(WeaponCard) || Equals(DefensorCard) || Equals(TrafficCard);
    }
}