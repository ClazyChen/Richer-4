using UnityEngine;

/// <summary>
/// PMapScene类：
/// 用于管理3D地图的顶层类
/// </summary>
/// 这个类被嵌套在PMapUI里面，并且只能在PMapUI里面访问
/// 其不会受到PUIManager的管理，而是在PMapUI被打开的同时打开
/// 
public class PMapScene : PAbstractUI {

    private class Config {
        public readonly static Color DefaultMapBackgroundColor = new Color(0.58f, 0.95f, 0.47f);
    }

    public readonly Transform Background;
    
    public readonly PPlayerGroupScene PlayerGroup;
    public readonly PBlockGroupScene BlockGroup;
    public readonly PPortalGroupScene PortalGroup;
    public bool HasInitialized = false;

    public PMapScene(Transform _Background) : base(_Background) {
        Background = UIBackgroundImage.Find("Background");
        PlayerGroup = new PPlayerGroupScene(UIBackgroundImage.Find("Players"));
        BlockGroup = new PBlockGroupScene(UIBackgroundImage.Find("Blocks"));
        PortalGroup = new PPortalGroupScene(UIBackgroundImage.Find("Portals"));
        Close();
    }

    public override void Open() {
        // 不调用base
        PlayerGroup.Open();
        BlockGroup.Open();
        PortalGroup.Open();
    }

    public override void Close() {
        // 不调用base
        HasInitialized = false;
        PlayerGroup.Close();
        BlockGroup.Close();
        PortalGroup.Close();
    }

    public void InitializeMap(PMap Map) {
        Close();
        PlayerGroup.InitializePlayers();
        BlockGroup.InitializeBlocks(Map);
        PortalGroup.InitializePortals(Map);
        Background.position = new Vector3((Map.Width - 1.0f) * 5.0f, -0.1f, (Map.Length - 1.0f) * 5.0f);
        Background.localScale = new Vector3(Map.Width + 1.0f, 1.0f, Map.Length + 1.0f);
        Background.gameObject.GetComponent<MeshRenderer>().material.color = Config.DefaultMapBackgroundColor;
        HasInitialized = true;
    }
}
