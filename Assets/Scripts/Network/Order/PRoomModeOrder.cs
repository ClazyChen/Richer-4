/// <summary>
/// 房间属性命令+游戏模式
/// </summary>
/// CR：设定游戏模式
public class PRoomModeOrder : POrder {
    public PRoomModeOrder() : base("mode",
        null,
        (string[] args) => {
            string GameMode = args[1];
            PSystem.CurrentMode = PMode.ListModes().Find((PMode Mode) => Mode.Name.Equals(GameMode));
        }) {
    }

    public PRoomModeOrder(string _GameMode) : this() {
        args = new string[] { _GameMode };
    }
}
