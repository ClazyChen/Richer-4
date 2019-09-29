using System;
using UnityEngine;
using UnityEngine.UI;

public class PChooseMapUI : PAbstractUI {
    public readonly Dropdown ChooseMapDropdown;
    public readonly Dropdown ChooseModeDropdown;
    public readonly Button EnterButton;
    public readonly Button ReturnButton;
    public readonly Button TestButton;

    public PChooseMapUI(Transform _Background) : base(_Background) {
        InitializeControls<Button>();
        InitializeControls<Dropdown>();
        Close();
    }

    public override void Open() {
        base.Open();
        #region 初始化选择地图下拉框
        PSystem.MapList.ForEach((PMap Map) => {
            ChooseMapDropdown.options.Add(new Dropdown.OptionData { text = Map.Name });
        });
        #endregion
        #region 初始化选择模式下拉框
        PMode.ListModes().ForEach((PMode Mode) => {
            ChooseModeDropdown.options.Add(new Dropdown.OptionData { text = Mode.Name });
        });
        #endregion
        ResetDropdowns();
        #region 返回按钮：回到InitialUI
        ReturnButton.onClick.AddListener(() => {
            PUIManager.AddNewUIAction("返回：转到IUI", () => PUIManager.ChangeUI<PInitialUI>());
        });
        #endregion
        #region 确定按钮：创建服务器
        EnterButton.onClick.AddListener(() => {
            int MapValue = ChooseMapDropdown.value;
            int ModeValue = ChooseModeDropdown.value;
            PSystem.AllAi = false;
            try {
                PNetworkManager.CreateServer(PSystem.MapList[MapValue], PMode.ListModes().Find((PMode Mode) => Mode.Name.Equals(ChooseModeDropdown.options[ModeValue].text)));
            } catch (Exception e) {
                PLogger.Log("建立服务器错误");
                PLogger.Log(e.ToString());
            }
            // 等待客户端接收到信息
        });
        #endregion
        #region 测试按钮：开启AI测试界面
        TestButton.onClick.AddListener(() => {
            int MapValue = ChooseMapDropdown.value;
            int ModeValue = ChooseModeDropdown.value;
            PSystem.AllAi = true;
            try {
                PNetworkManager.CreateServer(PSystem.MapList[MapValue], PMode.ListModes().Find((PMode Mode) => Mode.Name.Equals(ChooseModeDropdown.options[ModeValue].text)));
            } catch (Exception e) {
                PLogger.Log("建立服务器错误");
                PLogger.Log(e.ToString());
            }
            //int MapValue = ChooseMapDropdown.value;
            //int ModeValue = ChooseModeDropdown.value;
            //PSystem.AllAiConfig = string.Empty;
            //PSystem.CurrentMode = PMode.ListModes().Find((PMode Mode) => Mode.Name.Equals(ChooseModeDropdown.options[ModeValue].text));
            //PSystem.CurrentMap = PSystem.MapList[MapValue];
            //PUIManager.AddNewUIAction("测试：转到TUI", () => PUIManager.ChangeUI<PTestUI>());
        });
        #endregion
    }
}