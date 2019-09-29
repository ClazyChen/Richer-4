using UnityEngine;
using UnityEngine.UI;
using System;

public class PArchButtonUI : PAbstractUI {
    public readonly Button ArchButton;
    public PArchInfo ArchInfo;
    public int Index;

    public PArchButtonUI(Transform _Background):base(_Background) {
        ArchButton = UIBackgroundImage.GetComponent<Button>();
        Close();
    }

    public PArchButtonUI Initialize(Transform Prototype, int _Index, int LineCapacity, PArchInfo _ArchInfo) {
        Index = _Index;
        ArchInfo = _ArchInfo;
        UIBackgroundImage.GetComponentInChildren<Text>().text = ArchInfo.Name;
        UIBackgroundImage.localScale = new Vector3(1, 1, 1);
        UIBackgroundImage.localPosition = new Vector3(70.0f * (Index % LineCapacity) + Prototype.localPosition.x, -70.0f * (Index / LineCapacity) + Prototype.localPosition.y, 0.0f);
        ArchButton.onClick.AddListener(() => {
            PUIManager.GetUI<PArchUI>().ArchInfoInputField.text = ArchInfo.Name + "\n" + ArchInfo.Info;
        });
        UIBackgroundImage.gameObject.SetActive(true);
        return this;
    }
}
