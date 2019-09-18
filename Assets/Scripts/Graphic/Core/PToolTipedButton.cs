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
            if (Input.mousePosition.x > Screen.width - Th) {
                PUIManager.GetUI<PMapUI>().ToolTip.Show(ToolTip, new Vector3(Input.mousePosition.x - Th - 10, Input.mousePosition.y - 10, 0));
            } else {
                PUIManager.GetUI<PMapUI>().ToolTip.Show(ToolTip, new Vector3(Input.mousePosition.x + 10, Input.mousePosition.y - 10, 0));
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