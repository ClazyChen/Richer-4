using System;
using UnityEngine;
using UnityEngine.UI;

public class PUserUI : PAbstractUI {
    public readonly Button ReturnButton;
    public readonly Text UsernameText;
    public readonly InputField UsernameInputField;
    public readonly Button ModifyUsernameButton;
    public readonly Text ArchPointText;
    public readonly Text MoneyText;
    public readonly Text ChooseGeneralCardText;
    public readonly Button ChooseGeneralCardButton;
    public readonly Text LuckCardText;
    public readonly Button LuckCardButton;

    public PUserUI(Transform _Background) : base(_Background) {
        InitializeControls<Button>();
        InitializeControls<Text>();
        InitializeControls<InputField>();
        Close();
    }

    public override void Open() {
        base.Open();
        UsernameInputField.text = PSystem.UserManager.Nickname;
        ArchPointText.text = "�ɾ͵㣺" + PSystem.UserManager.ArchPoint + "/" + PSystem.ArchManager.TotalArchPoint();
        MoneyText.text = "������" + PSystem.UserManager.Money;
        ChooseGeneralCardText.text = "�㽫��������" + PSystem.UserManager.ChooseGeneral;
        LuckCardText.text = "������������" + PSystem.UserManager.Lucky;
        ModifyUsernameButton.onClick.AddListener(() => {
            string NewUsername = UsernameInputField.text;
            if (NewUsername.Equals(string.Empty) || NewUsername.Length > 8 || NewUsername.Contains(" ")) {
                UsernameInputField.text = "����ʧ��";
            } else {
                PSystem.UserManager.Nickname = NewUsername;
                PSystem.UserManager.Write();
            }
        });
        ChooseGeneralCardButton.onClick.AddListener(() => {
            if (PSystem.UserManager.Money >= 4) {
                PSystem.UserManager.Money -= 4;
                PSystem.UserManager.ChooseGeneral++;
                PSystem.UserManager.Write();
                MoneyText.text = "������" + PSystem.UserManager.Money;
                ChooseGeneralCardText.text = "�㽫��������" + PSystem.UserManager.ChooseGeneral;
            }
        });
        LuckCardButton.onClick.AddListener(() => {
            if (PSystem.UserManager.Money >= 2) {
                PSystem.UserManager.Money -= 2;
                PSystem.UserManager.Lucky++;
                PSystem.UserManager.Write();
                MoneyText.text = "������" + PSystem.UserManager.Money;
                LuckCardText.text = "������������" + PSystem.UserManager.Lucky;
            }
        });
        ReturnButton.onClick.AddListener(() => {
            PUIManager.AddNewUIAction("���أ�ת��IUI", () => PUIManager.ChangeUI<PInitialUI>());
        });
    }
}