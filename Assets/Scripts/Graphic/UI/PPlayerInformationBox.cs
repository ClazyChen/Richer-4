using UnityEngine;
using UnityEngine.UI;

public class PPlayerInformationBox : PAbstractUI {
    public readonly Text NameText;
    public readonly Text MoneyText;
    public readonly Text CardText;
    public readonly Text EquipText;
    public readonly Text JudgeText;
    public readonly Text FlagText;
    public readonly Text PeriodText;
    public readonly Image GeneralImageBackground;
    public readonly Image GeneralImage;

    private PPlayer AttachedPlayer = null;

    public PPlayerInformationBox(Transform _Background):base(_Background) {
        InitializeControls<Text>();
        InitializeControls<Image>();
        Close();
    }

    public PPlayerInformationBox Initialize(PPlayer _Player) {
        AttachedPlayer = _Player;
        UpdateInformation();
        return this;
    }

    public void UpdateInformation() {
        NameText.color = PPlayerScene.Config.PlayerColors[AttachedPlayer.TeamIndex];
        GeneralImageBackground.color = PPlayerScene.Config.PlayerColors[AttachedPlayer.Index];
        // 设置武将图片
        NameText.text = AttachedPlayer.Name; // + 武将名
        if (AttachedPlayer.IsAlive) {
            MoneyText.text = "￥" + AttachedPlayer.Money.ToString();
            CardText.text = "□" + AttachedPlayer.HandCardNumber.ToString();
            EquipText.text = "❀";
            JudgeText.text = "✪";
            FlagText.text = "";
        } else {
            MoneyText.text = "已阵亡";
        }
    }
}
