using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PGeneralUI : PAbstractUI {
    public class Config {
        public static Color GotGeneralColor = new Color(1.0f, 1.0f, 1.0f);
        public static Color NotGotGeneralColor = new Color(0.75f, 0.75f, 0.75f);
    }

    public readonly Button ReturnButton;
    public readonly Button BuyArchPointButton;
    public readonly Button BuyMoneyButton;
    public readonly InputField GeneralInfoInputField;
    public readonly PGeneralPanel GeneralPanel;

    public PGeneralUI(Transform _Background) : base(_Background) {
        InitializeControls<Button>();
        InitializeControls<InputField>();
        GeneralPanel = new PGeneralPanel(UIBackgroundImage.Find("GeneralPanel"));
        GeneralPanel.Initialize();
        Close();
    }

    public override void Open() {
        base.Open();
        #region 返回按钮：回到InitialUI
        ReturnButton.onClick.AddListener(() => {
            PUIManager.AddNewUIAction("返回：转到IUI", () => PUIManager.ChangeUI<PInitialUI>());
        });
        #endregion
        GeneralInfoInputField.text = string.Empty;
        GeneralPanel.Open();
        GeneralPanel.GroupUIList.ForEach((PGeneralButtonUI GeneralButton) => {
            if (PSystem.UserManager.GeneralList.Contains(GeneralButton.General.Name)) {
                GeneralButton.UIBackgroundImage.GetComponent<Image>().color = Config.GotGeneralColor;
            } else {
                GeneralButton.UIBackgroundImage.GetComponent<Image>().color = Config.NotGotGeneralColor;
            }
        });
    }
}
