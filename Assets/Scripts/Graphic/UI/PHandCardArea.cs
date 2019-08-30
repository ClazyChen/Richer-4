using UnityEngine;
using UnityEngine.UI;
using System.Threading;

public class PHandCardArea : PAbstractGroupUI<PHandCard>{

    public PHandCardArea(Transform _Background) : base(_Background) {
        Close();
    }

    /// <summary>
    /// 从参数的第一项开始，是新的
    /// </summary>
    /// <param name="HandCardNames"></param>
    public void Refresh(string[] HandCardNames) {
        GroupUIList.ForEach((PHandCard SubUI) => {
            SubUI.Close();
            Object.Destroy(SubUI.UIBackgroundImage.gameObject);
        });
        GroupUIList.Clear();
        for (int i = 0; i < HandCardNames.Length-1; ++ i) {
            AddSubUI().Initialize(HandCardNames[i+1], PrototypeUI.UIBackgroundImage.localPosition, i, HandCardNames.Length-1).Open();
        }
    }
}
