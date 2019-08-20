using UnityEngine;

public class PPlayerGroupScene : PAbstractGroupUI<PPlayerScene>{

    public PPlayerGroupScene(Transform _Background) : base(_Background) {
        Close();
    }

    /// <summary>
    /// 初始化所有玩家的棋子
    /// </summary>
    public void InitializePlayers() {
        foreach (PPlayer Player in PNetworkManager.NetworkClient.GameStatus.PlayerList) {
            AddSubUI().InitializePlayer(Player, PNetworkManager.NetworkClient.GameStatus.PlayerList.Count);
        }
    }


    public void MovePlayer(int Index, Vector3 Destination) {
        PAnimation.MoveAnimation(GroupUIList[Index].UIBackgroundImage, Destination, PAnimation.MovePlayerAnimation.FrameNumber, PAnimation.MovePlayerAnimation.TotalTime);
    }
}
