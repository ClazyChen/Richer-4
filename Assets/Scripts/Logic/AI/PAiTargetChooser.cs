using System;
using System.Collections.Generic;

public class PAiTargetChooser {
    
    public static PPlayer InjureTarget(PGame Game, PPlayer Player, PTrigger.PlayerCondition Condition =null, int ExpectedMoney=0) {

        return PMath.Max(Game.PlayerList.FindAll((PPlayer Target) => Target.IsAlive && (Condition == null || Condition(Game, Target))), (PPlayer Target) => {

            if (!Target.CanBeInjured || Player.OutOfGame) {
                return 0;
            }
            if (Target.Area.HandCardArea.CardNumber == 0 && Player.Weapon != null && Player.Weapon.Model is P_KuTingTao) {
                ExpectedMoney *= 2;
            }

            int Profit = ExpectedMoney *2 + Math.Max(0, (20000 - Target.Money)/1000) + PMath.RandInt(0,9);
            bool SameTeam = (Target.TeamIndex == Player.TeamIndex);
            int Cof = (SameTeam ? -1 : 1);
            if (SameTeam) {
                Profit -= ExpectedMoney * 2;
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
            // 加上受到、造成伤害触发技能的增益
            return Profit;
        }, true).Key;
    }
}