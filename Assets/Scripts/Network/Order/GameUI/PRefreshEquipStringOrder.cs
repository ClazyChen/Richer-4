using System;

/// <summary>
/// 刷新装备区文字命令
/// </summary>
/// 
public class PRefreshEquipStringOrder : POrder {
    public PRefreshEquipStringOrder() : base("refresh_equip_string",
        null,
        (string[] args) => {
            int PlayerIndex = Convert.ToInt32(args[1]);
            string EquipString = args[2];
            PAnimation.AddAnimation("RefreshEquipString-刷新信息栏", () => {
                if (0 <= PlayerIndex && PlayerIndex < PNetworkManager.NetworkClient.GameStatus.PlayerNumber) {
                    PNetworkManager.NetworkClient.GameStatus.PlayerList[PlayerIndex].EquipString = EquipString;
                    PUIManager.GetUI<PMapUI>().PlayerInformationGroup.Update(PlayerIndex);
                }
            });
        }) {
    }

    public PRefreshEquipStringOrder(string _PlayerIndex, string _EquipString) : this() {
        args = new string[] { _PlayerIndex, _EquipString };
    }

    public PRefreshEquipStringOrder(PPlayer Player) : this(Player.Index.ToString(), Player.GetEquipString()) {

    }
}
