/// <summary>
/// 接受连接命令
/// </summary>
/// CR：返回一个自我介绍命令给服务器
public class PAcceptOrder : POrder {
    public PAcceptOrder() : base("accept",
        null,
        (string[] args) => {
            PNetworkManager.NetworkClient.Send(new PIntroduceSelfOrder(PNetworkManager.CurrentNickname));
        }) {
    }
}
