/// <summary>
/// 掷骰结果命令+掷骰结果（1~6）
/// </summary>
/// 
public class PDiceResultOrder : POrder {
    public PDiceResultOrder() : base("dice_result",
        null,
        (string[] args) => {
            int DiceResult = int.Parse(args[1]);
            if (DiceResult >= 1 && DiceResult <= 6) {
                PUIManager.AddNewUIAction("DiceResult-掷骰结果：" + DiceResult.ToString(), () => {
                    PUIManager.GetUI<PMapUI>().Dice(DiceResult);
                });
            }
        }) {
    }

    public PDiceResultOrder(string _DiceResult) : this() {
        args = new string[] { _DiceResult };
    }
}
