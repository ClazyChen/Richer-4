/// <summary>
/// 自我介绍命令+昵称
/// </summary>
/// SR：对目标返回一个游戏模式命令
/// 然后对所有人返回一个房间数据更新命令
public class PIntroduceSelfOrder : POrder {
    public PIntroduceSelfOrder() : base("hello",
        (string[] args, string IPAddress) => {
            string Nickname = args[1];
            PLogger.Log("新的连接：" + Nickname + " @" + IPAddress);
            if (PNetworkManager.Game.Room.AddPlayer(Nickname, IPAddress)) {
                PLogger.Log("加入房间成功");
                PNetworkManager.NetworkServer.TellClient(IPAddress, new PRoomModeOrder(PNetworkManager.Game.GameMode.Name));
                PNetworkManager.NetworkServer.TellClients(new PRoomDataOrder(PNetworkManager.Game.Room.ToString()));
            } else {
                PLogger.Log("加入房间失败");
            }
            
        },
        null) {
    }
    public PIntroduceSelfOrder(string _Nickname):this() {
        args = new string[] { _Nickname };
    }
}
