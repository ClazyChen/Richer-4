using System;
using UnityEngine.UI;
/// <summary>
/// 显示信息命令+信息
/// </summary>
/// CR：在MUI上方的标题栏显示相应的信息
public class PShowInformationOrder : POrder {
    public PShowInformationOrder() : base("show_information",
        null,
        (string[] args) => {
            string Information = args[1];
            PAnimation.AddAnimation("显示消息[" + Information + "]", () => {
                PUIManager.GetUI<PMapUI>().InformationText.text = Information;
                PUIManager.GetUI<PMapUI>().InformationText.gameObject.SetActive(true);
            }, 1, 0, () => {
                PUIManager.AddNewUIAction("结束显示消息[" + Information + "]", () => {
                    PUIManager.GetUI<PMapUI>().InformationText.gameObject.SetActive(false);
                });
            });
        }) {
    }

    public PShowInformationOrder(string _Information) : this() {
        args = new string[] {_Information};
    }
}
