using System;

/// <summary>
/// 刷新标签域命令+玩家编号+标签域
/// </summary>
/// CR：刷新标签域并显示在MUI上
public class PRefreshMarkStringOrder : POrder {
    public PRefreshMarkStringOrder() : base("refresh_mark_string",
        null,
        (string[] args) => {
            int PlayerIndex = Convert.ToInt32(args[1]);
            string MarkString = args[2];
            PAnimation.AddAnimation("RefreshMarkString-刷新信息栏", () => {
                if (0 <= PlayerIndex && PlayerIndex < PNetworkManager.NetworkClient.GameStatus.PlayerNumber) {
                    PNetworkManager.NetworkClient.GameStatus.PlayerList[PlayerIndex].MarkString = MarkString;
                    PUIManager.GetUI<PMapUI>().PlayerInformationGroup.Update(PlayerIndex);
                }
            });
        }) {
    }

    public PRefreshMarkStringOrder(string _PlayerIndex, string _MarkString) : this() {
        args = new string[] { _PlayerIndex, _MarkString };
    }

    public PRefreshMarkStringOrder(PPlayer Player) : this(Player.Index.ToString(), Player.GetMarkString()) {

    }
}
