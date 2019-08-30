using System;

/// <summary>
/// 刷新手牌数量命令+玩家编号+手牌数量
/// </summary>
/// CR：刷新手牌数量并显示在MUI上
public class PRefreshHandCardNumberOrder : POrder {
    public PRefreshHandCardNumberOrder() : base("refresh_hand_card_number",
        null,
        (string[] args) => {
            int PlayerIndex = Convert.ToInt32(args[1]);
            int HandCardNumber = Convert.ToInt32(args[2]);
            PAnimation.AddAnimation("RefreshHandCardNumber-刷新信息栏", () => {
                if (0 <= PlayerIndex && PlayerIndex < PNetworkManager.NetworkClient.GameStatus.PlayerNumber) {
                    PNetworkManager.NetworkClient.GameStatus.PlayerList[PlayerIndex].HandCardNumber = HandCardNumber;
                }
                PUIManager.GetUI<PMapUI>().PlayerInformationGroup.Update(PlayerIndex);
            });
        }) {
    }

    public PRefreshHandCardNumberOrder(string _PlayerIndex, string _HandCardNumber) : this() {
        args = new string[] { _PlayerIndex, _HandCardNumber };
    }
}
