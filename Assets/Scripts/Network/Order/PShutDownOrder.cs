/// <summary>
/// 强制关闭游戏命令
/// </summary>
/// CR：关闭客户端并返回到IUI
public class PShutDownOrder : POrder {
    public PShutDownOrder() : base("shut_down",
        null,
        (string[] args) => {
            PNetworkManager.AbortClient();
            PUIManager.AddNewUIAction("ShutDown-返回InitialUI", () => {
                PUIManager.ChangeUI<PInitialUI>();
            });
        }) {
    }
}
