using UnityEngine;

/// <summary>
/// 移动位置命令+移动位置的玩家编号+目的地格子编号
/// </summary>
/// CR：播放移动玩家的棋子动画
public class PMovePositionOrder : POrder {
    public PMovePositionOrder() : base("move_position",
        null,
        (string[] args) => {
            int PlayerIndex = int.Parse(args[1]);
            int DestinationIndex = int.Parse(args[2]);
            if (0 <= PlayerIndex && PlayerIndex < PNetworkManager.NetworkClient.GameStatus.PlayerNumber && 0 <= DestinationIndex && DestinationIndex < PNetworkManager.NetworkClient.GameStatus.Map.BlockList.Count) {
                PBlock DestinationBlock = PNetworkManager.NetworkClient.GameStatus.Map.BlockList[DestinationIndex];
                PPlayer Player = PNetworkManager.NetworkClient.GameStatus.PlayerList[PlayerIndex];
                Player.Position = DestinationBlock;
                Vector3 DestinationSpacePosition = PPlayerScene.GetSpacePosition(Player);
                PUIManager.AddNewUIAction("启动移动玩家棋子", () => {
                    PUIManager.GetUI<PMapUI>().Scene.PlayerGroup.MovePlayer(PlayerIndex, DestinationSpacePosition);
                });
            }
        }) {
    }

    public PMovePositionOrder(string _PlayerIndex, string _DestinationIndex) : this() {
        args = new string[] { _PlayerIndex, _DestinationIndex };
    }
}
