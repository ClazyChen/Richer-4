/// <summary>
/// 关闭骰子命令
/// </summary>
/// CR：关闭骰子
public class PCloseDiceOrder : POrder {
    public PCloseDiceOrder() : base("close_dice",
        null,
        (string[] args) => {
            int Frame = 0;
            PAnimation.AddAnimation("关闭骰子", () => {
                if (Frame == 0) {
                    Frame = 1;
                } else {
                    PUIManager.GetUI<PMapUI>().DiceImage.gameObject.SetActive(false);
                }
            }, 2, 0.3f);
        }) {
    }
}
