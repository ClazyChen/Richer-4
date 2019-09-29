using UnityEngine;
using UnityEngine.UI;
using System.Threading;

public class PMessageBox : PAbstractGroupUI<PMessage>{

    public readonly Text TitleText;
    public Thread Monitor = null;

    public PMessageBox(Transform _Background) : base(_Background) {
        InitializeControls<Text>();
        Close();
    }

    public void CreateMessages(string Title, string[] ButtonTexts, string[] ToolTips = null) {
        int ButtonNumber = ButtonTexts.Length;
        float DeltaHeight = PrototypeUI.UIBackgroundImage.GetComponent<RectTransform>().rect.height * PrototypeUI.UIBackgroundImage.GetComponent<RectTransform>().lossyScale.y;
        Vector3 CenterPoint = PrototypeUI.UIBackgroundImage.GetComponent<RectTransform>().position;
        TitleText.text = Title;
        TitleText.rectTransform.position = CenterPoint + new Vector3(0, DeltaHeight * ButtonNumber /2);
        for (int i = 0; i < ButtonNumber; ++ i) {
            AddSubUI().Initialize(ButtonTexts[i], i, ButtonNumber, CenterPoint, DeltaHeight, ToolTips == null ? string.Empty : ToolTips[i]);
        }
        Monitor = new Thread(() => {
            PMessage ChosenMessage = null;
            PThread.WaitUntil(() => {
                ChosenMessage = GroupUIList.Find((PMessage Message) => Message.IsChosen);
                if (ChosenMessage != null && !(Title.Equals("点将") && ChosenMessage.MessageText.Contains("未获得"))) {
                    return true;
                } else {
                    if (ChosenMessage != null) {
                        ChosenMessage.IsChosen = false;
                    }
                    return false;
                }
            });
            #region 点将卡和手气卡的特殊判定
            if (Title.Contains("点将卡") && ChosenMessage.Index == 0) {
                // 使用了点将卡
                PSystem.UserManager.ChooseGeneral--;
                PSystem.UserManager.Write();
            }
            if (Title.Contains("手气卡") && ChosenMessage.Index == 0) {
                // 使用了手气卡
                PSystem.UserManager.Lucky--;
                PSystem.UserManager.Write();
            }
            #endregion
            PThread.Async(() => {
                PNetworkManager.NetworkClient.Send(new PChooseResultOrder(ChosenMessage.Index.ToString()));
                PUIManager.AddNewUIAction("关闭选项框", () => {
                    Close();
                });
            });
        }) {
            IsBackground = true
        };
        Monitor.Start();
    }

    public override void Close() {
        if (Monitor != null) {
            Monitor.Abort();
            Monitor = null;
        }
        base.Close();
    }
}
