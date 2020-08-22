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
        #region 造成伤害时发动的技能：古锭刀，龙胆，太极，苍狼，趁火打劫，女权，怒斩
        if (FromPlayer != null) {
            if (FromPlayer.Tags.ExistTag(P_WuZhao.NvQuanTag.Name) && (Source is PBlock || Source is PCard)) {
                BaseInjure += 2000;
            }
            if (FromPlayer.General is P_Gryu && Source is PBlock && FromPlayer.Area.EquipmentCardArea.CardNumber > Target.Area.EquipmentCardArea.CardNumber) {
                BaseInjure += 600;
            }
            if ((Target.Area.HandCardArea.CardNumber == 0 || (FromPlayer.General is P_IzayoiMiku && FromPlayer.TeamIndex != Target.TeamIndex)) && FromPlayer.HasEquipment<P_KuTingTao>() && Source is PBlock) {
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
            if (FromPlayer != null && FromPlayer.HasEquipment<P_TsaangLang>() && Source != null && Source is PCard Card && Card.Model is PSchemeCardModel) {
                Sum += 2000 * FromCof + 2000 * ToCof;
            }
            if (Player.HasInHand<P_CheevnHuoTaChieh>()) {
                Sum += 2000 + 2000 * ToCof;
            }
        }
        #endregion
        #region 受到伤害时发动的技能：八卦阵，百花裙，龙胆，太极，霸王，白衣，镇魂曲
        if (!(Target.TeamIndex != FromPlayer.TeamIndex && FromPlayer.General is P_IzayoiMiku)) {
            // 美九无视八卦阵百花裙
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
        } 
        if (Target.General is P_ZhaoYun && Target.Tags.ExistTag(P_ZhaoYun.PDanTag.TagName)) {
            if (P_ZhaoYun.LongDanIICondition(Game, Target, FromPlayer, BaseInjure)) {
                BaseInjure = PMath.Percent(BaseInjure, 50);
            }
        }
        if (Target.General is P_ZhangSanFeng && Target.Tags.ExistTag(P_ZhangSanFeng.PYangTag.Name)) {
            BaseInjure -= PMath.Percent(BaseInjure, 20);
        }
        if (Game.AlivePlayers().Exists((PPlayer _Player) => {
            return !_Player.Equals(Target) && _Player.TeamIndex != Target.TeamIndex && _Player.Distance(Target) <= 1 && _Player.General is P_Xdyu;
        })) {
            BaseInjure += 800;
        }
        if (Target.General is P_LvMeng && Target.Area.EquipmentCardArea.CardNumber > 0 && !(FromPlayer.General is P_IzayoiMiku && FromPlayer.TeamIndex != Target.TeamIndex)) {
            BaseInjure = Math.Min(PMath.Percent(BaseInjure, 50) + 2000, BaseInjure);
        }
        if (Target.General is P_IzayoiMiku && Game.AlivePlayersExist<P_Gabriel>()) {
            BaseInjure -= PMath.Percent(BaseInjure, 20);
        }
        #endregion

        int ExpectTargetMoney = Target.Money - BaseInjure;

        #region 濒死时发动的技能：蓄谋，精灵加护，圣女
        if (ExpectTargetMoney <= 0) {
            bool flag = true;
            if (!(FromPlayer.General is P_LvZhi)) {
                if (Target.General is P_Gabriel) {
                    flag = false;
                    // 破军歌姬的复活
                    if (FromPlayer.Equals(Target)) {
                        // 对自己伤害，不触发复活
                    } else if (FromPlayer.TeamIndex == Target.TeamIndex) {
                        // 美九自己的伤害，触发复活大利好
                        Sum += 15000;
                    } else {
                        // 对方的伤害
                        if (Game.AlivePlayersExist<P_IzayoiMiku>()) {
                            // 美九未死，大不利
                            Sum -= 15000;
                        } else {
                            flag = true;
                        }
                    }
                } else if (Game.AlivePlayersExist<P_JeanneDarc>()) {
                    PPlayer Jeanne = Game.AlivePlayers().Find((PPlayer _Player) => _Player.General is P_JeanneDarc);
                    if (Jeanne.TeamIndex == Target.TeamIndex && Jeanne.Area.OwnerCardNumber > 0 && Jeanne.Money > 5000) {
                        flag = false;
                        Sum += 3000 + 2000 * Jeanne.Area.OwnerCardNumber;
                    }
                }
            }
            if (flag) {
                Sum += 30000 * ToCof;
            }
        }
        #endregion
        Sum += BaseInjure * ToCof;
        Sum += BaseInjure * FromCof;


        #region 受到伤害后：离骚
        if (Target.General is P_QuYuan) {
            // Sum -= 900 * ToCof;
            // 因为离骚总是会发动，其他伤害也会触发，天灾也会触发
            // 所以不应该将离骚计入伤害计算
        }
        #endregion
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
            if (FromPlayer.General is P_IzayoiMiku && Target.General is P_Gabriel) {
                // 美九对破军歌姬的伤害，积极性
                Sum += 2000;
            } else if (FromPlayer.General is P_Gabriel && Target.General is P_IzayoiMiku) {
                // 破军歌姬对美九的伤害，消极性
                Sum -= 20000;
            } else {
                if (FromPlayer.Money > ExpectTargetMoney * 2) {
                    Sum -= 2000;
                } else if (FromPlayer.Money >= Target.Money) {
                    Sum -= 100;
                } else if (FromPlayer.Money < Target.Money * 2) {
                    Sum += 1000;
                }
            }
        }
        #endregion

        #region 美九歌厅的翻面效果
        if (Source is PBlock Block && Block.BusinessType.Equals(PBusinessType.Club)) {
            Sum += PAiMapAnalyzer.ChangeFaceExpect(Game, Target) * (-ToCof);
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