/// <summary>
/// PTime：
/// 用来表示发动时机的类
/// </summary>
public class PTime : PObject {
    public PTime(string _Name) {
        Name = _Name;
    }

    public static PTime StartGameTime = new PTime("游戏开始时");
    public static PTime PassBlockTime = new PTime("经过格子时");
    public static PTime MovePositionTime = new PTime("移动位置时");
    public static PTime GetMoneyTime = new PTime("获得金钱时");
    public static PTime PurchaseLandTime = new PTime("购买土地时");
    public static PTime GetHouseTime = new PTime("获得房屋时");
    public static PTime LoseMoneyTime = new PTime("失去金钱时");
    public static PTime PurchaseHouseTime = new PTime("购买房屋时");

    public class Toll {
        public static PTime AfterEmitTarget = new PTime("指定过路费的目标后");
        public static PTime AfterAcceptTarget = new PTime("成为过路费的目标后");
    }

    public class Injure {

    }
}