using System;
using System.Collections.Generic;

public class PAiTargetChooser {
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="Game"></param>
    /// <param name="Player"></param>
    /// <param name="Condition"></param>
    /// <param name="ExpectedMoney"></param>
    /// <param name="TagTsaangLang">是否会触发苍狼，即额外考虑获得一张牌的收益</param>
    /// <returns></returns>
    public static PPlayer InjureTarget(PGame Game, PPlayer Player, PTrigger.PlayerCondition Condition =null, int ExpectedMoney=0, bool TagTsaangLang = false) {

        return PMath.Max(Game.PlayerList.FindAll((PPlayer Target) => Target.IsAlive && (Condition == null || Condition(Game, Target))), (PPlayer Target) => {

            if (!Target.CanBeInjured || Player.OutOfGame) {
                return 0;
            }
            if (ExpectedMoney <= 1000 && Target.Traffic != null && Target.Traffic.Model is P_NanManHsiang) {
                return 0;
            }

            if (Target.Area.HandCardArea.CardNumber == 0 && Player.Weapon != null && Player.Weapon.Model is P_KuTingTao) {
                ExpectedMoney *= 2;
            }
            if (Target.Defensor != null && Target.Defensor.Model is P_PaKuaChevn) {
                ExpectedMoney = (PMath.Percent(ExpectedMoney, 50) + ExpectedMoney) / 2;
            }

            if (Target.Defensor != null && Target.Defensor.Model is P_PaiHuaChooon && !Player.Sex.Equals(Target.Sex)) {
                ExpectedMoney = PMath.Percent(ExpectedMoney, 50);
            }
            if (Player.General is P_ZhaoYun && Player.Tags.ExistTag(P_ZhaoYun.PDanTag.TagName) && (ExpectedMoney >= 3000 || ExpectedMoney >= Target.Money)) {
                ExpectedMoney = PMath.Percent(ExpectedMoney, 150);
            }
            if (Target.General is P_ZhaoYun && Target.Tags.ExistTag(P_ZhaoYun.PDanTag.TagName) && (ExpectedMoney >= 2000 || ExpectedMoney >= Target.Money)) {
                ExpectedMoney = PMath.Percent(ExpectedMoney, 50);
            }

            if (Player.General is P_ZhangSanFeng && Player.Tags.ExistTag(P_ZhangSanFeng.PYinTag.Name)) {
                ExpectedMoney += PMath.Percent(ExpectedMoney, 20);
            }
            if (Target.General is P_ZhangSanFeng && Target.Tags.ExistTag(P_ZhangSanFeng.PYangTag.Name)) {
                ExpectedMoney -= PMath.Percent(ExpectedMoney, 20);
            }

            int Profit = ExpectedMoney *2 + Math.Max(0, (20000 - Target.Money)/1000) + PMath.RandInt(0,9);
            bool SameTeam = (Target.TeamIndex == Player.TeamIndex);
            int Cof = (SameTeam ? -1 : 1);
            if (SameTeam) {
                Profit  = 0;
            }
            if (Target.Money <= 10000) {
                Profit += ExpectedMoney * Cof;
            }
            if (Target.Money <= 3000) {
                Profit += ExpectedMoney * Cof;
            }
            if (Target.Money <= ExpectedMoney) {
                Profit += 30000 * Cof;
            }

            if (TagTsaangLang || Player.Area.HandCardArea.CardList.Exists((PCard Card) => Card.Model is P_CheevnHuoTaChieh)) {
                Profit += Math.Max(0, PAiCardExpectation.FindMostValuableToGet(Game, Player, Target).Value);
            }

            if (Player.General is P_ChenYuanYuan) {
                Profit += 200;
            }
            if (Target.General is P_ChenYuanYuan) {
                Profit -= 200 * Cof;
            }

            // 加上受到、造成伤害触发技能的增益
            return Profit;
        }, true).Key;
    }
}