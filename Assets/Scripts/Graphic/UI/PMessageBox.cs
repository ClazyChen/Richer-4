﻿using UnityEngine;
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
            PThread.WaitUntil(() => (ChosenMessage = GroupUIList.Find((PMessage Message) => Message.IsChosen)) != null);
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
