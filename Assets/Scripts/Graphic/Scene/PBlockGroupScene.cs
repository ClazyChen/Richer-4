using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PBlockGroupScene : PAbstractGroupUI<PBlockScene> {

    public PBlockGroupScene(Transform _Background) : base(_Background) {
        Close();
    }

    /// <summary>
    /// 初始化所有格子
    /// </summary>
    /// <param name="Map">地图</param>
    public void InitializeBlocks(PMap Map) {
        foreach (PBlock Block in Map.BlockList) {
            AddSubUI().InitializeBlock(Block);
        }
    }
}
