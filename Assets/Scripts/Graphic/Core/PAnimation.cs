using UnityEngine.UI;
using UnityEngine;
using System;

public class PAnimation {
    public static int IDCount { get; private set; } = 1;

    public float TotalTime;
    public int FrameNumber;
    public int ID;
    private PAnimation(float Time, int Frame) {
        TotalTime = Time;
        FrameNumber = Frame;
        ID = IDCount++;
    }
    public static PAnimation DiceAnimation = new PAnimation(0.5f, 20);
    public static PAnimation ChangePerspectiveAnimation = new PAnimation(0.2f, 20);
    public static PAnimation MovePlayerAnimation = new PAnimation(0.4f, 20);
    public static PAnimation PushInformationAnimation = new PAnimation(0.5f, 25);

    public static void AddAnimation(string Name, Action Animation, int FrameNumber = 1, float TotalTime = 0.0f, Action Callback = null, Action Preparation = null) {
        PThread.Async(() => {
            int ID = IDCount++;
            PUIManager.RegisterAnimation(ID);
            // PLogger.Log("注册动画 " + Name + " #" + ID);
            PThread.WaitUntil(() => PUIManager.IsAvailable(ID));
            // PLogger.Log("开始动画 " + Name + " #" + ID);
            PUIManager.AddNewUIAction(string.Empty, () => {
                Preparation?.Invoke();
            }, ID);
            PThread.Repeat(() => {
                PUIManager.AddNewUIAction(string.Empty, Animation, ID);
            }, FrameNumber, TotalTime);
            PUIManager.AddNewUIAction(string.Empty, () => { }, ID, true);
            // PLogger.Log("结束动画 " + Name + " #" + ID);
            Callback?.Invoke();
        });
    }

    /// <summary>
    /// 创建一个直线移动动画
    /// </summary>
    /// <param name="transform">直线移动的物体</param>
    /// <param name="Destination">终止位置</param>
    public static void MoveAnimation(Transform transform, Vector3 Destination, int FrameNumber, float TotalTime, Action Callback = null, string ObjectName = null) {
        int RemainFrameNumber = FrameNumber;
        AddAnimation(ObjectName == null ? "移动物体" : "移动" + ObjectName, () => {
            Vector3 MoveSpeed = (Destination - transform.position) / (RemainFrameNumber--);
            Vector3 NextPosition = transform.position + MoveSpeed;
            if (Vector3.Dot(Destination - NextPosition, Destination - transform.position) < 0.0f) {
                transform.position = Destination;
            } else {
                transform.position = NextPosition;
            }
        }, FrameNumber, TotalTime, Callback);
    }

    /// <summary>
    /// 创建一个推送动画
    /// </summary>
    /// <param name="StartPoint"></param>
    public static void PushAnimation(PPlayer Player, string PushText, PPushType PushType) {
        const int FrameNumber = 25;
        const float TotalTime = 0.5f;
        float Offset = 100.0f;
        Vector3 Speed = new Vector3(0.0f, Offset / FrameNumber, 0.0f);
        Text NewPush = null;
        Color PushColor = PushType.PushColor;
        AddAnimation("推送[" + PushText + "]", () => {
            PushColor.a -= 0.8f / FrameNumber;
            NewPush.color = PushColor;
            NewPush.rectTransform.Translate(Speed);
        }, FrameNumber, TotalTime, () => {
            PUIManager.AddNewUIAction("删除推送[" + PushText + "]", () => {
                UnityEngine.Object.Destroy(NewPush.gameObject);
            });
        }, () => {
            NewPush = UnityEngine.Object.Instantiate(PUIManager.GetUI<PMapUI>().PushText);
            NewPush.transform.SetParent(PUIManager.GetUI<PMapUI>().UIBackgroundImage);
            NewPush.rectTransform.localScale *= PUIManager.GetUI<PMapUI>().PushText.rectTransform.lossyScale.y;
            NewPush.color = PushColor;
            NewPush.text = PushText;
            NewPush.rectTransform.position = PPlayerScene.GetScreenPosition(Player);
            NewPush.gameObject.SetActive(true);
        });
    }
}