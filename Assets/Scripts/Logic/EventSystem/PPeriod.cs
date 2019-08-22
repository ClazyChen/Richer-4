
public class PPeriod : PObject {
    public readonly PTime Before;
    public readonly PTime Start;
    public readonly PTime During;
    public readonly PTime End;
    public readonly PTime After;
    public readonly PTime Next;

    public PPeriod(string _Name) {
        Name = _Name;
        Before = new PTime(Name + "开始前");
        Start = new PTime(Name + "开始时");
        During = new PTime(Name);
        End = new PTime(Name + "结束时");
        After = new PTime(Name + "结束后");
        Next = new PTime(Name + "的下一个时期");
    }

    public PSettle Execute() {
        return new PSettle(Name + "结算", (PGame Game) => {
            Game.Monitor.CallTime(Before);
            Game.Monitor.CallTime(Start);
            Game.Monitor.CallTime(During);
            Game.Monitor.CallTime(End);
            Game.Monitor.CallTime(After);
            Game.Monitor.CallTime(Next);
        });
    }

    public bool IsFreeTime() {
        return Equals(FirstFreeTime) || Equals(SecondFreeTime);
    }

    public static PPeriod StartTurn = new PPeriod("回合开始时");
    public static PPeriod PreparationStage = new PPeriod("准备阶段");
    public static PPeriod JudgeStage = new PPeriod("判定阶段");
    public static PPeriod FirstFreeTime = new PPeriod("第一个空闲时间点");
    public static PPeriod DiceStage = new PPeriod("掷骰阶段");
    public static PPeriod WalkingStage = new PPeriod("行走阶段");
    public static PPeriod SettleStage = new PPeriod("结算阶段");
    public static PPeriod SecondFreeTime = new PPeriod("第二个空闲时间点");
    public static PPeriod EndTurnStage = new PPeriod("回合结束阶段");
    public static PPeriod EndTurn = new PPeriod("回合结束时");

}