/// <summary>
/// 刷新金钱数量命令+玩家编号+金钱数量
/// </summary>
/// CR：刷新金钱数量并显示在MUI上
public class PRefreshMoneyOrder : POrder {
    public PRefreshMoneyOrder() : base("refresh_money",
        null,
        (string[] args) => {
            int PlayerIndex = int.Parse(args[1]);
            int Money = int.Parse(args[2]);
            if (0 <= PlayerIndex && PlayerIndex < PNetworkManager.NetworkClient.GameStatus.PlayerNumber) {
                PNetworkManager.NetworkClient.GameStatus.PlayerList[PlayerIndex].Money = Money;
            }
            PUIManager.AddNewUIAction("RefreshMoney-刷新信息栏", () => {
                PUIManager.GetUI<PMapUI>().PlayerInformationGroup.Update(PlayerIndex);
            });
        }) {
    }

    public PRefreshMoneyOrder(string _PlayerIndex, string _Money) : this() {
        args = new string[] { _PlayerIndex, _Money };
    }
}
