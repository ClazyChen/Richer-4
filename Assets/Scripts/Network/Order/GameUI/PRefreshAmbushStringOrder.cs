using System;

/// <summary>
/// 刷新伏兵区文字命令
/// </summary>
/// 
public class PRefreshAmbushStringOrder : POrder {
    public PRefreshAmbushStringOrder() : base("refresh_ambush_string",
        null,
        (string[] args) => {
            int PlayerIndex = Convert.ToInt32(args[1]);
            string AmbushString = args[2];
            PAnimation.AddAnimation("RefreshAmbushString-刷新信息栏", () => {
                if (0 <= PlayerIndex && PlayerIndex < PNetworkManager.NetworkClient.GameStatus.PlayerNumber) {
                    PNetworkManager.NetworkClient.GameStatus.PlayerList[PlayerIndex].AmbushString = AmbushString;
                    PUIManager.GetUI<PMapUI>().PlayerInformationGroup.Update(PlayerIndex);
                }
            });
        }) {
    }

    public PRefreshAmbushStringOrder(string _PlayerIndex, string _AmbushString) : this() {
        args = new string[] { _PlayerIndex, _AmbushString };
    }

    public PRefreshAmbushStringOrder(PPlayer Player) : this(Player.Index.ToString(), Player.GetAmbushString()) {

    }
}
