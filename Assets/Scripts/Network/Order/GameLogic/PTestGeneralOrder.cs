using System;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// 测试武将命令+玩家编号+武将名
/// </summary>
/// SR：在ChooseManager中确认武将
/// CR：返回是否拥有对应的武将
public class PTestGeneralOrder : POrder {
    public PTestGeneralOrder() : base("test_general",
        (string[] args, string IPAddress) => {
            int PlayerIndex = Convert.ToInt32(args[1]);
            string GeneralName = args[2];
            PNetworkManager.NetworkServer.ChooseManager.ChosenAnswer = (GeneralName.Equals("XX") ? 1 : 0);
        },
        (string[] args) => {
            int PlayerIndex = Convert.ToInt32(args[1]);
            string GeneralName = args[2];
            if (PSystem.UserManager.GeneralList.Contains(GeneralName)) {
                PNetworkManager.NetworkClient.Send(new PTestGeneralOrder(PlayerIndex.ToString(), GeneralName));
            } else {
                PNetworkManager.NetworkClient.Send(new PTestGeneralOrder(PlayerIndex.ToString(), "XX"));
            }
        }) {
    }

    public PTestGeneralOrder(string PlayerIndex, string GeneralName) : this() {
        args = new string[] { PlayerIndex, GeneralName };
    }
}
