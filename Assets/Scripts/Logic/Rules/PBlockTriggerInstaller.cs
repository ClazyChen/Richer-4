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
            IsLocked = true,
            Player = Player,
            Time = PPeriod.SettleStage.During,
            Condition = (PGame Game) => {
                PBlock NowBlock = Game.NowPlayer.Position;
                PPurchaseTag PurchaseTag = Game.TagManager.FindPeekTag<PPurchaseTag>(PPurchaseTag.TagName);
                return (Game.NowPlayer.Equals(Player) && NowBlock.CanPurchase && NowBlock.Lord == null && PurchaseTag.Count < PurchaseTag.Limit && NowBlock.Price < Game.NowPlayer.Money);
            },
            Effect = (PGame Game) => {
                bool Purchase = Game.NowPlayer.IsAI || PNetworkManager.NetworkServer.ChooseManager.AskYesOrNo(Game.NowPlayer, "是否购买土地？");
                if (Purchase) {
                    Game.TagManager.FindPeekTag<PPurchaseTag>(PPurchaseTag.TagName).Count++;
                    Game.PurchaseLand(Game.NowPlayer, Game.NowPlayer.Position);
                }
            }
        });
        MultiPlayerTriggerList.Add((PPlayer Player) => new PTrigger("购买房屋") {
            IsLocked = true,
            Player = Player,
            Time = PPeriod.SettleStage.During,
            Condition = (PGame Game) => {
                PBlock NowBlock = Game.NowPlayer.Position;
                PPurchaseTag PurchaseTag = Game.TagManager.FindPeekTag<PPurchaseTag>(PPurchaseTag.TagName);
                return (Game.NowPlayer.Equals(Player) && Game.NowPlayer.Equals(NowBlock.Lord) && PurchaseTag.Count < PurchaseTag.Limit && NowBlock.HousePrice < Game.NowPlayer.Money);
            },
            Effect = (PGame Game) => {
                bool Purchase = Game.NowPlayer.IsAI || PNetworkManager.NetworkServer.ChooseManager.AskYesOrNo(Game.NowPlayer, "是否购买房屋？");
                if (Purchase) {
                    Game.TagManager.FindPeekTag<PPurchaseTag>(PPurchaseTag.TagName).Count++;
                    Game.PurchaseHouse(Game.NowPlayer, Game.NowPlayer.Position);
                }
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
                return Game.NowPlayer.TeamIndex == Game.NowPlayer.Position.Lord.TeamIndex || PAiCardExpectation.FindLeastValuable(Game, Player, Player, false, false, true).Value < 750;
            },
            Effect = (PGame Game) => {
                Game.GiveCardTo(Game.NowPlayer, Game.NowPlayer.Position.Lord, false);
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
                return Game.NowPlayer.TeamIndex != Player.TeamIndex;
            },
            Effect = (PGame Game) => {
                Game.Toll(Player, Game.NowPlayer, Game.NowPlayer.Position);
            }
        });
    }
}