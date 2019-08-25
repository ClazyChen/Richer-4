/// <summary>
/// 关闭骰子命令
/// </summary>
/// CR：关闭骰子
public class PCloseDiceOrder : POrder {
    public PCloseDiceOrder() : base("close_dice",
        null,
        (string[] args) => {
            PAnimation.AddAnimation("关闭骰子", () => {
                PUIManager.GetUI<PMapUI>().DiceImage.gameObject.SetActive(false);
            });
        }) {
    }
}
