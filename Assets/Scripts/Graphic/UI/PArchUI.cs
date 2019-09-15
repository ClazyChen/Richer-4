using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PArchUI : PAbstractUI {
    public class Config {
        public static Color GotArchievementColor = new Color(1.0f, 1.0f, 1.0f);
        public static Color NotGotArchievementColor = new Color(0.75f, 0.75f, 0.75f);
    }

    public readonly Button ReturnButton;
    public readonly InputField ArchInfoInputField;
    public readonly PArchPanel ArchPanel;

    public PArchUI(Transform _Background) : base(_Background) {
        InitializeControls<Button>();
        InitializeControls<InputField>();
        ArchPanel = new PArchPanel(UIBackgroundImage.Find("ArchPanel"));
        ArchPanel.Initialize();
        Close();
    }

    public override void Open() {
        base.Open();
        #region 返回按钮：回到InitialUI
        ReturnButton.onClick.AddListener(() => {
            PUIManager.AddNewUIAction("返回：转到IUI", () => PUIManager.ChangeUI<PInitialUI>());
        });
        #endregion
        ArchInfoInputField.text = string.Empty;
        ArchPanel.Open();
        ArchPanel.GroupUIList.ForEach((PArchButtonUI ArchButton) => {
            if (PSystem.ArchManager.ArchList.Exists((string x ) => x .Equals(ArchButton.ArchInfo.Name))) {
                ArchButton.UIBackgroundImage.GetComponent<Image>().color = Config.GotArchievementColor;
            } else {
                ArchButton.UIBackgroundImage.GetComponent<Image>().color = Config.NotGotArchievementColor;
            }
        });
    }
}
