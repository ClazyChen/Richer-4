using UnityEngine;
using UnityEngine.UI;
using System.Threading;

public class PCardUIGroup : PAbstractGroupUI<PCardUI>{

    public int StartIndex = 0;

    public PCardUIGroup(Transform _Background) : base(_Background) {
        Close();
    }

    /// <summary>
    /// 从参数的第一项开始，是新的
    /// </summary>
    /// <param name="CardNames"></param>
    public void Refresh(string[] CardNames) {
        GroupUIList.ForEach((PCardUI SubUI) => {
            SubUI.Close();
            Object.Destroy(SubUI.UIBackgroundImage.gameObject);
        });
        GroupUIList.Clear();
        for (int i = 0; i < CardNames.Length-1; ++ i) {
            AddSubUI().Initialize(CardNames[i+1], PrototypeUI.UIBackgroundImage.localPosition, i + StartIndex, CardNames.Length-1).Open();
        }
    }
}
