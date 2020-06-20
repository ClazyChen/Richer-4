using System;
using System.Collections.Generic;

public class P_YangYuHuan: PGeneral {

    public P_YangYuHuan() : base("杨玉环") {
        Sex = PSex.Female;
        Age = PAge.Renaissance;
        Index = 3;
        Tips = "定位：控制\n" +
            "难度：简单\n" +
            "史实：唐代宫廷音乐家、舞蹈家，中国古代四大美女之一，唐玄宗宠妃。\n" +
            "攻略：\n杨玉环是新手推荐的武将，使用简单，强度稳定，并且不弱。\n杨玉环可以通过主动调整来增加【羞花】的发动频率，【羞花】的条件1和3、4往往是互斥的，而条件2又和【品荔】冲突，因此收益期望并不高。如果要主动调整，建议只进行小幅度买房、出牌决策上的调整，因为【羞花】的收益远没有到值得大幅调整战略的程度。\n因为【羞花】的发动条件是“最少”，所以如果杨玉环进入到人数较少的阶段，比如只有两个玩家存活时，【羞花】的收益就变得很高。因此，杨玉环的嘲讽也相对较高。";

        PSkill PinLi = new PSkill("品荔");
        SkillList.Add(PinLi
            .AddTimeTrigger(
            new PTime[] {
                PPeriod.SettleStage.Start
            },
            (PTime Time, PPlayer Player, PSkill Skill) => {
                return new PTrigger(PinLi.Name) {
                    IsLocked = false,
                    Player = Player,
                    Time = Time,
                    AIPriority = 150,
                    Condition = (PGame Game) => {
                        return Player.Equals(Game.NowPlayer) && Player.Equals(Player.Position.Lord) && Player.Position.BusinessType.Equals(PBusinessType.Institute) && Game.PlayerList.Exists((PPlayer _Player) => _Player.Area.CardNumber > 0 && !_Player.Equals(Player) && _Player.IsAlive);
                    },
                    AICondition = (PGame Game) => {
                        return P_ShunShouChiienYang.AIBaseEmitTargets(Game, Player, 0)[0] != null;
                    },
                    Effect = (PGame Game) => {
                        PinLi.AnnouceUseSkill(Player);
                        PCard Card = new P_ShunShouChiienYang().Instantiate();
                        Card.Point = 0;
                        PTrigger Trigger = Card.Model.MoveInHandTriggerList.Find((Func<PPlayer, PCard, PTrigger> TriggerGenerator) => TriggerGenerator(Player, Card).Time.Equals(PPeriod.FirstFreeTime.During))?.Invoke(Player, Card);
                        if (Trigger != null) {
                            Game.Logic.StartSettle(new PSettle("品荔[顺手牵羊]", Trigger.Effect));
                        }
                    }
                };
            }));
        PSkill XiuHua = new PSkill("羞花") {
            SoftLockOpen = true
        };
        SkillList.Add(XiuHua
            .AddTrigger(
            (PPlayer Player, PSkill Skill) => {
                return new PTrigger(XiuHua.Name) {
                    IsLocked = false,
                    Player = Player,
                    Time = PPeriod.EndTurn.During,
                    AIPriority = 120,
                    Condition = (PGame Game) => {
                        return Player.Equals(Game.NowPlayer);
                    },
                    Effect = (PGame Game) => {
                        XiuHua.AnnouceUseSkill(Player);
                        List<PPlayer> AlivePlayers = Game.AlivePlayers(Player);
                        int X = new List<bool>() {
                            AlivePlayers.TrueForAll((PPlayer _Player) => _Player.Money > Player.Money),
                            AlivePlayers.TrueForAll((PPlayer _Player) => _Player.Area.HandCardArea.CardNumber > Player.Area.HandCardArea.CardNumber),
                            AlivePlayers.TrueForAll((PPlayer _Player) => _Player.LandNumber > Player.LandNumber),
                            AlivePlayers.TrueForAll((PPlayer _Player) => _Player.HouseNumber > Player.HouseNumber)
                        }.FindAll((bool x) => x).Count;
                        if (X > 0) {
                            Game.GetMoney(Player, 200 * X);
                        }
                    }
                };
            }));
    }

}