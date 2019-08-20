using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class PJoinUI : PAbstractUI {
    public readonly Button JoinGameButton;
    public readonly Button ReturnButton;
    public readonly InputField ServerIPInputField;
    public readonly InputField NicknameInputField;
    public readonly Text ErrorText;

    private const string IPAddressPattern = "((2[0-4]\\d|25[0-5]|[01]?\\d\\d?)\\.){3}(2[0-4]\\d|25[0-5]|[01]?\\d\\d?)";

    public PJoinUI(Transform _Background) : base(_Background) {
        InitializeControls<Button>();
        InitializeControls<InputField>();
        InitializeControls<Text>();
        Close();
    }

    public override void Close() {
        base.Close();
        ErrorText.text = string.Empty;
    }

    public override void Open() {
        base.Open();
        #region 返回按钮：回到InitialUI
        ReturnButton.onClick.AddListener(() => {
            PUIManager.AddNewUIAction("返回：转到IUI", () => PUIManager.ChangeUI<PInitialUI>());
        });
        #endregion
        #region 加入游戏按钮：向服务器发送请求
        JoinGameButton.onClick.AddListener(() => {
            string ServerIP = ServerIPInputField.text;
            string Nickname = NicknameInputField.text;
            if (!Regex.IsMatch(ServerIP, IPAddressPattern)) {
                ErrorText.text = "IP地址不正确";
            } else if (Nickname.Equals(string.Empty) || Nickname.Length > 8 || Nickname.Contains(" ")) {
                ErrorText.text = "昵称须为1~8个字且不能有空格";
            } else {
                if (PNetworkManager.CreateClient(ServerIP, Nickname)) {
                    ErrorText.text = string.Empty;
                    // 不进行任何操作，等待服务器的回应
                } else {
                    ErrorText.text = "网络连接失败";
                }
            }
        });
        #endregion
    }
}
