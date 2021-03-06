﻿using UnityEngine;

/// <summary>
/// PBlockScene类：
/// 用来实现一个格子
/// </summary>
public class PBlockScene : PAbstractUI{
    public class Config {
        public readonly static Color DefaultBlockColor = new Color(0.66f, 0.66f, 0.66f);
        public readonly static Color HighlightedBlockColor = new Color(1.0f, 1.0f, 1.0f);
    }

    public readonly TextMesh BlockName;
    public readonly TextMesh BlockType;
    public readonly TextMesh BlockNumber;
    public readonly Transform BlockImage;

    public PBlockScene(Transform _Background) : base(_Background) {
        InitializeControls<TextMesh>();
        BlockImage = UIBackgroundImage.Find("BlockImage");
        Close();
    }

    public override void Open() {
        base.Open();
        BlockImage.gameObject.GetComponent<MeshRenderer>().material.color = Config.DefaultBlockColor;
    }

    public void InitializeBlock(PBlock Block) {
        BlockName.text = GetInformationText(Block);
        BlockNumber.text = Block.CanPurchase && Block.Lord != null ? Block.HouseNumber.ToString() : string.Empty;
        BlockType.text = Block.BusinessType.Equals(PBusinessType.NoType) ? string.Empty : Block.BusinessType.Name;
        UIBackgroundImage.position = GetSpacePosition(Block);
        SetColor(Block.Lord == null ? Config.DefaultBlockColor : PPlayerScene.Config.PlayerColors[Block.Lord.Index]);
    }

    /// <summary>
    /// 获取格子的空间位置
    /// </summary>
    public static Vector3 GetSpacePosition(PBlock Block) {
        return new Vector3(10.0f * Block.Y, 0.0f, 10.0f * Block.X);
    }

    private static string GetInformationText(PBlock Block) {
        string ret = Block.Name;
        if (Block.Name.Equals("COMMERCIAL LAND") || Block.Name.Equals("LAND")) {
            ret += "\n" + Block.Price.ToString();
        }
        if (Block.GetCardStop > 0) {
            ret += "\n摸" + Block.GetCardStop + "牌";
        }
        if (Block.Name.Equals("BONUS")) {
            if (Block.GetMoneyStopSolid != 0) {
                ret += "\n+" + Block.GetMoneyStopSolid.ToString();
            }
            if (Block.GetMoneyStopPercent != 0) {
                ret += "\n+" + Block.GetMoneyStopPercent.ToString() + "%";
            }
            return ret;
        }
        if (Block.Name.Equals("DISASTER")) {
            if (Block.GetMoneyStopSolid != 0) {
                ret += "\n-" + (-Block.GetMoneyStopSolid).ToString();
            }
            if (Block.GetMoneyStopPercent != 0) {
                ret += "\n-" + (-Block.GetMoneyStopPercent).ToString() + "%";
            }
        }
        return ret;
    }
}
