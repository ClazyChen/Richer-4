using UnityEngine;
using System.Threading;

public class PCameraController {
    private Transform Tracking = null;
    public readonly Transform Camera;
    private volatile bool IsTracking;
    private volatile bool IsChangingPerspective;
    private Thread CameraThread = null;

    public class Config {
        public static string MainCameraName = "Main Camera";

        public static Vector3 CameraLockedDistance = new Vector3(20.0f, 30.0f, 0.0f);
        public static Vector3 CameraZoomDistance = new Vector3(2.0f, 3.0f, 0.0f);
        public static int ChangePerspectiveFrameNumber = 20;
        public static float ChangePerspectiveTime = 0.2f;
        public static float CameraInterval = 0.01f;
    }

    public PCameraController() {
        Camera = GameObject.Find(Config.MainCameraName).transform;
        Close();
    }

    public void Open() {
        CameraThread = new Thread(() => {
            while (true) {
                if (IsTracking && !IsChangingPerspective) {
                    PUIManager.AddNewUIAction(string.Empty, () => {
                        Track();
                    });
                }
                PThread.Delay(Config.CameraInterval);
            }
        }) {
            IsBackground = true
        };
        CameraThread.Start();
    }

    public void Close() {
        if (CameraThread != null) {
            CameraThread.Abort();
        }
        IsTracking = false;
        IsChangingPerspective = false;
    }

    private void Track() {
        if (Tracking != null) {
            Camera.position = Tracking.position + Config.CameraLockedDistance;
        }
    }

    public void StopTracking() {
        IsTracking = false;
    }

    /// <summary>
    /// 跟踪某个玩家的棋子
    /// </summary>
    /// <param name="Player"></param>
    public void SetTracking(PPlayer Player) {
        PAnimation.AddAnimation("转换跟踪目标", () => {
            IsTracking = false;
            Tracking = PUIManager.GetUI<PMapUI>().Scene.PlayerGroup.GroupUIList[Player.Index].UIBackgroundImage;
            ChangePerspective(Tracking.position + Config.CameraLockedDistance);
            IsTracking = true;
        });
    }

    /// <summary>
    /// 移动到达目标
    /// </summary>
    /// <param name="Destination">移动到的目标（照相机中心目标）</param>
    /// <param name="Immediately">是否立即移动</param>
    public void ChangePerspective(Vector3 Destination) {
        if (!IsChangingPerspective) {
            IsChangingPerspective = true;
            PUIManager.AddNewUIAction("切换照相机视角", () => {
                Camera.position = Destination;
                IsChangingPerspective = false;
            });
        }
    }
}
