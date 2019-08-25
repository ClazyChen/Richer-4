using System;
using UnityEngine.UI;
/// <summary>
/// 推送消息命令+推送起始玩家序号+推送内容+推送类型
/// </summary>
/// CR：显示相应的推送
public class PPushTextOrder : POrder {
    public PPushTextOrder() : base("push_text",
        null,
        (string[] args) => {
            PPlayer Player = PNetworkManager.NetworkClient.GameStatus.FindPlayer(Convert.ToInt32(args[1]));
            string TextToPush = args[2];
            PPushType PushType = FindInstance<PPushType>(args[3]);
            if (Player != null && PushType != null) {
                PAnimation.PushAnimation(Player, TextToPush, PushType);
            }
        }) {
    }

    public PPushTextOrder(string _PlayerIndex, string _Text, string _PushType) : this() {
        args = new string[] { _PlayerIndex, _Text, _PushType};
    }
}
