public class PCardType : PObject {
    private PCardType(string _Name) {
        Name = _Name;
    }
    public static PCardType NoType = new PCardType("无类型");
    public static PCardType SchemeCard = new PCardType("计策牌");
    public static PCardType AmbushCard = new PCardType("伏击牌");
    public static PCardType WeaponCard = new PCardType("武器牌");
    public static PCardType DefensorCard = new PCardType("防具牌");
    public static PCardType TrafficCard = new PCardType("坐骑牌");

    public bool IsNormal() {
        return Equals(SchemeCard) || Equals(AmbushCard);
    }

    public bool IsEquipment() {
        return Equals(WeaponCard) || Equals(DefensorCard) || Equals(TrafficCard);
    }
}