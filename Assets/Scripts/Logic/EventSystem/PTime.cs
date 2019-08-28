/// <summary>
/// PTime：
/// 用来表示发动时机的类
/// </summary>
public class PTime : PObject {
    public PTime(string _Name) {
        Name = _Name;
    }

    public bool IsPeroidTime() {
        foreach (PPeriod Peroid in PPeriodTriggerInstaller.TurnFlow) {
            if (Equals(Peroid.Before) || Equals(Peroid.During) || Equals(Peroid.End) || Equals(Peroid.After) || Equals(Peroid.Start)) {
                return true;
            }
        }
        return false;
    }

    public static PTime StartGameTime = new PTime("游戏开始时");
    public static PTime PassBlockTime = new PTime("经过格子时");
    public static PTime MovePositionTime = new PTime("移动位置时");
    public static PTime GetMoneyTime = new PTime("获得金钱时");
    public static PTime PurchaseLandTime = new PTime("购买土地时");
    public static PTime GetHouseTime = new PTime("获得房屋时");
    public static PTime LoseMoneyTime = new PTime("失去金钱时");
    public static PTime PurchaseHouseTime = new PTime("购买房屋时");

    public static PTime EnterDyingTime = new PTime("进入濒死状态时");
    public static PTime LeaveDyingTime = new PTime("离开濒死状态时");
    public static PTime DieTime = new PTime("死亡时");
    public static PTime AfterDieTime = new PTime("死亡后");

    public class Toll {
        public static PTime AfterEmitTarget = new PTime("指定过路费的目标后");
        public static PTime AfterAcceptTarget = new PTime("成为过路费的目标后");
    }

    public class Injure {
        public static PTime StartSettle = new PTime("伤害结算开始时");
        public static PTime BeforeEmitInjure = new PTime("即将造成伤害时");
        public static PTime BeforeAcceptInjure = new PTime("即将受到伤害时");
        public static PTime EmitInjure = new PTime("造成伤害时");
        public static PTime AcceptInjure = new PTime("受到伤害时");
        public static PTime AfterEmitInjure = new PTime("造成伤害后");
        public static PTime AfterAcceptInjure = new PTime("受到伤害后");
        public static PTime EndSettle = new PTime("伤害结算结束时");
    }
}