using System.Linq;
using System.Collections.Generic;

/// <summary>
/// 更新手牌命令+
/// </summary>
/// CR：在手牌显示区一一显示相应的手牌
public class PRefreshHandCardsOrder : POrder {
    public PRefreshHandCardsOrder() : base("refresh_hand_cards",
        null,
        (string[] args) => {
            PAnimation.AddAnimation("更新手牌区", () => {
                PUIManager.GetUI<PMapUI>().HandCardArea.Refresh(args);
            });
        }) {
    }

    public PRefreshHandCardsOrder( string[] _HandCardNames) : this() {
        args = _HandCardNames;
    }
}
