using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

/// <summary>
/// PUIManager类：
/// 负责UI控制的顶层类
/// </summary>
public class PUIManager: MonoBehaviour {
    public class NamedAction {
        public string Name;
        public int ID;
        public Action Action;
        public int AnimationID;

        public override string ToString() {
            return Name + " #" + ID.ToString();
        }
    }

    private static int CurrentAnimationID = 0;
    private static List<int> WaitingAnimation = new List<int>();
    private static int ActionIDCount = 0;
    /// <summary>
    /// 其他线程发送给主线程的UI操作等待队列
    /// </summary>
    public static Queue<NamedAction> ActionWaitingList { get; private set; } = new Queue<NamedAction>();

    public static List<PAbstractUI> UIList;
    public static void ChangeUI<T>() where T : PAbstractUI {
        UIList.ForEach((PAbstractUI UI) => { UI.Close(); });
        GetUI<T>().Open();
    }
    public static T GetUI<T>() where T : PAbstractUI {
        return (T)UIList.Find((PAbstractUI UI) => UI.Name.Equals(typeof(T).Name.Substring(1)));
    }
    public static PAbstractUI CurrentUI {
        get {
            return UIList.Find((PAbstractUI UI) => UI.IsActive);
        }
    }
    public static bool IsCurrentUI<T>() {
        return CurrentUI != null && CurrentUI.GetType().Equals(typeof(T));
    }


    /// <summary>
    /// 往UI操作队列中加入新的操作
    /// </summary>
    /// <param name="ActionName">操作名（记录在日志里）</param>
    /// <param name="UIAction">操作内容</param>
    /// <param name="AnimationID">当前播放动画的ID</param>
    /// <param name="AnimationEnding">动画播放是否结束</param>
    public static void AddNewUIAction(string ActionName, Action UIAction, int AnimationID = 0, bool AnimationEnding = false) {
        lock (ActionWaitingList) {
            if (!ActionName.Equals(string.Empty)) {
                PLogger.Log("创建操作 " + ActionName + " #" + ActionIDCount.ToString());
            }
            ActionWaitingList.Enqueue(new NamedAction() {
                Name = ActionName,
                ID = ActionIDCount++,
                AnimationID = AnimationID,
                Action = () => {
                    if (AnimationID > 0) {
                        CurrentAnimationID = AnimationID;
                    }
                    UIAction();
                    if (AnimationID == CurrentAnimationID && AnimationEnding) {
                        //PLogger.Log("结束动画(真) " + ActionName + " #" + AnimationID);
                        //PLogger.Log("Waiting List:");
                        //WaitingAnimation.ForEach((int x) => PLogger.Log(x.ToString()));
                        CurrentAnimationID ++;
                    }
                }
            });
        }
    }

    public static void RegisterAnimation(int ID) {
        //lock (WaitingAnimation) {
        //    WaitingAnimation.Add(ID);
        //    WaitingAnimation.Sort();
        //}
    }

    public static bool IsAvailable(int ID) {
        return CurrentAnimationID == ID || CurrentAnimationID == 0;
       // return CurrentAnimationID == 0 && WaitingAnimation.Count > 0 && WaitingAnimation[0] == ID; 
    }

    void Start() {
        #region 从Canvas中的UI层建立UI列表
        UIList = new List<PAbstractUI>();
        Transform Canvas = GameObject.Find("Canvas").transform;
        for (int i = 0; i < Canvas.childCount; ++i) {
            Transform Child = Canvas.GetChild(i);
            if (Child.name.EndsWith("UI")) {
                Type UIClassType = Type.GetType("P" + Child.name);
                if (UIClassType != null) {
                    try {
                        PAbstractUI UIInstance = (PAbstractUI)Activator.CreateInstance(UIClassType, BindingFlags.Default, null, new object[] { Child }, null);
                        UIInstance.Name = Child.name;
                        UIList.Add(UIInstance);
                    } catch (Exception e) {
                        PLogger.Log("PUIManager - UI名格式错误：" + Child.name);
                        Debug.Log(e.ToString());
                    }
                }
            }
        }
        #endregion
        ChangeUI<PInitialUI>();
    }

    void Update() {
        lock (ActionWaitingList) {
            Queue<NamedAction> TempQueue = new Queue<NamedAction>();
            while (ActionWaitingList.Count > 0) {
                NamedAction CurrentAction = ActionWaitingList.Dequeue();
                try {
                    if (CurrentAction != null) {
                        if (CurrentAction.AnimationID > 0) {
                            if (CurrentAnimationID > 0 && CurrentAnimationID != CurrentAction.AnimationID) {
                                TempQueue.Enqueue(CurrentAction);
                                continue;
                            }
                            //} else {
                            //    if (CurrentAnimationID == 0) {
                            //        WaitingAnimation.Remove(CurrentAction.AnimationID);
                            //    }
                            //    CurrentAnimationID = CurrentAction.AnimationID;
                            //}
                        }
                        if (!CurrentAction.Name.Equals(string.Empty)) {
                            PLogger.Log("执行操作 " + CurrentAction.ToString());
                        }
                        bool ActionCompleted = false;
                        PThread.Async(() => {
                            PThread.Delay(0.5f);
                            if (!ActionCompleted) {
                                PLogger.Log("操作异常：" + CurrentAction.ToString());
                                throw new TimeoutException("UI操作超时");
                            }
                        });
                        CurrentAction.Action();
                        ActionCompleted = true;
                    }
                } catch (Exception e) {
                    PLogger.Log("操作 " + CurrentAction.ToString() + " 发生错误");
                    PLogger.Log(e.ToString());
                }
            }
            while (TempQueue.Count > 0) {
                ActionWaitingList.Enqueue(TempQueue.Dequeue());
            }
        }
    }
}
