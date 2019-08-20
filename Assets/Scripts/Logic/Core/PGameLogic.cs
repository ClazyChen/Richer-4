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
        public string SettleName;
        public Thread ActionThread;
        public bool Finished;
        public SettleThread(string Name, Action action) {
            SettleName = Name;
            Finished = false;
            ActionThread = new Thread(() => {
                action();
                Finished = true;
            }) {
                IsBackground = true
            };
        }
    }

    private Thread GameLogicThread = null;
    private Queue<Action> GameLogicActionQueue;
    public bool GameLogicThreadStopFlag = false;
    private Stack<SettleThread> SettleThreadStack;

    /*
     * 游戏逻辑线程（GLT）内处理游戏逻辑，由PGame打开
     * 游戏逻辑分成两种，一种是顺序进行的（回合流程），一种是回调进行的（结算流程）
     * 顺序流程：加入流程队列，在之前的流程进行完之后触发
     * 回调流程：打断当前流程（阻塞）并立刻启动，结束后回到被打断的流程
     * 分别放在Queue和Stack中
     */

    /// <summary>
    /// 在GLT队列中加入顺序流程
    /// </summary>
    /// <param name="Name">添加的操作名</param>
    /// <param name="action">添加的操作方法</param>
    public void EnqueueAction(string Name, Action action) {
        if (GameLogicActionQueue != null) {
            PLogger.Log("新建顺序流程：" + Name);
            lock (GameLogicActionQueue) {
                GameLogicActionQueue.Enqueue(action);
            }
        }
    }

    /// <summary>
    /// 在STS中加入结算流程
    /// </summary>
    /// <param name="Name">添加的操作名</param>
    /// <param name="action">添加的操作方法</param>
    public void NewSettle(string Name, Action action) {
        SettleThread NewSettleThread = new SettleThread(Name, action);
        SettleThreadStack.Push(NewSettleThread);
        PLogger.Log("开始结算 " + Name);
        NewSettleThread.ActionThread.Start();
        PThread.WaitUntil(() => NewSettleThread.Finished);
        PLogger.Log("终止结算 " + Name);
        if (NewSettleThread.ActionThread.IsAlive) {
            NewSettleThread.ActionThread.Abort();
        }
        if (NewSettleThread.Equals(SettleThreadStack.Peek())) {
            SettleThreadStack.Pop();
        }
    }

    public PGameLogic() {
        GameLogicActionQueue = new Queue<Action>();
        SettleThreadStack = new Stack<SettleThread>();
    }

    /// <summary>
    /// 启动GLT
    /// </summary>
    public void StartGameLogicThread() {
        GameLogicActionQueue.Clear();
        SettleThreadStack.Clear();
        GameLogicThreadStopFlag = false;
        (GameLogicThread = new Thread(() => {
            while (true) {
                if (!GameLogicThreadStopFlag && GameLogicActionQueue.Count > 0) {
                    GameLogicActionQueue.Dequeue()();
                }
                PThread.Delay(Config.ThreadWaitTime);
            }
        }) {
            Priority = ThreadPriority.BelowNormal,
            IsBackground = true
        }).Start();
    }

    /// <summary>
    /// 在结算线程栈清空后重启GLT
    /// 用于【当前结算完毕后终止回合】
    /// 会阻塞当前线程
    /// </summary>
    public void RestartGameLogicThreadAfterSettle() {
        GameLogicThreadStopFlag = true;
        PThread.WaitUntil(() => SettleThreadStack.Count == 0);
        RestartGameLogicThread();
    }

    /// <summary>
    /// 重新启动GLT（中止当前所有结算）
    /// 可用于【立刻中止回合】的情形
    /// </summary>
    public void RestartGameLogicThread() {
        ShutDownGameLogicThread();
        StartGameLogicThread();
    }

    /// <summary>
    /// 关闭GLT
    /// </summary>
    public void ShutDownGameLogicThread() {
        GameLogicThreadStopFlag = true;
        #region 清空结算线程栈
        if (SettleThreadStack != null) {
            while (SettleThreadStack.Count > 0) {
                Thread Top = SettleThreadStack.Peek().ActionThread;
                if (Top != null && Top.IsAlive) {
                    Top.Abort();
                }
                SettleThreadStack.Pop();
            }
        }
        #endregion
        if (GameLogicThread != null && GameLogicThread.IsAlive) {
            GameLogicThread.Abort();
        }
        if (GameLogicActionQueue != null) {
            GameLogicActionQueue.Clear();
        }
    }

    public bool SingleSettle() {
        return SettleThreadStack.Count == 1;
    }
}
