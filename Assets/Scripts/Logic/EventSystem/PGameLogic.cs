using System.Collections.Generic;
using System;
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
    public class SettleThread {
        public PSettle Settle;
        public Thread ActionThread;
        public bool Finished;
        public SettleThread(PSettle _Settle) {
            Settle = _Settle;
            Finished = false;
            ActionThread = new Thread(() => {
                Settle.SettleAction();
                Finished = true;
            }) {
                IsBackground = true
            };
        }
    }

    private Stack<SettleThread> SettleThreadStack;

    /// <summary>
    /// 启动结算
    /// </summary>
    /// <param name="Name">添加的操作名</param>
    /// <param name="action">添加的操作方法</param>
    public void StartSettle(PSettle Settle) {
        SettleThread NewSettleThread = new SettleThread(Settle);
        SettleThreadStack.Push(NewSettleThread);
        PLogger.Log("开始结算 " + Settle.Name);
        NewSettleThread.ActionThread.Start();
        PThread.WaitUntil(() => NewSettleThread.Finished);
        PLogger.Log("终止结算 " + Settle.Name);
        if (NewSettleThread.ActionThread.IsAlive) {
            NewSettleThread.ActionThread.Abort();
        }
        if (NewSettleThread.Equals(SettleThreadStack.Peek())) {
            SettleThreadStack.Pop();
        }
    }

    public PGameLogic() {
        SettleThreadStack = new Stack<SettleThread>();
    }

    public void ShutDown() {
        if (SettleThreadStack != null) {
            while (SettleThreadStack.Count > 0) {
                Thread Top = SettleThreadStack.Peek().ActionThread;
                if (Top != null && Top.IsAlive) {
                    Top.Abort();
                }
                SettleThreadStack.Pop();
            }
        }
    }
}
