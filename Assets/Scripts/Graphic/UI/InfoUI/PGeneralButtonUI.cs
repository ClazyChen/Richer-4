using UnityEngine;
using UnityEngine.UI;
using System;

public class PGeneralButtonUI : PAbstractUI {
    public readonly Button GeneralButton;
    public PGeneral General;
    public int Index;

    public PGeneralButtonUI(Transform _Background):base(_Background) {
        GeneralButton = UIBackgroundImage.GetComponent<Button>();
        Close();
    }

    public PGeneralButtonUI Initialize(Transform Prototype, int _Index, int LineCapacity, PGeneral _General) {
        Index = _Index;
        General = _General;
        UIBackgroundImage.GetComponentInChildren<Text>().text = General.Name;
        UIBackgroundImage.localScale = new Vector3(1, 1, 1);
        UIBackgroundImage.localPosition = new Vector3(70.0f * (Index % LineCapacity) + Prototype.localPosition.x, -70.0f * (Index / LineCapacity) + Prototype.localPosition.y, 0.0f);

        void InvokeBuyGeneral(Button TargetButton) {
            string Method = (TargetButton.Equals(PUIManager.GetUI<PGeneralUI>().BuyArchPointButton) ? "成就点" : "银两");
            TargetButton.onClick.RemoveAllListeners();
            TargetButton.GetComponentInChildren<Text>().text = General.Cost.ToString() + Method + " 购买";
            TargetButton.onClick.AddListener(() => {
                if (!PSystem.UserManager.GeneralList.Contains(General.Name)) {
                    bool CanPurchase = false;
                    if (Method.Equals("成就点") && PSystem.UserManager.ArchPoint >= General.Cost) {
                        PSystem.UserManager.ArchPoint -= General.Cost;
                        CanPurchase = true;
                    } else if (Method.Equals("银两") && PSystem.UserManager.Money >= General.Cost) {
                        PSystem.UserManager.Money -= General.Cost;
                        CanPurchase = true;
                    }
                    if (CanPurchase) {
                        PSystem.UserManager.GeneralList.Add(General.Name);
                        PSystem.UserManager.Write();
                        UIBackgroundImage.GetComponent<Image>().color = PGeneralUI.Config.GotGeneralColor;
                        PUIManager.GetUI<PGeneralUI>().BuyArchPointButton.interactable = false;
                        PUIManager.GetUI<PGeneralUI>().BuyMoneyButton.interactable = false;
                    }
                }
            });
            if (PSystem.UserManager.GeneralList.Contains(General.Name)) {
                TargetButton.interactable = false;
            } else {
                TargetButton.interactable = true;
            }
        }

        GeneralButton.onClick.AddListener(() => {
            PUIManager.GetUI<PGeneralUI>().GeneralInfoInputField.text = General.Name + "\n" +
                                                                        "性别：" + General.Sex + "\n" +
                                                                        "时代：" + General.Age + "\n" +
                             string.Join("\n", General.SkillList.ConvertAll((PSkill Skill) => {
                                 PSkillInfo SkillInfo = ListInstance<PSkillInfo>().Find((PSkillInfo Info) => Info.Name.Equals(Skill.Name));
                                 if (SkillInfo == null) {
                                     return Skill.Name;
                                 } else {
                                     return Skill.Name + "：" + SkillInfo.ToolTip;
                                 }
                             })) + "\n\n" + 
                              General.Tips;
            InvokeBuyGeneral(PUIManager.GetUI<PGeneralUI>().BuyArchPointButton);
            InvokeBuyGeneral(PUIManager.GetUI<PGeneralUI>().BuyMoneyButton);
        });
        UIBackgroundImage.gameObject.SetActive(true);
        return this;
    }
}
