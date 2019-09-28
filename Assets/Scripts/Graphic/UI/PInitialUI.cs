using UnityEngine;
using UnityEngine.UI;

public class PInitialUI : PAbstractUI {
    public readonly Button CreateGameButton;
    public readonly Button JoinGameButton;
    public readonly Button GeneralButton;
    public readonly Button ArchievementButton;
    public readonly Button UserButton;

    public PInitialUI(Transform _Background) : base(_Background) {
        InitializeControls<Button>();
        Close();
    }

    public override void Open() {
        base.Open();
        #region 创建游戏：跳转到ChooseMapUI
        CreateGameButton.onClick.AddListener(() => {
            PUIManager.AddNewUIAction("创建游戏：转到CMUI", () => PUIManager.ChangeUI<PChooseMapUI>());
        });
        #endregion
        #region 加入游戏：跳转到JoinUI
        JoinGameButton.onClick.AddListener(() => {
            PUIManager.AddNewUIAction("加入游戏：转到JUI", () => PUIManager.ChangeUI<PJoinUI>());
        });
        #endregion
        #region 成就按钮：跳转到ArchUI
        ArchievementButton.onClick.AddListener(() => {
            PUIManager.AddNewUIAction("成就：转到AUI", () => PUIManager.ChangeUI<PArchUI>());
        });
        #endregion
        #region 用户按钮：跳转到UserUI
        UserButton.onClick.AddListener(() => {
            PUIManager.AddNewUIAction("成就：转到UUI", () => PUIManager.ChangeUI<PUserUI>());
        });
        #endregion
    }
}
