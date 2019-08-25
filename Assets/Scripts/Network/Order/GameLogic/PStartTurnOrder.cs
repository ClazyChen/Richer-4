/// <summary>
/// 开始回合命令+开始回合的玩家编号
/// </summary>
/// 
public class PStartTurnOrder : POrder {
    public PStartTurnOrder() : base("start_turn",
        null,
        (string[] args) => {
            int NowPlayerIndex = int.Parse(args[1]);
            PNetworkManager.NetworkClient.GameStatus.NowPlayerIndex = NowPlayerIndex;
            PUIManager.AddNewUIAction("开始跟踪", () => {
                PUIManager.GetUI<PMapUI>().CameraController.SetTracking(PNetworkManager.NetworkClient.GameStatus.NowPlayer);
            });
        }) {
    }

    public PStartTurnOrder(string _NowPlayerIndex) : this() {
        args = new string[] { _NowPlayerIndex };
    }
}
