using System;
using System.Collections.Generic;

public class PAiTargetChooser {


    /// <summary>
    /// 伤害预测收益，无修正情形为基本伤害的2倍or0
    /// </summary>
    /// <param name="Game"></param>
    /// <param name="Player">视点玩家</param>
    /// <param name="FromPlayer">造成伤害的玩家</param>
    /// <param name="Target"></param>
    /// <param name="BaseInjure">基本伤害量</param>
    /// <param name="Source">伤害方式</param>
    /// <returns></returns>
    public static int InjureExpect(PGame Game, PPlayer Player, PPlayer FromPlayer, PPlayer Target, int BaseInjure, PObject Source) {
        #region 防止伤害的情况
        if (Player.OutOfGame || 
            !Target.CanBeInjured ||
            (FromPlayer == null && Target.HasEquipment<P_YinYangChing>()) ||
            (BaseInjure <= 1000 && Target.HasEquipment<P_NanManHsiang>())) {
            return 0;
        }
        #endregion
        int FromCof = FromPlayer == null ? 0 : (Player.TeamIndex == FromPlayer.TeamIndex ? 1 : -1);
        int ToCof = (Player.TeamIndex != Target.TeamIndex ? 1 : -1);
        int Sum = 0;
        #region 造成伤害时发动的技能：古锭刀，龙胆，太极，苍狼，趁火打劫
        if (FromPlayer != null) {
            if (Target.Area.HandCardArea.CardNumber == 0 && FromPlayer.HasEquipment<P_KuTingTao>() && Source is PBlock) {
                BaseInjure *= 2;
            }
            if (FromPlayer.General is P_ZhaoYun && FromPlayer.Tags.ExistTag(P_ZhaoYun.PDanTag.TagName)) {
                if (P_ZhaoYun.LongDanICondition(Game, FromPlayer, Target, BaseInjure)) {
                    BaseInjure = PMath.Percent(BaseInjure, 150);
                }
            }
            if (FromPlayer.General is P_ZhangSanFeng && Player.Tags.ExistTag(P_ZhangSanFeng.PYinTag.Name)) {
                BaseInjure += PMath.Percent(BaseInjure, 20);
            }
        }
        if (Target.Area.OwnerCardNumber > 0) {
            if (FromPlayer != null && FromPlayer.HasEquipment<P_TsaangLang>() && Source != null && Source is PCard && ((PCard)Source).Model is PSchemeCardModel) {
                Sum += 2000 * FromCof + 2000 * ToCof;
            }
            if (Player.HasInHand<P_CheevnHuoTaChieh>()) {
                Sum += 2000 + 2000 * ToCof;
            }
        }
        #endregion
        #region 受到伤害时发动的技能：八卦阵，百花裙，龙胆，太极
        if (Target.HasEquipment<P_PaKuaChevn>()) {
            if (Target.General is P_LiuJi) {
                Sum -= 1000 * ToCof;
                BaseInjure = PMath.Percent(BaseInjure, 50);
            } else {
                BaseInjure = (BaseInjure + PMath.Percent(BaseInjure, 50)) / 2;
            }
        }
        if (Target.HasEquipment<P_PaiHuaChooon>() && FromPlayer != null && !Target.Sex.Equals(FromPlayer.Sex)) {
            BaseInjure = PMath.Percent(BaseInjure, 50);
        }
        if (Target.General is P_ZhaoYun && Target.Tags.ExistTag(P_ZhaoYun.PDanTag.TagName)) {
            if (P_ZhaoYun.LongDanIICondition(Game, Target, FromPlayer, BaseInjure)) {
                BaseInjure = PMath.Percent(BaseInjure, 50);
            }
        }
        if (Target.General is P_ZhangSanFeng && Target.Tags.ExistTag(P_ZhangSanFeng.PYangTag.Name)) {
            BaseInjure -= PMath.Percent(BaseInjure, 20);
        }
        #endregion

        int ExpectTargetMoney = Target.Money - BaseInjure;
        if (ExpectTargetMoney <= 0) {
            Sum += 30000 * ToCof;
        }
        Sum += BaseInjure * ToCof;
        Sum += BaseInjure * FromCof;

        #region 伤害结束后：风云
        if (FromPlayer != null && FromPlayer.General is P_ChenYuanYuan) {
            Sum += 200 * FromCof;
        }
        if (Target.General is P_ChenYuanYuan) {
            Sum -= 200 * ToCof;
        }
        #endregion

        #region 队友间平衡：自身和目标的合理阈值为50%-200%
        if (FromPlayer.TeamIndex == Target.TeamIndex) {
            if (FromPlayer.Money > Target.Money * 2) {
                Sum -= 1000;
            } else if (FromPlayer.Money < Target.Money * 2) {
                Sum += 1000;
            }
        }
        #endregion
        return Sum;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Game"></param>
    /// <param name="Player"></param>
    /// <param name="Condition"></param>
    /// <param name="ExpectedMoney"></param>
    /// <returns></returns>
    public static PPlayer InjureTarget(PGame Game, PPlayer FromPlayer, PPlayer Player, PTrigger.PlayerCondition Condition =null, int ExpectedMoney=0, PObject Source = null, bool AllowNegative = false) {

        return PMath.Max(Game.PlayerList.FindAll((PPlayer Target) => Target.IsAlive && (Condition == null || Condition(Game, Target))), (PPlayer Target) => {

            return InjureExpect(Game, Player, FromPlayer, Target, ExpectedMoney, Source) + PMath.RandInt(0,10);
        }, !AllowNegative).Key;
    }
}