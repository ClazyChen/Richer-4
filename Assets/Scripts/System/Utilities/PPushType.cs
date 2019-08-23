using UnityEngine;

public class PPushType : PObject {
    public readonly Color PushColor;

    private PPushType(string _Name, Color _Color) {
        Name = _Name;
        PushColor = _Color;
    }

    public PPushType Information = new PPushType("消息", new Color(0, 0, 1));
    public PPushType Heal = new PPushType("回复金钱", new Color(0, 0.6f, 0));
    public PPushType Injure = new PPushType("受到伤害", new Color(1, 0, 0));
    public PPushType Throw = new PPushType("弃置金钱", new Color(0.5f, 0.5f, 0));
    public PPushType NoType = new PPushType("无类型", new Color(0, 0, 0));
}
