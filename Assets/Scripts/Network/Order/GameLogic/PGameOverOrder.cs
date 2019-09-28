using System.Linq;
/// <summary>
/// 结束游戏+胜利者列表
/// </summary>
/// CR：显示一个MessageBox，等待玩家按下之后发出一个信号，表示准备好
public class PGameOverOrder : POrder {
    public PGameOverOrder() : base("game_over",
        null,
        (string[] args) => {
            string Winners = args[1];
            int GetMoney = 2;
            if (Winners.Split(',').Contains(PSystem.UserManager.Nickname)) {
                GetMoney += PSystem.CurrentMode.Bonus;
            }
            PSystem.UserManager.Money += GetMoney;
            PSystem.UserManager.Write();
            PAnimation.AddAnimation("游戏结束", () => {
                PUIManager.GetUI<PMapUI>().Ask("游戏结束，银两+" + GetMoney, new string[] {
                    "为" + Winners + "的胜利干杯！",
                    "天佑" + Winners + "！"
                });
            });
        }) {
    }

    public PGameOverOrder(string _Winners) : this() {
        args = new string[] { _Winners };
    }
}
