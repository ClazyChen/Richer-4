using System;
using System.Collections.Generic;

public class P_WuZhao: PGeneral {

    public static PTag NvQuanTag = new PTag("女权标记");

    public P_WuZhao() : base("武瞾") {
        Sex = PSex.Female;
        Age = PAge.Renaissance;
        Index = 15;
        Cost = 30;
        Tips = "定位：攻击\n" +
            "难度：困难\n" +
            "史实：武周皇帝，中国历史上唯一统一王朝女皇帝。在位期间以酷吏政治闻名，晚年骄奢淫逸，致使唐朝中衰。\n" +
            "攻略：\n武瞾有且仅有两个限定技，对两个限定技的使用能力决定了她的存在感和强度。武瞾玩家对于局势的观察能力非常重要，因为发动时机的选择直接影响到武瞾的强度。\n如果选择的发动时机得当，武瞾的强度是很高的。【女权】在储存了大量伤害类计策的情况下，可以带来惊人的爆发伤害，因此在【女权】发动之前，【瞒天过海】、【关门捉贼】等牌并不建议使用。使用【以逸待劳】加【浑水摸鱼】的组合精准打击敌人也是优秀的策略。\n【迁都】的发动时机更依靠于玩家把握力，原则上越晚【迁都】可以赠送的房屋越多，但如果【迁都】过晚，则可能建造的城堡不够发挥输出。【迁都】和【上屋抽梯】也是很好的组合。";

        PSkill NvQuan = new PSkill("女权") {
            Initiative = true
        };
        SkillList.Add(NvQuan
            .AnnouceGameOnce()
            .AddTimeTrigger(
            new PTime[] {
                PPeriod.FirstFreeTime.During,
                PPeriod.SecondFreeTime.During
            },
            (PTime Time, PPlayer Player, PSkill Skill) => {
                return new PTrigger(NvQuan.Name) {
                    IsLocked = false,
                    Player = Player,
                    Time = Time,
                    AIPriority = 250,
                    CanRepeat = true,
                    Condition = (PGame Game) => {
                        return Player.Equals(Game.NowPlayer) && (Player.IsAI || Game.Logic.WaitingForEndFreeTime()) && Player.RemainLimit(NvQuan.Name);
                    },
                    AICondition = (PGame Game) => {
                        /*
                         * 女权的发动条件：
                         * 【只会在第一个空闲时间点发动】
                         * 观察场上所有女性直到你的下回合开始的过程
                         * 即，对场上所有人的下一回合行动，如果领主为女性，记录相应的伤害收益
                         * 然后，对
                         * 注：在没发动女权的情况下，武瞾将不会使用以下牌：
                         * 瞒天过海、关门捉贼、笑里藏刀、反间计、打草惊蛇、抛砖引玉、以逸待劳、浑水摸鱼
                         * 
                         * 当有女权状态且有浑水摸鱼的情况下，女性角色的以逸待劳将指定所有敌方角色为目标
                         */
                        if (!Game.NowPeriod.Equals(PPeriod.FirstFreeTime)) {
                            return false;
                        }
                        int ExpectSum = 0;
                        Game.AlivePlayers().FindAll((PPlayer _Player) => !_Player.Equals(Player)).ForEach((PPlayer _Player) => {
                            List<PBlock> NextBlocks = PAiMapAnalyzer.NextBlocks(Game, _Player);
                            int TempSum = 0;
                            NextBlocks.ForEach((PBlock Block) => {
                                if (Block.Lord != null && Block.Lord.TeamIndex != _Player.TeamIndex && Block.Lord.Sex.Equals(PSex.Female)) {
                                    TempSum += 4000 * (Block.Lord.TeamIndex == Player.TeamIndex ? 1 : -1);
                                }
                            });
                            TempSum /= Math.Max(1, NextBlocks.Count);
                            ExpectSum += TempSum;
                        });
                        ExpectSum += 4000 * Player.Area.HandCardArea.CardList.FindAll((PCard Card) => Card.Model is P_ManTiienKuoHai).Count;
                        ExpectSum += Game.Enemies(Player).FindAll((PPlayer _Player) => _Player.Position.HouseNumber == 0).Count * 4000 *
                                     Player.Area.HandCardArea.CardList.FindAll((PCard Card) => Card.Model is P_KuanMevnChoTsev).Count;
                        ExpectSum += 4000 * Math.Min(Player.Area.HandCardArea.CardList.FindAll((PCard Card) => Card.Type.IsEquipment()).Count,
                                     Player.Area.HandCardArea.CardList.FindAll((PCard Card) => Card.Model is P_HsiaoLiTsaangTao).Count);
                        if (Player.Area.HandCardArea.CardList.Exists((PCard Card) => {
                            return Card.Model is P_FanChienChi || Card.Model is P_TaTsaaoChingShev || Card.Model is P_PaaoChuanYinYoo ||
                                   Card.Model is P_IITaiLao;
                        })) {
                            ExpectSum += 4000 * Game.Enemies(Player).Count *
                                         Player.Area.HandCardArea.CardList.FindAll((PCard Card) => Card.Model is P_HunShuiMoYoo).Count;
                        }
                        return ExpectSum > 12000 || (ExpectSum > 6000 && Player.Money <= 2000);
                    },
                    Effect = (PGame Game) => {
                        NvQuan.AnnouceUseSkill(Player);
                        Game.AlivePlayers().ForEach((PPlayer _Player) => {
                            if (_Player.Sex.Equals(PSex.Female)) {
                                _Player.Tags.CreateTag(NvQuanTag);
                            }
                        });
                        Player.Tags.FindPeekTag<PUsedTag>(PUsedTag.TagNamePrefix + NvQuan.Name).Count++;
                    }
                };
            })
            .AddTrigger((PPlayer Player, PSkill Skill) => {
                return new PTrigger(NvQuan.Name + "[增加伤害]") {
                    IsLocked = true,
                    Player = null,
                    Time = PTime.Injure.EmitInjure,
                    Condition = (PGame Game) => {
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        return (InjureTag.FromPlayer != null && InjureTag.FromPlayer.Tags.ExistTag(NvQuanTag.Name)) ;
                    },
                    Effect = (PGame Game) => {
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        NvQuan.AnnouceUseSkill(InjureTag.FromPlayer);
                        InjureTag.Injure += 2000;
                    }
                };
            }).AddTrigger((PPlayer Player, PSkill Skill) => {
                return new PTrigger(NvQuan.Name + "[效果结束]") {
                    IsLocked = true,
                    Player = null,
                    Time = PPeriod.StartTurn.Start,
                    Condition = (PGame Game) => {
                        return Game.NowPlayer.Equals(Player);
                    },
                    Effect = (PGame Game) => {
                        Game.AlivePlayers().ForEach((PPlayer _Player) => {
                            _Player.Tags.PopTag<PTag>(NvQuanTag.Name);
                        });
                    }
                };
            }));


        KeyValuePair<PBlock, int> QianDuTarget(PGame Game, PPlayer Player) {
            int BusinessLandCount = Game.Map.BlockList.FindAll((PBlock Block) => Player.Equals(Block.Lord) && Block.IsBusinessLand).Count;
            return PMath.Max(Game.Map.BlockList.FindAll((PBlock Block) => Player.Equals(Block.Lord) && !Block.IsBusinessLand), (PBlock Block) => {
                int HouseNumber = Game.GetBonusHouseNumberOfCastle(Player, Block);
                HouseNumber += BusinessLandCount;
                return HouseNumber * PAiMapAnalyzer.HouseValue(Game, Player, Block);
            });
        }
        PSkill QianDu = new PSkill("迁都") {
            Initiative = true
        };
        SkillList.Add(QianDu
            .AnnouceGameOnce()
            .AddTimeTrigger(
            new PTime[] {
                PPeriod.FirstFreeTime.During,
                PPeriod.SecondFreeTime.During
            },
            (PTime Time, PPlayer Player, PSkill Skill) => {
                return new PTrigger(QianDu.Name) {
                    IsLocked = false,
                    Player = Player,
                    Time = Time,
                    AIPriority = 280,
                    CanRepeat = true,
                    Condition = (PGame Game) => {
                        return Player.Equals(Game.NowPlayer) && (Player.IsAI || Game.Logic.WaitingForEndFreeTime()) && Player.RemainLimit(QianDu.Name) && 
                        Game.Map.BlockList.Exists((PBlock Block) => Player.Equals(Block.Lord) && !Block.IsBusinessLand);
                    },
                    AICondition = (PGame Game) => {
                        if (!Game.NowPeriod.Equals(PPeriod.FirstFreeTime)) {
                            return false;
                        }
                        return QianDuTarget(Game, Player).Value > 12000;
                    },
                    Effect = (PGame Game) => {
                        QianDu.AnnouceUseSkill(Player);
                        PBlock TargetBlock = null;
                        if (Player.IsAI) {
                            TargetBlock = QianDuTarget(Game, Player).Key;
                        } else {
                            TargetBlock = PNetworkManager.NetworkServer.ChooseManager.AskToChooseBlock(Player, QianDu.Name, (PBlock Block) => {
                                return Player.Equals(Block.Lord) && !Block.IsBusinessLand;
                            });
                        }
                        if (TargetBlock != null) {
                            int BusinessLandCount = Game.Map.BlockList.FindAll((PBlock Block) => Player.Equals(Block.Lord) && Block.IsBusinessLand).Count;
                            int HouseNumber = Game.GetBonusHouseNumberOfCastle(Player, TargetBlock) + BusinessLandCount;
                            Game.MovePosition(Player, Player.Position, TargetBlock);
                            PNetworkManager.NetworkServer.TellClients(new PHighlightBlockOrder(TargetBlock.Index.ToString()));
                            TargetBlock.BusinessType = PBusinessType.Castle;
                            Game.GetHouse(TargetBlock, HouseNumber);
                        }
                        Player.Tags.FindPeekTag<PUsedTag>(PUsedTag.TagNamePrefix + QianDu.Name).Count++;
                    }
                };
            }));
    }

}