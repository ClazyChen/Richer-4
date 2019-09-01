using UnityEngine;
using System;

/// <summary>
/// 高亮格子命令+格子标号
/// </summary>
/// CR：高亮对应的格子
public class PHighlightBlockOrder : POrder {
    public PHighlightBlockOrder() : base("hightlight_block",
        null,
        (string[] args) => {
            int BlockIndex = Convert.ToInt32(args[1]);
            int Frame = 0;
            if (PUIManager.IsCurrentUI<PMapUI>() && 0 <= BlockIndex && BlockIndex < PNetworkManager.NetworkClient.GameStatus.Map.BlockList.Count) {
                PAnimation.AddAnimation("高亮格子", () => {
                    PBlockScene Scene = PUIManager.GetUI<PMapUI>().Scene.BlockGroup.GroupUIList[BlockIndex];
                    if (Frame == 0) {
                        Frame = 1;
                        Scene.BlockImage.gameObject.GetComponent<MeshRenderer>().material.color =PBlockScene.Config.HighlightedBlockColor;
                    } else {
                        Scene.BlockImage.gameObject.GetComponent<MeshRenderer>().material.color = PBlockScene.Config.DefaultBlockColor;
                    }
                }, 2, 0.5f);
            }
        }) {
    }

    public PHighlightBlockOrder(string _BlockIndex) : this() {
        args = new string[] {  _BlockIndex };
    }
}
