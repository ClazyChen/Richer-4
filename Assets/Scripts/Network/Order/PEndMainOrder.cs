/// <summary>
/// 结束回合命令+结束回合的玩家编号
/// </summary>
/// 
public class PEndMainOrder : POrder {
    public PEndMainOrder() : base("end_main",
        null,
        (string[] args) => {
        }) {
    }

    public PEndMainOrder(string _NowPlayerIndex) : this() {
        args = new string[] { _NowPlayerIndex };
    }
}
