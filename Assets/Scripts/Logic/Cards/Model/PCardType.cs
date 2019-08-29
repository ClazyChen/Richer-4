public class PCardType : PObject {
    private readonly int Id;

    private PCardType(string _Name, int _Id) {
        Name = _Name;
        Id = _Id;
    }
    public static PCardType NoType = new PCardType("无类型", 0);
    public static PCardType SchemeCard = new PCardType("计策牌", 1);
    public static PCardType AmbushCard = new PCardType("伏击牌", 2);
    public static PCardType WeaponCard = new PCardType("武器牌", 3);
    public static PCardType DefensorCard = new PCardType("防具牌", 4);
    public static PCardType TrafficCard = new PCardType("坐骑牌", 5);

    public bool IsNormal() {
        return Equals(SchemeCard) || Equals(AmbushCard);
    }

    public bool IsEquipment() {
        return Equals(WeaponCard) || Equals(DefensorCard) || Equals(TrafficCard);
    }

    public int CompareWith(PCardType CardType) {
        return Id - CardType.Id;
    }
}