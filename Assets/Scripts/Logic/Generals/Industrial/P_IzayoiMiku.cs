﻿using System;
using System.Collections.Generic;

public class P_IzayoiMiku: PGeneral {

    public class PSoloTag : PTag
    {
        public static string TagName = "独奏";
        public PSoloTag() : base(TagName) { }
    }

    public P_IzayoiMiku() : base("诱宵美九") {
        Sex = PSex.Female;
        Age = PAge.Industrial;
        Index = 28;
        Cost = 50;
        Tips = "定位：攻击\n" +
            "难度：简单\n" +
            "史实：轻小说《约会大作战》中女主角。龙胆寺女子学院学生，歌手，偶像。\n" +
            "攻略：\n【独奏】可以压制住全部的防御牌，既不能被闪避也不能被减伤，还无法通过【借尸还魂】、【走为上计】自救，一定程度上使得项羽、廉颇、屈原一类的自伤角色不敢铤而走险。贞德的【圣女】能够对【独奏】形成一定程度的克制。如果美九有【古锭刀】，则每一次过路费都触发【古锭刀】技能的效果，从而成为很强的火力点。新版美九的【独奏】无法触发花木兰的【枭姬】，但花木兰仍可以通过【易装】给她【古锭刀】来形成配合。\n美九的团队通常需要建一座城堡来发动【轮舞曲】。如果在未来大概率拿不到新的商业用地的情况下，即使是3级城堡，在【轮舞曲】之后也相当于免费升了2级的购物中心。";

        NewGeneral = true;

        PSkill Solo = new PSkill("独奏");
        SkillList.Add(Solo
            .AddTrigger(
            (PPlayer Player, PSkill Skill) => {
                return new PTrigger(Solo.Name) {
                    IsLocked = false,
                    Player = Player,
                    Time = PTime.Injure.BeforeEmitInjure,
                    AIPriority = 200,
                    Condition = (PGame Game) => {
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        return Player.Equals(InjureTag.FromPlayer) && InjureTag.Injure > 0 && InjureTag.ToPlayer != null && InjureTag.ToPlayer.Area.OwnerCardNumber > 0;
                    },
                    AICondition = (PGame Game) => {
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        PPlayer ToPlayer = InjureTag.ToPlayer;
                        return ToPlayer.TeamIndex != Player.TeamIndex || (ToPlayer.General is P_Gabriel);
                    },
                    Effect = (PGame Game) => {
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        PPlayerCardArea ToPlayerArea = InjureTag.ToPlayer.Area;
                        Solo.AnnouceUseSkill(Player);
                        Game.CardManager.MoveAll(ToPlayerArea.HandCardArea, ToPlayerArea.OutOfGameArea);
                        // Game.CardManager.MoveAll(ToPlayerArea.EquipmentCardArea, ToPlayerArea.OutOfGameArea);
                        Player.Tags.CreateTag(new PSoloTag());
                    }
                };
            })
            .AddTrigger(
            (PPlayer Player, PSkill Skill) => {
                return new PTrigger(Solo.Name) {
                    IsLocked = true,
                    Player = Player,
                    Time = PTime.Injure.EndSettle,
                    AIPriority = 200,
                    Condition = (PGame Game) => {
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        return Player.Equals(InjureTag.FromPlayer) && InjureTag.Injure > 0 && InjureTag.ToPlayer != null && Player.Tags.ExistTag(PSoloTag.TagName);
                    },
                    Effect = (PGame Game) => {
                        Solo.AnnouceUseSkill(Player);
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        PPlayer ToPlayer = InjureTag.ToPlayer;
                        Player.Tags.PopTag<PSoloTag>(PSoloTag.TagName);
                        if (ToPlayer.IsAlive) {
                            Game.CardManager.MoveAll(ToPlayer.Area.OutOfGameArea, ToPlayer.Area.HandCardArea);
                        } else {
                            Game.CardManager.MoveAll(ToPlayer.Area.OutOfGameArea, Player.Area.HandCardArea);
                            PPlayer ExtraTarget = null;
                            int ExtraInjure = PMath.Percent(InjureTag.Injure, 50);
                            if (Player.IsAI) {
                                ExtraTarget = PAiTargetChooser.InjureTarget(Game, Player, Player, PTrigger.Except(Player), ExtraInjure, Solo);
                            } else {
                                ExtraTarget = PNetworkManager.NetworkServer.ChooseManager.AskForTargetPlayer(Player, PTrigger.Except(Player), Solo.Name + "[造成" + ExtraInjure.ToString() + "点伤害]", true);
                            }
                            if (ExtraTarget != null) {
                                Game.Injure(Player, ExtraTarget, ExtraInjure, Solo);
                            }
                        }
                    }
                };
            }));

        PSkill Rando = new PSkill("轮舞曲") {
            Initiative = true
        };
        SkillList.Add(Rando
            .AnnounceGameOnce()
            .AddTimeTrigger(
            new PTime[] {
                PPeriod.FirstFreeTime.During,
                PPeriod.SecondFreeTime.During
            },
            (PTime Time, PPlayer Player, PSkill Skill) => {
                return new PTrigger(Rando.Name) {
                    IsLocked = false,
                    Player = Player,
                    Time = Time,
                    AIPriority = 280,
                    CanRepeat = true,
                    Condition = (PGame Game) => {
                        return Player.Equals(Game.NowPlayer) && (Player.IsAI || Game.Logic.WaitingForEndFreeTime()) && Player.RemainLimit(Rando.Name) &&
                        Game.Map.BlockList.Exists((PBlock Block) => Block.Lord != null && Block.IsBusinessLand);
                    },
                    AICondition = (PGame Game) => {
                        PBlock MaxHouseBlock = PMath.Max(Game.Map.BlockList.FindAll((PBlock Block) => Block.Lord != null && Block.IsBusinessLand && Block.Lord.TeamIndex == Player.TeamIndex && !Block.BusinessType.Equals(PBusinessType.ShoppingCenter)), (PBlock Block) => Block.HouseNumber, true).Key;
                        if (MaxHouseBlock == null) {
                            return false;
                        }
                        int ClubBonus = PMath.Percent(MaxHouseBlock.Price, 40) * Game.Enemies(Player).Count * MaxHouseBlock.HouseNumber;
                        if (MaxHouseBlock.BusinessType.Equals(PBusinessType.Institute)) {
                            ClubBonus -= 4000 * Game.Teammates(Player).Count;
                        }
                        int TeammateCardBonus = -2000 * PMath.Sum(Game.Teammates(Player, false).ConvertAll((PPlayer _Player) => Math.Min(2, _Player.Area.HandCardArea.CardNumber)));
                        int EnemyCardBonus = 2000 * PMath.Sum(Game.Enemies(Player).ConvertAll((PPlayer _Player) => Math.Min(2, _Player.Area.HandCardArea.CardNumber)));
                        int SumBonus = ClubBonus + TeammateCardBonus + EnemyCardBonus;
                        return SumBonus >= 8000 * (1 + Game.Map.BlockList.FindAll((PBlock _Block) => _Block.IsBusinessLand && _Block.Lord == null).Count);
                    },
                    Effect = (PGame Game) => {
                        Rando.AnnouceUseSkill(Player);
                        Game.Traverse((PPlayer _Player) => {
                            if (_Player != Player) {
                                for (int i = 0; i < 2; ++ i) {
                                    if (_Player.Area.HandCardArea.CardNumber > 0) {
                                        Game.ThrowCard(_Player, _Player, true, false, false);
                                    }
                                }
                            }
                        }, Player);
                        PBlock Target = null;
                        if (Player.IsAI) {
                            PBlock MaxHouseBlock = PMath.Max(Game.Map.BlockList.FindAll((PBlock Block) => Block.Lord != null && Block.IsBusinessLand && Block.Lord.TeamIndex == Player.TeamIndex && !Block.BusinessType.Equals(PBusinessType.ShoppingCenter)), (PBlock Block) => Block.HouseNumber, true).Key;
                            Target = MaxHouseBlock;
                        } else {
                            Target = PNetworkManager.NetworkServer.ChooseManager.AskToChooseBlock(Player, Rando.Name + "-选择一处商业用地", (PBlock Block) => Block.IsBusinessLand && Block.Lord != null);
                        }
                        Target.BusinessType = PBusinessType.Club;
                        PNetworkManager.NetworkServer.TellClients(new PRefreshBlockBasicOrder(Target));
                        Rando.DeclareUse(Player);
                    }
                };
            }));
    }

}