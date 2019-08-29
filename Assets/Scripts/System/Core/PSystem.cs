using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class PSystem: MonoBehaviour {
    public static List<PMap> MapList { get; private set; }
    public static PRoom CurrentRoom = null;
    public static PMode CurrentMode = null;
    public static int PlayerIndex = 0;

    void Start() {
        PLogger.StartLogging(true);
        #region 初始化地图库
        string MapDirectory = PPath.GetPath("Data\\Maps");
        string[] MapFileNames = Directory.GetFiles(MapDirectory, "*.xml");
        MapList = new List<PMap>();
        foreach (string MapFileName in MapFileNames) {
            MapList.Add(new PMap(MapFileName));
        }
        #endregion
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            if (PUIManager.IsCurrentUI<PMapUI>()) {
                PUIManager.GetUI<PMapUI>().Space();
            }
        }

        // 鼠标滚动
        if (Input.GetAxis("Mouse ScrollWheel") < 0) {
            if (PUIManager.CurrentUI.Equals(PUIManager.GetUI<PMapUI>()) && PUIManager.GetUI<PMapUI>().CameraController != null) {
                if (PMath.InRect(Input.mousePosition, PUIManager.GetUI<PMapUI>().InformationText.rectTransform)) {
                    PUIManager.AddNewUIAction("查看下一条信息", () => {
                        PUIManager.GetUI<PMapUI>().NextInformation();
                    });
                } else {
                    PUIManager.AddNewUIAction("缩放[zoom+]", () => {
                        if (PCameraController.Config.CameraLockedDistance.magnitude < 60.0f) {
                            PCameraController.Config.CameraLockedDistance += PCameraController.Config.CameraZoomDistance;
                            PUIManager.GetUI<PMapUI>().CameraController.ChangePerspective(PUIManager.GetUI<PMapUI>().CameraController.Camera.position + PCameraController.Config.CameraZoomDistance);
                        }
                    });
                }
            }
        } else if (Input.GetAxis("Mouse ScrollWheel") > 0) {
            if (PUIManager.CurrentUI.Equals(PUIManager.GetUI<PMapUI>()) && PUIManager.GetUI<PMapUI>().CameraController != null) {
                if (PMath.InRect(Input.mousePosition, PUIManager.GetUI<PMapUI>().InformationText.rectTransform)) {
                    PUIManager.AddNewUIAction("查看上一条信息", () => {
                        PUIManager.GetUI<PMapUI>().LastInformation();
                    });
                } else {
                    PUIManager.AddNewUIAction("缩放[zoom-]", () => {
                        if (PCameraController.Config.CameraLockedDistance.magnitude > 20.0f) {
                            PCameraController.Config.CameraLockedDistance -= PCameraController.Config.CameraZoomDistance;
                            PUIManager.GetUI<PMapUI>().CameraController.ChangePerspective(PUIManager.GetUI<PMapUI>().CameraController.Camera.position - PCameraController.Config.CameraZoomDistance);
                        }
                    });
                }
            }
        }
    }
}