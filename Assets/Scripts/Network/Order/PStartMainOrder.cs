/// <summary>
/// 开始回合命令+开始回合的玩家编号
/// </summary>
/// 
public class PStartMainOrder : POrder {
    public PStartMainOrder() : base("start_main",
        null,
        (string[] args) => {
            int NowPlayerIndex = int.Parse(args[1]);
            PNetworkManager.NetworkClient.GameStatus.NowPlayerIndex = NowPlayerIndex;
            PUIManager.AddNewUIAction("开始跟踪", () => {
                PUIManager.GetUI<PMapUI>().CameraController.SetTracking(PNetworkManager.NetworkClient.GameStatus.NowPlayer);
            });
        }) {
    }

    public PStartMainOrder(string _NowPlayerIndex) : this() {
        args = new string[] { _NowPlayerIndex };
    }
}
