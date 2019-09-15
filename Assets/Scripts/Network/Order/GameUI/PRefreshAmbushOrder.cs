using System.Collections.Generic;

/// <summary>
/// 更新伏兵命令
/// </summary>
/// CR：在手牌显示区一一显示相应的手牌
public class PRefreshAmbushOrder : POrder {
    public PRefreshAmbushOrder() : base("refresh_ambush",
        null,
        (string[] args) => {
            PAnimation.AddAnimation("更新伏兵区", () => {
                PUIManager.GetUI<PMapUI>().AmbushCardArea.Refresh(args);
            });
        }) {
    }

    public PRefreshAmbushOrder(string[] _AmbushNames) : this() {
        args = _AmbushNames;
    }
}
