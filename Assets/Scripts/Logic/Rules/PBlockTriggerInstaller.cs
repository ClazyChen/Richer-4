public class PBlockTriggerInstaller : PSystemTriggerInstaller { 

    public PBlockTriggerInstaller() : base("格子的停留结算") {
        TriggerList.Add(new PTrigger("初始化结算阶段购买土地或房屋的次数") {
            IsLocked = true,
            Time = PPeriod.SettleStage.Before,
            Effect = (PGame Game) => {
                Game.TagManager.CreateTag(new PPurchaseTag(1, 0));
            }
        });
        TriggerList.Add(new PTrigger("清除结算阶段购买土地或房屋的次数") {
            IsLocked = true,
            Time = PPeriod.SettleStage.After,
            Effect = (PGame Game) => {
                Game.TagManager.PopTag<PPurchaseTag>(PPurchaseTag.TagName);
            }
        });
        TriggerList.Add(new PTrigger("奖励点（固定数额）") {
            IsLocked = true,
            Time = PPeriod.SettleStage.During,
            Condition = (PGame Game) => {
                return Game.NowPlayer.Position.GetMoneyStopSolid > 0;
            },
            Effect = (PGame Game) => {
                Game.GetMoney(Game.NowPlayer, Game.NowPlayer.Position.GetMoneyStopSolid);
            }
        });
        TriggerList.Add(new PTrigger("奖励点（百分比）") {
            IsLocked = true,
            Time = PPeriod.SettleStage.During,
            Condition = (PGame Game) => {
                return Game.NowPlayer.Position.GetMoneyStopPercent > 0;
            },
            Effect = (PGame Game) => {
                Game.GetMoney(Game.NowPlayer, PMath.Percent(Game.NowPlayer.Money, Game.NowPlayer.Position.GetMoneyStopPercent));
            }
        });
        TriggerList.Add(new PTrigger("天灾（固定数额）") {
            IsLocked = true,
            Time = PPeriod.SettleStage.During,
            Condition = (PGame Game) => {
                return Game.NowPlayer.Position.GetMoneyStopSolid < 0;
            },
            Effect = (PGame Game) => {
                Game.Injure(null, Game.NowPlayer, -Game.NowPlayer.Position.GetMoneyStopSolid, null);
            }
        });
        TriggerList.Add(new PTrigger("天灾（百分比）") {
            IsLocked = true,
            Time = PPeriod.SettleStage.During,
            Condition = (PGame Game) => {
                return Game.NowPlayer.Position.GetMoneyStopPercent < 0;
            },
            Effect = (PGame Game) => {
                Game.Injure(null, Game.NowPlayer, PMath.Percent(Game.NowPlayer.Money, -Game.NowPlayer.Position.GetMoneyStopPercent), null);
            }
        });
        TriggerList.Add(new PTrigger("牌库") {
            IsLocked = true,
            Time = PPeriod.SettleStage.During,
            Condition = (PGame Game) => {
                return Game.NowPlayer.Position.GetCardStop > 0;
            },
            Effect = (PGame Game) => {
                Game.GetCard(Game.NowPlayer, Game.NowPlayer.Position.GetCardStop);
            }
        });
        MultiPlayerTriggerList.Add((PPlayer Player) => new PTrigger("购买土地") {
            IsLocked = false,
            Player = Player,
            Time = PPeriod.SettleStage.During,
            Condition = (PGame Game) => {
                PBlock NowBlock = Game.NowPlayer.Position;
                PPurchaseTag PurchaseTag = Game.TagManager.FindPeekTag<PPurchaseTag>(PPurchaseTag.TagName);
                return (Game.NowPlayer.Equals(Player) && NowBlock.CanPurchase && NowBlock.Lord == null && PurchaseTag.Count < PurchaseTag.Limit && NowBlock.Price < Game.NowPlayer.Money);
            },
            Effect = (PGame Game) => {
                Game.TagManager.FindPeekTag<PPurchaseTag>(PPurchaseTag.TagName).Count++;
                Game.PurchaseLand(Game.NowPlayer, Game.NowPlayer.Position);
            }
        });
        MultiPlayerTriggerList.Add((PPlayer Player) => new PTrigger("购买房屋") {
            IsLocked = false,
            Player = Player,
            Time = PPeriod.SettleStage.During,
            CanRepeat = true,
            Condition = (PGame Game) => {
                PBlock NowBlock = Game.NowPlayer.Position;
                PPurchaseTag PurchaseTag = Game.TagManager.FindPeekTag<PPurchaseTag>(PPurchaseTag.TagName);
                return (Game.NowPlayer.Equals(Player) && Game.NowPlayer.Equals(NowBlock.Lord) && PurchaseTag.Count < PurchaseTag.Limit && NowBlock.HousePrice < Game.NowPlayer.Money);
            },
            AICondition = (PGame Game) => {
                PBlock NowBlock = Game.NowPlayer.Position;
                PPurchaseTag PurchaseTag = Game.TagManager.FindPeekTag<PPurchaseTag>(PPurchaseTag.TagName);
                /*
                 * AI的买房策略：
                 * 第1次：必买
                 * 第2次：钱多于20000 or 钱多于10000且地价高于2000
                 * 以上：钱多于15000且地价多于3000 or 钱多于10000且为购物中心
                 * 
                 * 必买：公园
                 * 必不买：廉颇 or 无房被兵
                 * 
                 * 赵云：2000次数上限为3,1000无限建
                 */
                if (NowBlock.BusinessType.Equals(PBusinessType.Park)) {
                    return true;
                }
                if (Player.General is P_LianPo) {
                    return false;
                }
                if (Player.Area.AmbushCardArea.CardList.Exists((PCard Card) => Card.Model is P_PingLiangTsuunTuan) && !Player.HasHouse) {
                    return false;
                }
                if (PurchaseTag.Count == 0) {
                    return true;
                }
                if (PurchaseTag.Count == 1) {
                    if (Player.Money >= 20000) {
                        return true;
                    } else if (Player.Money >= 10000 && NowBlock.Price >= 2000) {
                        return true;
                    }
                }
                if (Player.Money >= 15000 && NowBlock.Price >= 3000) {
                    return true;
                }
                if (Player.Money >= 10000 && NowBlock.BusinessType.Equals(PBusinessType.ShoppingCenter)) {
                    return true;
                }
                if (Player.General is P_ZhaoYun) {
                    if (Player.Money >= 5000 && NowBlock.Price < 3000 && PurchaseTag.Count <= 2) {
                        return true;
                    }
                    if (Player.Money >= 2000 && NowBlock.Price < 2000 ) {
                        return true;
                    }
                }
                return false;
            },
            Effect = (PGame Game) => {
                Game.TagManager.FindPeekTag<PPurchaseTag>(PPurchaseTag.TagName).Count++;
                Game.PurchaseHouse(Game.NowPlayer, Game.NowPlayer.Position);
            }
        });
        MultiPlayerTriggerList.Add((PPlayer Player) => new PTrigger("研究所[视察研究成果]") {
            Player = Player,
            Time = PPeriod.SettleStage.During,
            Condition = (PGame Game) => {
                PBlock NowBlock = Game.NowPlayer.Position;
                return Player.Equals(NowBlock.Lord) && NowBlock.BusinessType.Equals(PBusinessType.Institute);
            },
            AICondition = (PGame Game) => {
                return Player.TeamIndex == Game.NowPlayer.TeamIndex;
            },
            Effect = (PGame Game) => {
                int Number = PMath.RandInt(2, 7) / 2;
                Game.GetCard(Game.NowPlayer, Number);
            }
        });
        MultiPlayerTriggerList.Add((PPlayer Player) => new PTrigger("当铺[典当手牌]") {
            Player = Player,
            Time = PPeriod.SettleStage.During,
            Condition = (PGame Game) => {
                PBlock NowBlock = Game.NowPlayer.Position;
                return Game.NowPlayer.Equals(Player) && NowBlock.Lord != null && Player.Area.HandCardArea.CardNumber > 0 && NowBlock.BusinessType.Equals(PBusinessType.Pawnshop);
            },
            AICondition = (PGame Game) => {
                return Game.NowPlayer.TeamIndex == Game.NowPlayer.Position.Lord.TeamIndex || PAiCardExpectation.FindLeastValuable(Game, Player, Player, true, false, false, true).Value < 750;
            },
            Effect = (PGame Game) => {
                Game.GiveCardTo(Game.NowPlayer, Game.NowPlayer.Position.Lord, true, false);
                Game.GetMoney(Game.NowPlayer, 2000);
            }
        });
        TriggerList.Add(new PTrigger("公园[扩建政府补助]") {
            IsLocked = true,
            Time = PTime.PurchaseHouseTime,
            Condition = (PGame Game) => {
                return Game.TagManager.FindPeekTag<PPurchaseHouseTag>(PPurchaseHouseTag.TagName).Block.BusinessType.Equals(PBusinessType.Park);
            },
            Effect = (PGame Game) => {
                PPurchaseHouseTag PurchaseHouseTag = Game.TagManager.FindPeekTag<PPurchaseHouseTag>(PPurchaseHouseTag.TagName);
                Game.GetMoney(PurchaseHouseTag.Player, PMath.Percent(PurchaseHouseTag.Block.Price, 10));
            }
        });
        MultiPlayerTriggerList.Add((PPlayer Player) => new PTrigger("收取过路费") {
            Player = Player,
            Time = PPeriod.SettleStage.During,
            Condition = (PGame Game) => {
                return !Game.NowPlayer.Equals(Player) && Player.Equals(Game.NowPlayer.Position.Lord);
            },
            AICondition = (PGame Game) => {
                if ((Game.NowPlayer.General is P_PanYue || Game.NowPlayer.General is P_ChenSheng) && Game.NowPlayer.TeamIndex == Player.TeamIndex) {
                    // 给闲居和起义让路
                    return false;
                }
                return PAiTargetChooser.InjureExpect(Game, Player, Player, Game.NowPlayer, Game.NowPlayer.Position.Toll, Game.NowPlayer.Position) > 0;
            },
            Effect = (PGame Game) => {
                Game.Toll(Player, Game.NowPlayer, Game.NowPlayer.Position);
            }
        });
    }
}