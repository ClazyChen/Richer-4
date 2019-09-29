using UnityEngine;
using UnityEngine.UI;
using System;

public class PToolTip : PAbstractUI {
    public readonly Text ToolTipText;
    private TextGenerator GeneratorForLayout;

    public PToolTip(Transform _Background):base(_Background) {
        InitializeControls<Text>();
        GeneratorForLayout = new TextGenerator();
        Close();
    }

    public void Show(string _ToolTip, Vector3 Position) {
        Open();
        UIBackgroundImage.GetComponent<RectTransform>().position = Position;
        ToolTipText.text = _ToolTip;
        UIBackgroundImage.GetComponent<RectTransform>().sizeDelta = new Vector2(200.0f, GeneratorForLayout.GetPreferredHeight(_ToolTip, ToolTipText.GetGenerationSettings(new Vector2(ToolTipText.GetPixelAdjustedRect().size.x, 0.0f))) / ToolTipText.pixelsPerUnit);
    }
}
