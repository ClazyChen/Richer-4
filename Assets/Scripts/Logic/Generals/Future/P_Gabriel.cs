using System;
using System.Collections.Generic;

public class P_Gabriel : PGeneral
{

    public class PElfPowerTag : PNumberedTag
    {
        public static string TagName = "精灵力量";
        public PElfPowerTag() : base(TagName, 1) {

        }
    }

    public P_Gabriel() : base("破军歌姬") {
        Sex = PSex.Female;
        Age = PAge.Future;
        Index = 2001;
        Cost = 1;
        Tips = string.Empty;
        CanBeChoose = false;

        PSkill ElfMiku = new PSkill("精灵加护·美九") {
            Lock = true
        };
        SkillList.Add(ElfMiku
            .AddTrigger(
            (PPlayer Player, PSkill Skill) => {
                return new PTrigger(ElfMiku.Name) {
                    IsLocked = true,
                    Player = Player,
                    Time = PTime.EnterDyingTime,
                    AIPriority = 100,
                    Condition = (PGame Game) => {
                        return Game.TagManager.FindPeekTag<PDyingTag>(PDyingTag.TagName).Player.Equals(Player) && Game.AlivePlayersExist<P_IzayoiMiku>();
                    },
                    Effect = (PGame Game) => {
                        ElfMiku.AnnouceUseSkill(Player);
                        int TargetMoney = 10000;
                        int Bonus = TargetMoney - Player.Money;
                        if (Bonus > 0) {
                            Game.GetMoney(Player, Bonus);
                        }
                        Player.Tags.CreateTag(new PElfPowerTag());
                    }
                };
            }));

        PSkill AngelAwakeMiku = new PSkill("音之天使·觉醒") {
            Lock = true
        };
        SkillList.Add(AngelAwakeMiku
            .AnnounceGameOnce()
            .AddTrigger(
            (PPlayer Player, PSkill Skill) => {
                return new PTrigger(AngelAwakeMiku.Name) {
                    IsLocked = true,
                    Player = Player,
                    Time = PPeriod.StartTurn.During,
                    AIPriority = 200,
                    Condition = (PGame Game) => {
                        PElfPowerTag ElfPowerTag = Player.Tags.FindPeekTag<PElfPowerTag>(PElfPowerTag.TagName);
                        return Game.NowPlayer.Equals(Player) && Player.RemainLimit(AngelAwakeMiku.Name) && ElfPowerTag != null && ElfPowerTag.Value >= 3 && Game.AlivePlayers().Exists((PPlayer _Player) => _Player.General is P_IzayoiMiku);
                    },
                    Effect = (PGame Game) => {
                        AngelAwakeMiku.AnnouceUseSkill(Player);
                        PPlayer Miku = Game.AlivePlayers().Find((PPlayer _Player) => _Player.General is P_IzayoiMiku);
                        Game.Map.BlockList.ForEach((PBlock Block) => {
                            if (Block.IsBusinessLand) {
                                Block.Lord = Miku;
                                Block.BusinessType = PBusinessType.Club;
                                PNetworkManager.NetworkServer.TellClients(new PRefreshBlockBasicOrder(Block));
                            }
                        });
                        AngelAwakeMiku.DeclareUse(Player);
                    }
                };
            }));

        PSkill Requiem = new PSkill("镇魂曲") {
            Lock = true
        };
        SkillList.Add(Requiem
            .AddTrigger(
            (PPlayer Player, PSkill Skill) => {
                return new PTrigger(Requiem.Name) {
                    IsLocked = true,
                    Player = Player,
                    Time = PTime.Injure.AcceptInjure,
                    AIPriority = 255,
                    Condition = (PGame Game) => {
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        return InjureTag.Injure > 0 && InjureTag.ToPlayer != null && InjureTag.ToPlayer.General is P_IzayoiMiku;
                    },
                    Effect = (PGame Game) => {
                        Requiem.AnnouceUseSkill(Player);
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        InjureTag.Injure -= PMath.Percent(InjureTag.Injure, 20);
                    }
                };
            }));

        PSkill March = new PSkill("进行曲") {
            Lock = true
        };
        SkillList.Add(March
            .AddTrigger(
            (PPlayer Player, PSkill Skill) => {
                return new PTrigger(March.Name) {
                    IsLocked = true,
                    Player = Player,
                    Time = PPeriod.EndTurn.During,
                    AIPriority = 255,
                    Condition = (PGame Game) => {
                        return Game.NowPlayer.General is P_IzayoiMiku && Game.NowPlayer.Equals(Game.NowPlayer.Position.Lord);
                    },
                    Effect = (PGame Game) => {
                        March.AnnouceUseSkill(Player);
                        PElfPowerTag ElfPowerTag = Player.Tags.FindPeekTag<PElfPowerTag>(PElfPowerTag.TagName);
                        int BonusHouse = 0;
                        if (ElfPowerTag != null) {
                            BonusHouse = ElfPowerTag.Value;
                        }
                        Game.GetHouse(Game.NowPlayer.Position, 1 + BonusHouse);
                        Game.GetCard(Game.NowPlayer, 1);
                    }
                };
            }));
    }

}