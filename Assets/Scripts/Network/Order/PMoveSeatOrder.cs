/// <summary>
/// 房间内移动座性命令+目的地座位序号
/// </summary>
/// SR：移动座位
/// 然后对所有人返回一个房间数据更新命令
public class PMoveSeatOrder : POrder {
    public PMoveSeatOrder() : base("move_seat",
        (string[] args, string IPAddress) => {
            try {
                int TargetPlace = int.Parse(args[1]);
                if (!PNetworkManager.Game.Room.PlayerList[TargetPlace].PlayerType.Equals(PPlayerType.Player)) {
                    if (PNetworkManager.Game.GameMode.Seats[TargetPlace].Locked) {
                        // 位置被锁定，不可更换
                    } else {
                        PNetworkManager.Game.Room.MovePlayer(PNetworkManager.Game.Room.FindIndexByIPAddress(IPAddress), TargetPlace);
                        PNetworkManager.NetworkServer.TellClients(new PRoomDataOrder(PNetworkManager.Game.Room.ToString()));
                    }
                }
            } catch {
                PLogger.Log("MoveSeat-错误：" + args[1]);
            }
        },
        null) {
    }
    public PMoveSeatOrder(string _SeatIndex) : this() {
        args = new string[] { _SeatIndex };
    }
}
