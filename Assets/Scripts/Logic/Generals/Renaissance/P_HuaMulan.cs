using System;
using System.Collections.Generic;

public class P_HuaMulan: PGeneral {

    public P_HuaMulan() : base("花木兰") {
        Sex = PSex.Female;
        Age = PAge.Renaissance;
        Index = 23;
        Cost = 20;
        Tips = "定位：控制\n" +
            "难度：待定\n" +
            "史实：出自经典诗歌《木兰辞》。中国古代替父从军的女英雄。\n" +
            "攻略：\n暂无";

        PSkill XiaoJi = new PSkill("枭姬") {
            SoftLockOpen = true
        };
        SkillList.Add(XiaoJi
            .AddTrigger((PPlayer Player, PSkill Skill) => {
                return new PTrigger(XiaoJi.Name) {
                    IsLocked = false,
                    Player = Player,
                    Time = PTime.Card.LeaveAreaTime,
                    AIPriority = 200,
                    Condition = (PGame Game) => {
                        PMoveCardTag MoveCardTag = Game.TagManager.FindPeekTag<PMoveCardTag>(PMoveCardTag.TagName);
                        return Player.Area.EquipmentCardArea.Equals(MoveCardTag.Source);
                    },
                    Effect = (PGame Game) => {
                        XiaoJi.AnnouceUseSkill(Player);
                        Game.GetCard(Player);
                    }
                };
            }));

        PPlayer YiZhuangTarget(PGame Game, PPlayer Player) {
            return PMath.Max(Game.AlivePlayers(Player), (PPlayer TargetPlayer) => {
                int Profit = 0;
                PSex OriginalSex = Player.Sex;
                Player.Sex = PSex.Male;
                foreach (PCardType CardType in new PCardType[] {
                    PCardType.WeaponCard, PCardType.DefensorCard, PCardType.TrafficCard
                }) {
                    PCard Card = Player.GetEquipment(CardType);
                    PCard TargetCard = TargetPlayer.GetEquipment(CardType);
                    if (Card != null) {
                        Profit += 2000;
                        Profit -= Card.AIInEquipExpectation(Game, Player);
                        Profit += Card.AIInEquipExpectation(Game, TargetPlayer) * (Player.TeamIndex == TargetPlayer.TeamIndex ? 1 : -1);
                    }
                    if (TargetCard != null) {
                        Profit += TargetCard.AIInEquipExpectation(Game, Player);
                        Profit -= TargetCard.AIInEquipExpectation(Game, TargetPlayer) * (Player.TeamIndex == TargetPlayer.TeamIndex ? 1 : -1);
                    }
                }
                Player.Sex = OriginalSex;
                return Profit - 6000;
            }, true).Key;
        }
        PSkill YiZhuang = new PSkill("易装") {
            Initiative = true
        };
        SkillList.Add(YiZhuang
            .AnnouceGameOnce()
            .AddTimeTrigger(
            new PTime[] {
                PPeriod.FirstFreeTime.During,
                PPeriod.SecondFreeTime.During
            },
            (PTime Time, PPlayer Player, PSkill Skill) => {
                return new PTrigger(YiZhuang.Name) {
                    IsLocked = false,
                    Player = Player,
                    Time = Time,
                    AIPriority = 280,
                    CanRepeat = true,
                    Condition = (PGame Game) => {
                        return Player.Equals(Game.NowPlayer) && (Player.IsAI || Game.Logic.WaitingForEndFreeTime()) && Player.RemainLimit(YiZhuang.Name);
                    },
                    AICondition = (PGame Game) => {
                        return YiZhuangTarget(Game, Player) != null;
                    },
                    Effect = (PGame Game) => {
                        YiZhuang.AnnouceUseSkill(Player);
                        PPlayer Target = null;
                        if (Player.IsAI) {
                            Target = YiZhuangTarget(Game, Player);
                        } else {
                            Target = PNetworkManager.NetworkServer.ChooseManager.AskForTargetPlayer(Player, PTrigger.Except(Player), YiZhuang.Name);
                        }
                        if (Target != null) {
                            List<PCard> MulanEquipments = new List<PCard>();
                            foreach (PCard Card in Player.Area.EquipmentCardArea.CardList) {
                                MulanEquipments.Add(Card);
                            }
                            List<PCard> TargetEquipements = new List<PCard>();
                            foreach (PCard Card in Target.Area.EquipmentCardArea.CardList) {
                                TargetEquipements.Add(Card);
                            }
                            foreach (PCard Card in MulanEquipments) {
                                Game.CardManager.MoveCard(Card, Player.Area.EquipmentCardArea, Game.CardManager.SettlingArea);
                            }
                            foreach (PCard Card in TargetEquipements) {
                                Game.CardManager.MoveCard(Card, Target.Area.EquipmentCardArea, Game.CardManager.SettlingArea);
                            }
                            foreach (PCard Card in MulanEquipments) {
                                Game.CardManager.MoveCard(Card, Game.CardManager.SettlingArea, Target.Area.EquipmentCardArea);
                            }
                            foreach (PCard Card in TargetEquipements) {
                                Game.CardManager.MoveCard(Card, Game.CardManager.SettlingArea, Player.Area.EquipmentCardArea);
                            }
                            Player.Sex = PSex.Male;
                            Player.Tags.CreateTag(new PTag(YiZhuang.Name));
                            Player.Tags.FindPeekTag<PUsedTag>(PUsedTag.TagNamePrefix + YiZhuang.Name).Count++;
                        }
                    }
                };
            }));
    }

}