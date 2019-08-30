public class PBlockPassTriggerInstaller : PSystemTriggerInstaller { 
    public PBlockPassTriggerInstaller() : base("格子的经过结算") {
        TriggerList.Add(new PTrigger("经过奖励点（固定数额）") {
            IsLocked = true,
            Time = PTime.PassBlockTime,
            Condition = (PGame Game) => {
                return Game.TagManager.FindPeekTag<PPassBlockTag>(PPassBlockTag.TagName).Block.GetMoneyPassSolid > 0;
            },
            Effect = (PGame Game) => {
                Game.GetMoney(Game.TagManager.FindPeekTag<PPassBlockTag>(PPassBlockTag.TagName).Player, Game.TagManager.FindPeekTag<PPassBlockTag>(PPassBlockTag.TagName).Block.GetMoneyPassSolid);
            }
        });
        TriggerList.Add(new PTrigger("经过奖励点（百分比）") {
            IsLocked = true,
            Time = PTime.PassBlockTime,
            Condition = (PGame Game) => {
                return Game.TagManager.FindPeekTag<PPassBlockTag>(PPassBlockTag.TagName).Block.GetMoneyPassPercent > 0;
            },
            Effect = (PGame Game) => {
                Game.GetMoney(Game.TagManager.FindPeekTag<PPassBlockTag>(PPassBlockTag.TagName).Player, PMath.Percent(Game.TagManager.FindPeekTag<PPassBlockTag>(PPassBlockTag.TagName).Player.Money, Game.TagManager.FindPeekTag<PPassBlockTag>(PPassBlockTag.TagName).Block.GetMoneyPassPercent));
            }
        });
        TriggerList.Add(new PTrigger("经过天灾（固定数额）") {
            IsLocked = true,
            Time = PTime.PassBlockTime,
            Condition = (PGame Game) => {
                return Game.TagManager.FindPeekTag<PPassBlockTag>(PPassBlockTag.TagName).Block.GetMoneyPassSolid < 0;
            },
            Effect = (PGame Game) => {
                Game.Injure(null, Game.TagManager.FindPeekTag<PPassBlockTag>(PPassBlockTag.TagName).Player, -Game.TagManager.FindPeekTag<PPassBlockTag>(PPassBlockTag.TagName).Block.GetMoneyPassSolid);
            }
        });
        TriggerList.Add(new PTrigger("经过天灾（百分比）") {
            IsLocked = true,
            Time = PTime.PassBlockTime,
            Condition = (PGame Game) => {
                return Game.TagManager.FindPeekTag<PPassBlockTag>(PPassBlockTag.TagName).Block.GetMoneyPassPercent < 0;
            },
            Effect = (PGame Game) => {
                Game.Injure(null, Game.TagManager.FindPeekTag<PPassBlockTag>(PPassBlockTag.TagName).Player, PMath.Percent(Game.TagManager.FindPeekTag<PPassBlockTag>(PPassBlockTag.TagName).Player.Money, -Game.TagManager.FindPeekTag<PPassBlockTag>(PPassBlockTag.TagName).Block.GetMoneyPassPercent));
            }
        });
        TriggerList.Add(new PTrigger("牌库") {
            IsLocked = true,
            Time = PTime.PassBlockTime,
            Condition = (PGame Game) => {
                return Game.TagManager.FindPeekTag<PPassBlockTag>(PPassBlockTag.TagName).Block.GetCardPass > 0;
            },
            Effect = (PGame Game) => {
                Game.GetCard(Game.TagManager.FindPeekTag<PPassBlockTag>(PPassBlockTag.TagName).Player, Game.TagManager.FindPeekTag<PPassBlockTag>(PPassBlockTag.TagName).Block.GetCardPass);
            }
        });
    }
}