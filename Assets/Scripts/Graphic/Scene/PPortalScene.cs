using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PPortalScene : PAbstractUI {
    public PPortalScene(Transform _Background) : base(_Background) {
        Close();
    }

    public void InitializePortal(PPortal Portal) {
        UIBackgroundImage.position = GetSpacePosition(Portal);
        UIBackgroundImage.eulerAngles = GetEnlerAngles(Portal);
    }

    /// <summary>
    /// 获取传送门的空间位置
    /// </summary>
    private static Vector3 GetSpacePosition(PPortal Portal) {
        return new Vector3(10.0f * Portal.Y, 0.0f, 10.0f * Portal.X);
    }

    /// <summary>
    /// 获取传送门的欧拉角
    /// </summary>
    private static Vector3 GetEnlerAngles(PPortal Portal) {
        return new Vector3(90.0f, 0.0f, Portal.RotateAngle);
    }
}
