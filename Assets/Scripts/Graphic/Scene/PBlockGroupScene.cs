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

    public int FindBlockSceneIndex(Vector3 WorldPosition) {
        for (int i = 0; i < GroupUIList.Count; ++ i) {
            Vector3 BlockSpacePosition = GroupUIList[i].UIBackgroundImage.position;
            if (Mathf.Abs(WorldPosition.x - BlockSpacePosition.x) < 0.9f && Mathf.Abs(WorldPosition.z - BlockSpacePosition.z) < 0.9f) {
                return i;
            }
        }
        return -1;
    }
}
