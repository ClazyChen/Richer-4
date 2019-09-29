using System;
using System.Collections.Generic;

public class P_Temujin: PGeneral {

    public P_Temujin() : base("铁木真") {
        Sex = PSex.Male;
        Age = PAge.Renaissance;
        Index = 11;
        Cost = 20;
        Tips = "定位：控制\n" +
            "难度：简单\n" +
            "史实：大蒙古国可汗，杰出的军事家、政治家，统一了蒙古诸部，开疆拓土，远至东欧，被尊为“成吉思汗”。\n" +
            "攻略：\n铁木真是一个新手推荐武将，基本上使用没有任何难度。";

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
                        int Result = Game.Judge(Player, 6);
                        if (Result %2 == 0) {
                            PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                            PCard Card = new P_CheevnHuoTaChieh().Instantiate();
                            Card.Point = 0;
                            PTrigger Trigger = Card.Model.MoveInHandTriggerList.Find((Func<PPlayer, PCard, PTrigger> TriggerGenerator) => TriggerGenerator(Player, Card).Time.Equals(PTime.Injure.AcceptInjure))?.Invoke(Player, Card);
                            if (Trigger != null) {
                                Game.Logic.StartSettle(new PSettle("抢掠[趁火打劫]", Trigger.Effect));
                            }
                        }
                    }
                };
            }));
    }

}