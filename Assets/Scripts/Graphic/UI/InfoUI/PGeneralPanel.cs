using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using System.Collections.Generic;

public class PGeneralPanel : PAbstractGroupUI<PGeneralButtonUI>{


    public PGeneralPanel(Transform _Background) : base(_Background) {
        Close();
    }

    public void Initialize() {
        int Count = 0;
        float AllLength = UIBackgroundImage.gameObject.GetComponent<RectTransform>().rect.width;
        int LineCapacity = Mathf.FloorToInt((AllLength + 10.0f) / 70.0f);

        List<PGeneral> GeneralList = ListSubTypeInstances<PGeneral>().FindAll((PGeneral General) => !(General is P_Soldier) && General.CanBeChoose);
        GeneralList.Sort((PGeneral g1, PGeneral g2) => {
            return g1.Index - g2.Index;
        });

        GeneralList.ForEach((PGeneral General) => {
            AddSubUI().Initialize(PrototypeUI.UIBackgroundImage, Count++, LineCapacity, General);

        });
    }
}
