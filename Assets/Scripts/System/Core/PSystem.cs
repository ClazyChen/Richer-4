using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class PSystem: MonoBehaviour {
    public static List<PMap> MapList { get; private set; }
    public static PRoom CurrentRoom = null;
    public static PMode CurrentMode = null;
    public static int PlayerIndex = 0;
    public static PArchManager ArchManager = null;

    public static Vector3 LastMousePosition;
    public static bool MouseRightButtonDown = false;
    public class Config {
        public static float MouseSensitivity = 0.1f;
    }

    void Start() {
        PLogger.StartLogging(true);
        ArchManager = new PArchManager();
        ArchManager.Read();
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

        if (Input.GetMouseButtonDown(1)) {
            MouseRightButtonDown = true;
            if (PUIManager.CurrentUI.Equals(PUIManager.GetUI<PMapUI>())) {
                PUIManager.AddNewUIAction("启动右键拖拽", () => {
                    PUIManager.GetUI<PMapUI>().CameraController.StopTracking();
                    LastMousePosition = Input.mousePosition;
                });
            }
        }
        if (Input.GetMouseButtonUp(1)) {
            MouseRightButtonDown = false;
        }
        if (MouseRightButtonDown) {
            if (PUIManager.CurrentUI.Equals(PUIManager.GetUI<PMapUI>())) {
                PUIManager.AddNewUIAction(string.Empty, () => {
                    Vector3 MouseDirection = Input.mousePosition - LastMousePosition;
                    LastMousePosition = Input.mousePosition;
                    Vector3 MoveDirection = new Vector3(MouseDirection.y, 0, -MouseDirection.x) * Config.MouseSensitivity;
                    PUIManager.GetUI<PMapUI>().CameraController.ChangePerspective(PUIManager.GetUI<PMapUI>().CameraController.Camera.position + MoveDirection);
                });
            }
        }

        #region 鼠标滚动
        if (Input.GetAxis("Mouse ScrollWheel") < 0) {
            if (PUIManager.CurrentUI.Equals(PUIManager.GetUI<PMapUI>()) && PUIManager.GetUI<PMapUI>().CameraController != null) {
                if (PMath.InRect(Input.mousePosition, PUIManager.GetUI<PMapUI>().InformationText.rectTransform)) {
                    PUIManager.AddNewUIAction("查看下一条信息", () => {
                        PUIManager.GetUI<PMapUI>().NextInformation();
                    });
                } else {
                    PUIManager.AddNewUIAction("缩放[zoom+]", () => {
                        if (PCameraController.Config.CameraLockedDistance.magnitude < 80.0f) {
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
        #endregion

        #region 选择格子
        if (Input.GetMouseButtonUp(0)) {
            if (PUIManager.CurrentUI.Equals(PUIManager.GetUI<PMapUI>()) && !PUIManager.GetUI<PMapUI>().MessageBox.IsActive) {
                Vector3 WorldPosition1 = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 20.0f));
                Vector3 WorldPosition2 = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 30.0f));
                Vector3 WorldPosition = new Vector3((WorldPosition1.x - WorldPosition2.x) / (WorldPosition1.y - WorldPosition2.y) * (-WorldPosition1.y) + WorldPosition1.x, 0, (WorldPosition1.z - WorldPosition2.z) / (WorldPosition1.y - WorldPosition2.y) * (-WorldPosition1.y) + WorldPosition1.z);
                PBlock FindBlockResult = PNetworkManager.NetworkClient.GameStatus.Map.FindBlock(PUIManager.GetUI<PMapUI>().Scene.BlockGroup.FindBlockSceneIndex(WorldPosition));
                if (FindBlockResult != null) {
                    PNetworkManager.NetworkClient.Send(new PClickOnBlockOrder(FindBlockResult.Index.ToString()));
                }
            }
        }
        #endregion
    }
}