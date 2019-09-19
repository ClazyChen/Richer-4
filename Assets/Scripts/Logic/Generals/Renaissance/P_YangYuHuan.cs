using System;
using System.Collections.Generic;

public class P_YangYuHuan: PGeneral {

    public P_YangYuHuan() : base("杨玉环") {
        Sex = PSex.Female;
        Age = PAge.Renaissance;
        Index = 3;
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
                        List<PPlayer> AlivePlayers = Game.PlayerList.FindAll((PPlayer _Player) => _Player.IsAlive && !_Player.Equals(Player));
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