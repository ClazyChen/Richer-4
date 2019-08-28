using System;

/// <summary>
/// 选择结果命令+选择结果标号
/// </summary>
/// SR：将选择管理器的结果置为发送过来的结果
public class PChooseResultOrder : POrder {
    public PChooseResultOrder() : base("choose_result",
        (string[] args, string IPAddress) => {
            if (PNetworkManager.NetworkServer.Game.EndGameFlag) {
                PNetworkManager.NetworkServer.Game.Prepared(IPAddress);
            } else {
                PNetworkManager.NetworkServer.ChooseManager.ChosenAnswer = Convert.ToInt32(args[1]);
            }
        },
        null) {
    }
    public PChooseResultOrder(string _Result) : this() {
        args = new string[] { _Result };
    }
}
