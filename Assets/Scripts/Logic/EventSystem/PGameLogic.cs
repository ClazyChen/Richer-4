using System.Collections.Generic;
using System.Threading;

/// <summary>
/// PGameLogic类
/// 用于设计游戏逻辑线程
/// </summary>
public class PGameLogic {

    private class Config {
        public static float ThreadWaitTime = 0.01f;
    }

    /// <summary>
    /// 用于处理一个结算的线程
    /// </summary>
    public class SettleRecord {
        public PSettle Settle;
        //public Thread ActionThread;
        public bool Finished;
        public SettleRecord(PSettle _Settle) {
            Settle = _Settle;
            Finished = false;
        }
    }

    private readonly PGame Game;
    private volatile Stack<SettleRecord> SettleRecordStack;
    public Thread LogicThread;

    /// <summary>
    /// 启动结算
    /// </summary>
    /// <param name="Name">添加的操作名</param>
    /// <param name="action">添加的操作方法</param>
    public void StartSettle(PSettle Settle) {
        SettleRecord NewSettleRecord = new SettleRecord(Settle);
        lock (SettleRecordStack) {
            SettleRecordStack.Push(NewSettleRecord);
        }
        PLogger.Log("开始结算 " + Settle.Name);
        NewSettleRecord.Settle.SettleAction(Game);
        //NewSettleRecord.ActionThread.Start();
        //PThread.WaitUntil(() => NewSettleRecord.Finished);
        PLogger.Log("终止结算 " + Settle.Name);
        //if (NewSettleRecord.ActionThread.IsAlive) {
        //    NewSettleRecord.ActionThread.Abort();
        //}
        lock (SettleRecordStack) {
            if (SettleRecordStack.Count > 0 && NewSettleRecord.Equals(SettleRecordStack.Peek())) {
                SettleRecordStack.Pop();
            }
        }
    }

    public void StartLogic(PSettle Settle) {
        (LogicThread = new Thread(() => {
            StartSettle(Settle);
        })).Start();
    }

    public PGameLogic(PGame _Game) {
        Game = _Game;
        SettleRecordStack = new Stack<SettleRecord>();
        LogicThread = null;
    }

    public void ShutDown() {
        lock (SettleRecordStack) {
            SettleRecordStack.Clear();
        }
        if (LogicThread != null && LogicThread.IsAlive) {
            LogicThread.Abort();
        }
        LogicThread = null;
    }

    public bool WaitingForEndFreeTime() {
        return SettleRecordStack.Count > 0 && SettleRecordStack.Peek().Settle.Name.Contains("触发[玩家的空闲时间点]");
    }
}
