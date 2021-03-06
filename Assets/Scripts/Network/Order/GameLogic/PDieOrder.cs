﻿using System;

/// <summary>
/// 玩家死亡命令+死亡的玩家编号
/// </summary>
/// 
public class PDieOrder : POrder {
    public PDieOrder() : base("die",
        null,
        (string[] args) => {
            int DiePlayerIndex = Convert.ToInt32(args[1]);
            PAnimation.AddAnimation("玩家死亡", () => {
                PNetworkManager.NetworkClient.GameStatus.FindPlayer(DiePlayerIndex).IsAlive = false;
                PUIManager.GetUI<PMapUI>().PlayerInformationGroup.GroupUIList[DiePlayerIndex].Initialize(PNetworkManager.NetworkClient.GameStatus.FindPlayer(DiePlayerIndex));
                PUIManager.GetUI<PMapUI>().Scene.PlayerGroup.GroupUIList[DiePlayerIndex].Close();
            });
            
        }) {
    }

    public PDieOrder(string _PlayerIndex) : this() {
        args = new string[] { _PlayerIndex };
    }
}
