public class PBlockTriggerInstaller : PSystemTriggerInstaller { 
    public PBlockTriggerInstaller() : base("格子的停留结算") {
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
    }
}