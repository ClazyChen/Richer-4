using UnityEngine;

/// <summary>
/// PPlayerScene类：
/// 用于管理3D地图中一个Player棋子
/// </summary>
/// 
public class PPlayerScene : PAbstractUI {

    public class Config {
        public readonly static Color[] PlayerColors = {
            new Color(1, 0.35f, 0),
            new Color(0, 1, 0.09f),
            new Color(1, 1, 0),
            new Color(0, 0.86f, 0.89f),
            new Color(0.32f, 0.42f, 0.96f),
            new Color(0.8f, 0.52f, 0.96f),
            new Color(1, 0.56f, 0.75f),
            new Color(1, 0.68f, 0)
        };
    }

    private static int PlayerNumber = 0;

    public PPlayerScene(Transform _Background) : base(_Background) {
        Close();
    }

    public void InitializePlayer(PPlayer Player, int _PlayerNumber) {
        PlayerNumber = _PlayerNumber;
        UIBackgroundImage.position = PBlockScene.GetSpacePosition(Player.Position) + PlayerPositionBias(Player, PlayerNumber);
        SetColor(Config.PlayerColors[Player.Index]);
    }

    private static Vector3 PlayerPositionBias(PPlayer Player, int PlayerNumber) {
        if (PlayerNumber <= 0) {
            return Vector3.zero;
        }
        float BiasAngle = 2 * Mathf.PI * Player.Index / PlayerNumber;
        float BiasRadius = 1 / Mathf.Sin(Mathf.PI / PlayerNumber);
        return new Vector3(-BiasRadius * Mathf.Sin(BiasAngle), 0.0f, BiasRadius * Mathf.Cos(BiasAngle));
    }

    public static Vector3 GetSpacePosition(PPlayer Player) {
        return PBlockScene.GetSpacePosition(Player.Position) + PlayerPositionBias(Player, PlayerNumber);
    }

    public static Vector3 GetScreenPosition(PPlayer Player) {
        return PUIManager.GetUI<PMapUI>().CameraController.Camera.GetComponent<Camera>().WorldToScreenPoint(PUIManager.GetUI<PMapUI>().Scene.PlayerGroup.GroupUIList[Player.Index].UIBackgroundImage.position);
    }
}
