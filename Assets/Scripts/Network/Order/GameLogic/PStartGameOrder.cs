using System;

/// <summary>
/// 开始游戏命令[+地图名+玩家Index]
/// </summary>
/// SR：如果游戏没有开始则开始游戏
/// CR：初始化游戏状态数据，切换到MUI
public class PStartGameOrder : POrder {
    public PStartGameOrder() : base("start_game",
        (string[] args, string IPAddress) => {
            if (!PNetworkManager.NetworkServer.Game.StartGameFlag) {
                if (PNetworkManager.NetworkServer.Game.Room.IsFull()) {
                    PNetworkManager.NetworkServer.Game.StartGame();
                }
            }
        },
        (string[] args) => {
            string MapName = args[1];
            int PlayerIndex = Convert.ToInt32(args[2]);
            PMap Map = PSystem.MapList.Find((PMap TempMap) => TempMap.Name.Equals(MapName));
            if (Map != null) {
                PSystem.PlayerIndex = PlayerIndex;
                PNetworkManager.NetworkClient.GameStatus = new PGameStatus(Map, PSystem.CurrentMode);
                PNetworkManager.NetworkClient.GameStatus.StartGame();
                PUIManager.AddNewUIAction("StartGame[Client]-初始化地图切换到MUI", () => {
                    PUIManager.ChangeUI<PMapUI>();
                });
            }
        }) {
    }

    public PStartGameOrder(string _MapName, string _PlayerIndexString) : this() {
        args = new string[] { _MapName, _PlayerIndexString };
    }
}
