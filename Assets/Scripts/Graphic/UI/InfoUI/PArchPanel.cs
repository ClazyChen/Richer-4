using UnityEngine;
using UnityEngine.UI;
using System.Threading;

public class PArchPanel : PAbstractGroupUI<PArchButtonUI>{


    public PArchPanel(Transform _Background) : base(_Background) {
        Close();
    }

    public void Initialize() {
        int Count = 0;
        float AllLength = UIBackgroundImage.gameObject.GetComponent<RectTransform>().rect.width;
        int LineCapacity = Mathf.FloorToInt((AllLength + 10.0f) / 70.0f);

        ListInstance<PArchInfo>().ForEach((PArchInfo ArchInfo) => {
            AddSubUI().Initialize(PrototypeUI.UIBackgroundImage, Count++, LineCapacity, ArchInfo);
        });
    }
}
