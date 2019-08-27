using UnityEngine;
using UnityEngine.UI;

public class PMessage : PAbstractUI {
    public bool IsChosen = false;
    public int Index;

    public PMessage(Transform _Background) : base(_Background) {
        Close();
    }

    public void Initialize(string Text, int _Index, int ButtonNumber, Vector3 CenterPoint, float Delta) {
        Index = _Index;
        UIBackgroundImage.GetComponent<RectTransform>().position = CenterPoint + new Vector3(0, Delta * ButtonNumber /2 - Delta * (Index + 1));
        UIBackgroundImage.GetComponentInChildren<Text>().text = Text;
        UIBackgroundImage.GetComponent<Button>().onClick.AddListener(() => {
            IsChosen = true;
        });
    }
}
