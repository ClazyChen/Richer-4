/// <summary>
/// PTime：
/// 用来表示发动时机的类
/// </summary>
public class PTime : PObject {
    public PTime(string _Name) {
        Name = _Name;
    }

    public static PTime StartGameTime = new PTime("游戏开始时");
}