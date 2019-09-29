using System;
using System.Collections.Generic;

public class P_Temujin: PGeneral {

    public P_Temujin() : base("铁木真") {
        Sex = PSex.Male;
        Age = PAge.Renaissance;
        Index = 11;
        PSkill QiangLve = new PSkill("抢掠") {
            SoftLockOpen = true
        };
        SkillList.Add(QiangLve
            .AddTrigger(
            (PPlayer Player, PSkill Skill) => {
                return new PTrigger(QiangLve.Name) {
                    IsLocked = false,
                    Player = Player,
                    Time = PTime.Injure.EmitInjure,
                    AIPriority = 15,
                    Condition = (PGame Game) => {
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        return InjureTag.Injure > 0 && InjureTag.ToPlayer != null && Player.Equals(InjureTag.FromPlayer) &&
                        InjureTag.InjureSource is PBlock && !Player.Equals(InjureTag.ToPlayer) && InjureTag.ToPlayer.Area.HandCardArea.CardNumber + InjureTag.ToPlayer.Area.EquipmentCardArea.CardNumber > 0;
                    },
                    AICondition = (PGame Game) => {
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        return InjureTag.ToPlayer.TeamIndex != Player.TeamIndex ;
                    },
                    Effect = (PGame Game) => {
                        QiangLve.AnnouceUseSkill(Player);
                        int Result = Game.Judge(Player);
                        if (Result %2 == 0) {
                            PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                            PCard Card = new P_CheevnHuoTaChieh().Instantiate();
                            Card.Point = 0;
                            PTrigger Trigger = Card.Model.MoveInHandTriggerList.Find((Func<PPlayer, PCard, PTrigger> TriggerGenerator) => TriggerGenerator(Player, Card).Time.Equals(PTime.Injure.EmitInjure))?.Invoke(Player, Card);
                            if (Trigger != null) {
                                Game.Logic.StartSettle(new PSettle("抢掠[趁火打劫]", Trigger.Effect));
                            }
                        }
                    }
                };
            }));
    }

}