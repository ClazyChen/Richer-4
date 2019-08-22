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
}