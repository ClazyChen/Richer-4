using UnityEngine;

public class PPlayerInformationBoxGroup : PAbstractGroupUI<PPlayerInformationBox> {
    public PPlayerInformationBoxGroup(Transform _Background) : base(_Background) {
        Close();
    }

    /// <summary>
    /// 初始化所有玩家的信息区域
    /// </summary>
    public void InitializeBoxes(PGameStatus Game) {
        foreach (PPlayer Player in Game.PlayerList) {
            RectTransform SubUI = AddSubUI().Initialize(Player).UIBackgroundImage.GetComponent<RectTransform>();
            SubUI.localPosition = PrototypeUI.UIBackgroundImage.GetComponent<RectTransform>().localPosition + new Vector3(0, -70.0f* Player.Index, 0);
        }
    }

    public void Update(int Index) {
        if (0<=Index && Index < GroupUIList.Count) {
            GroupUIList[Index].UpdateInformation();
        }
    }
}
