using UnityEngine;

public class PPortalGroupScene : PAbstractGroupUI<PPortalScene> {

    public PPortalGroupScene(Transform _Background) : base(_Background) {
        Close();
    }

    /// <summary>
    /// 初始化所有传送门
    /// </summary>
    /// <param name="Map">地图</param>
    public void InitializePortals(PMap Map) {
        foreach (PPortal Portal in Map.PortalList) {
            AddSubUI().InitializePortal(Portal);
        }
    }
}
