using System.Collections.Generic;

/// <summary>
/// 更新手牌命令+
/// </summary>
/// CR：在手牌显示区一一显示相应的手牌
public class PRefreshEquipmentsOrder : POrder {
    public PRefreshEquipmentsOrder() : base("refresh_equipments",
        null,
        (string[] args) => {
            PAnimation.AddAnimation("更新装备区", () => {
                PUIManager.GetUI<PMapUI>().EquipCardArea.Refresh(args);
            });
        }) {
    }

    public PRefreshEquipmentsOrder(string[] _EquipmentNames) : this() {
        args = _EquipmentNames;
    }
}
