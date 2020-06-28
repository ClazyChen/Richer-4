using System;
using System.Collections.Generic;

public class P_HuaMulan: PGeneral {

    public P_HuaMulan() : base("花木兰") {
        Sex = PSex.Female;
        Age = PAge.Renaissance;
        Index = 23;
        Cost = 20;
        Tips = "定位：爆发\n" +
            "难度：简单\n" +
            "史实：出自经典诗歌《木兰辞》。中国古代替父从军的女英雄。\n" +
            "攻略：\n花木兰是一名拥有不俗爆发能力的武将。【枭姬】的存在使得花木兰极度依赖装备牌，因此能够与唐寅、时迁等武将配合，同时也能够对这些武将形成强大的威慑力。【枭姬】往往能够提供绝地反杀的能力，或能使花木兰东山再起，因此在针对花木兰时必须注意其队伍的研究所或给牌武将，同时慎用【落凤弓】、【借刀杀人】等牌，从一定程度上来说提供了花木兰一定的防御力。【易装】是一个强大的爆发技能，不仅能与队友交换装备达成爆发，还能抢夺敌方的大量装备，形成局势逆转，因此可在一定程度上克制关羽。【易装】还可用来转换性别，从而使敌方的【百花裙】等装备失效。针对花木兰必须阻止其装备成形，因此吕蒙是一个不错的选择。";

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
                        return Player.Area.EquipmentCardArea.Equals(MoveCardTag.Source) && !Player.Equals(MoveCardTag.Destination.Owner);
                    },
                    Effect = (PGame Game) => {
                        XiaoJi.AnnouceUseSkill(Player);
                        Game.GetCard(Player);
                        Game.GetMoney(Player, 1500);
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
                        Profit -= Card.Model.AIInEquipExpectation(Game, Player);
                        Profit += Card.Model.AIInEquipExpectation(Game, TargetPlayer) * (Player.TeamIndex == TargetPlayer.TeamIndex ? 1 : -1);
                    }
                    if (TargetCard != null) {
                        Profit += TargetCard.Model.AIInEquipExpectation(Game, Player);
                        Profit -= TargetCard.Model.AIInEquipExpectation(Game, TargetPlayer) * (Player.TeamIndex == TargetPlayer.TeamIndex ? 1 : -1);
                    }
                }
                Player.Sex = OriginalSex;
                return Profit - 7500;
            }, true).Key;
        }
        PSkill YiZhuang = new PSkill("易装") {
            Initiative = true
        };
        SkillList.Add(YiZhuang
            .AnnounceGameOnce()
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
                        if (Game.Teammates(Player).Exists((PPlayer _Player) => _Player.General is P_WuZhao)) {
                            PPlayer _Player = Game.PlayerList.Find((PPlayer __Player) => __Player.General is P_WuZhao);
                            if (_Player.RemainLimit("女权")) {
                                return false;
                            } else if (Player.Tags.ExistTag(P_WuZhao.NvQuanTag.Name)) {
                                return false;
                            }
                        }
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
                            YiZhuang.DeclareUse(Player);
                        }
                    }
                };
            }));
    }

}