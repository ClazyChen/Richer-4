/// <summary>
/// 切换房间座位属性命令+待切换的座位序号
/// </summary>
/// SR：修改对应的座位属性
/// 然后对所有人返回一个房间数据更新命令
public class PSwitchSeatOrder : POrder {
    public PSwitchSeatOrder() : base("switch_seat",
        (string[] args, string IPAddress) => {
            try {
                int TargetPlace = int.Parse(args[1]);
                if (!PNetworkManager.Game.Room.PlayerList[TargetPlace].PlayerType.Equals(PPlayerType.Player)) {
                    if (PNetworkManager.Game.GameMode.Seats[TargetPlace].Locked) {

                    } else {
                        PNetworkManager.Game.Room.SwitchSeatAttribute(TargetPlace);
                        PNetworkManager.NetworkServer.TellClients(new PRoomDataOrder(PNetworkManager.Game.Room.ToString()));
                    }
                }
            } catch {
                PLogger.Log("SwitchSeat-错误：" + args[1]);
            }
        },
        null) {
    }
    public PSwitchSeatOrder(string _SeatIndex) : this() {
        args = new string[] { _SeatIndex };
    }
}
