using UnityEngine;
using UnityEngine.UI;
using System;

public class PCardUI : PAbstractUI {
    public readonly PToolTipedButton HandCardButton;
    public int Index;

    public PCardUI(Transform _Background):base(_Background) {
        HandCardButton = UIBackgroundImage.GetComponent<PToolTipedButton>();
        Close();
    }

    public PCardUI Initialize(string CardName, Vector3 PrototypePosition, int _Index, int Count) {
        float Interval = 105.0f;
        float AllLength = UIBackgroundImage.parent.gameObject.GetComponent<RectTransform>().rect.width;
        if (Interval * Count > AllLength && Count > 1) {
            Interval = (AllLength - 105.0f) / (Count - 1);
        }
        Sprite Image = Resources.Load<Sprite>("Images/Cards/" + CardName);
        if (Image != null) {
            UIBackgroundImage.GetComponent<Image>().sprite = Image;
            UIBackgroundImage.localScale = new Vector3(1, 1, 1);
            UIBackgroundImage.localPosition = new Vector3(Interval * (_Index % 1000) + PrototypePosition.x, 0.0f, 0.0f);
            Index = _Index;
            HandCardButton.onClick.AddListener(() => {
                PNetworkManager.NetworkClient.Send(new PClickOnCardOrder(Index.ToString()));
            });
            HandCardButton.ToolTip = FindInstance<PCardToolTip>(CardName).ToolTip;
        }
        return this;
    }
}
