using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PToolTipedButton : Button {
    public string ToolTip = string.Empty;

    public override void OnPointerEnter(PointerEventData eventData) {
        if (ToolTip == null || ToolTip.Length == 0) {
            base.OnPointerEnter(eventData);
            return;
        }
        PUIManager.AddNewUIAction(string.Empty, () => {
            float Th = 200 * PUIManager.GetUI<PMapUI>().ToolTip.UIBackgroundImage.GetComponent<RectTransform>().lossyScale.x;
            float PosX = 0.0f;
            if (Input.mousePosition.x > Screen.width - Th) {
                PosX = Input.mousePosition.x - Th - 10;
            } else {
                PosX = Input.mousePosition.x + 10;
            }
            PUIManager.GetUI<PMapUI>().ToolTip.Show(ToolTip, new Vector3(PosX, Input.mousePosition.y - 10, 0));
            float HhtTh = PUIManager.GetUI<PMapUI>().ToolTip.UIBackgroundImage.GetComponent<RectTransform>().rect.height * PUIManager.GetUI<PMapUI>().ToolTip.UIBackgroundImage.GetComponent<RectTransform>().lossyScale.y;
            float PosY = PUIManager.GetUI<PMapUI>().ToolTip.UIBackgroundImage.GetComponent<RectTransform>().position.y;
            if (Input.mousePosition.y > Screen.height - HhtTh - 10) {
                PosY -= HhtTh + 10;
                PUIManager.GetUI<PMapUI>().ToolTip.Show(ToolTip, new Vector3(PosX, PosY, 0));
            }
        });
    }

    public override void OnPointerExit(PointerEventData eventData) {
        if (ToolTip == null || ToolTip.Length == 0) {
            base.OnPointerExit(eventData);
            return;
        }
        PUIManager.AddNewUIAction(string.Empty, () => {
            if (PUIManager.GetUI<PMapUI>().ToolTip.ToolTipText.text.Equals(ToolTip)) {
                PUIManager.GetUI<PMapUI>().ToolTip.Close();
            }
        });
    }
}