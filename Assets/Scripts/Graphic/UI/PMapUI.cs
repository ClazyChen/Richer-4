using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// PMapUI类：
/// 用于实现地图界面
/// </summary>
/// PMapUI类本身并不负责管理3D地图的部分
public class PMapUI : PAbstractUI {

    //private class Config {
    //    public static readonly Vector3 DiceLockedDistance = new Vector3(0.0f, 80.0f, 0.0f);
    //}

    public readonly Button EndFreeTimeButton;
    public readonly Image DiceImage;
    public readonly PMapScene Scene;
    public readonly PMessageBox MessageBox;
    public readonly PPlayerInformationBoxGroup PlayerInformationGroup;
    public readonly PCameraController CameraController;
    public readonly Text InformationText;
    public readonly Text PushText;

    private readonly Sprite[] DiceSpriteList;

    public PMapUI(Transform _Background) : base(_Background) {
        InitializeControls<Button>();
        InitializeControls<Image>();
        InitializeControls<Text>();
        Scene = new PMapScene(GameObject.Find("Map").transform);
        MessageBox = new PMessageBox(UIBackgroundImage.Find("MessageBox"));
        PlayerInformationGroup = new PPlayerInformationBoxGroup(UIBackgroundImage.Find("PlayerInformationBoxes"));
        CameraController = new PCameraController();
        DiceSpriteList = new Sprite[6];
        for (int i = 0; i < 6; ++ i) {
            DiceSpriteList[i] = Resources.Load<Sprite>("Images/Dice/" + (i+1).ToString());
        }
        Close();
    }

    public override void Open() {
        InitializeMap(PNetworkManager.NetworkClient.GameStatus);
        base.Open();
        Scene.Open();
        CameraController.Open();
        PlayerInformationGroup.Open();
        InformationText.text = string.Empty;
        DiceImage.gameObject.SetActive(false);
        EndFreeTimeButton.onClick.AddListener(() => {
            PNetworkManager.NetworkClient.Send(new PEndFreeTimeOrder());
        });
        CameraController.ChangePerspective(Scene.PlayerGroup.GroupUIList[0].UIBackgroundImage.position + PCameraController.Config.CameraLockedDistance);
    }

    public override void Close() {
        CameraController.Close();
        Scene.Close();
        MessageBox.Close();
        PlayerInformationGroup.Close();
        base.Close();
    }

    public void InitializeMap(PGameStatus GameStatus) {
        Scene.InitializeMap(GameStatus.Map);
        PlayerInformationGroup.InitializeBoxes(GameStatus);
    }

    public void Ask(string Title, string[] Options) {
        MessageBox.Open();
        MessageBox.CreateMessages(Title, Options);
    }
    
    /// <summary>
    /// 被按下空格时的操作
    /// </summary>
    /// 当MessageBox被激活时，视为MessageBox选中第一项
    /// 当MessageBox未激活时，视为按下EndFreeTimeButton
    public void Space() {
        if (!MessageBox.IsActive) {
            EndFreeTimeButton.onClick.Invoke();
        } else if (MessageBox.GroupUIList.Count > 0) {
            MessageBox.GroupUIList[0].UIBackgroundImage.GetComponent<Button>().onClick.Invoke();
        }
    }

    /// <summary>
    /// 播放掷骰子的动画
    /// </summary>
    /// <param name="DiceResult">掷骰子的结果</param>
    public void Dice(int DiceResult) {
        PAnimation.AddAnimation("掷骰子", () => {
            int DiceMiddleResult = PRandom.RandInt(1, 6);
            DiceImage.sprite = DiceSpriteList[DiceMiddleResult - 1];
        }, PAnimation.DiceAnimation.FrameNumber, PAnimation.DiceAnimation.TotalTime, () => {
            PUIManager.AddNewUIAction("掷骰子-显示最终结果", () => {
                DiceImage.sprite = DiceSpriteList[DiceResult - 1];
            });
        }, () => {
            DiceImage.gameObject.SetActive(true);
        });
    }
}
