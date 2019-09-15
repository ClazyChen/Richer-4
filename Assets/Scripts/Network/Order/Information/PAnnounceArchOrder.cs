using System;
/// <summary>
/// 宣布成就命令
/// </summary>
/// CR：宣布成就
public class PAnnounceArchOrder : POrder {
    public PAnnounceArchOrder() : base("announce_arch",
        null,
        (string[] args) => {
            PPlayer Player = PNetworkManager.NetworkClient.GameStatus.FindPlayer(Convert.ToInt32(args[1]));
            string ArchName = args[2];
            if (Player != null && ArchName != null && PSystem.ArchManager.AnnounceArch(FindInstance<PArchInfo>(ArchName))) {
                PAnimation.PushAnimation(Player, "获得成就：" +ArchName,  PPushType.Information);
            }
        }) {
    }

    public PAnnounceArchOrder(string _PlayerIndex, string _ArchName) : this() {
        args = new string[] { _PlayerIndex, _ArchName };
    }
}
