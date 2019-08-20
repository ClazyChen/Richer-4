using System.Collections;
using System.Collections.Generic;
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
            AddSubUI().Initialize(Player);
        }
    }

    public void Update(int Index) {
        if (0<=Index && Index < GroupUIList.Count) {
            GroupUIList[Index].UpdateInformation();
        }
    }
}
