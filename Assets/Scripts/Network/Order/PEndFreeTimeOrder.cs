/// <summary>
/// 结束空闲时间点命令
/// </summary>
/// 
public class PEndFreeTimeOrder : POrder {
    public PEndFreeTimeOrder() : base("end_free_time",
        (string[] args, string IPAddress) => {
            //if (PNetworkManager.Game.Logic.SingleSettle() && PNetworkManager.Game.NowPlayer.IPAddress.Equals(IPAddress) && PNetworkManager.Game.NowPlayer.Marks.ExistMark(PMark.FreeTimeOperating)) {
            //    PNetworkManager.Game.NowPlayer.Marks.RemoveMark(PMark.FreeTimeOperating);
            //}
        },
        null) {
    }
}
