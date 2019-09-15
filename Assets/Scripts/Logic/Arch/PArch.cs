using System.Collections.Generic;
using System;

public abstract class PArch : PSystemTriggerInstaller {
    protected PArch(string _Name) : base(_Name) {
    }

    protected void Announce(PGame Game, PPlayer Player, string ArchName) {
        if (Player != null && Player.IsUser && Game.PlayerList.TrueForAll((PPlayer _Player) => _Player.IsAI || _Player.TeamIndex == Player.TeamIndex)) {
            PNetworkManager.NetworkServer.TellClient(Player, new PAnnounceArchOrder(Player.Index.ToString(), ArchName));
        }
    }
}