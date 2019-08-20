/// <summary>
/// 拒绝命令
/// </summary>
public class PRejectOrder : POrder {
    public PRejectOrder() : base("reject",
        null,
        (string[] args) => {
            if (PUIManager.IsCurrentUI<PJoinUI>()) {
                PUIManager.AddNewUIAction("Reject-显示拒绝信息", () => {
                    PUIManager.GetUI<PJoinUI>().ErrorText.text = "房间已满";
                });
            }
        }){
    }
}
