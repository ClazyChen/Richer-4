using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PToolTipedButton : Button {
    [SerializeField]
    public string ToolTip = string.Empty;

    public override void OnPointerEnter(PointerEventData eventData) {
        if (ToolTip == null || ToolTip.Length == 0) {
            base.OnPointerEnter(eventData);
            return;
        }
        PUIManager.AddNewUIAction(string.Empty, () => {
            PUIManager.GetUI<PMapUI>().ToolTip.Show(ToolTip, new Vector3(Input.mousePosition.x + 10,
            Input.mousePosition.y - 10, 0));
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