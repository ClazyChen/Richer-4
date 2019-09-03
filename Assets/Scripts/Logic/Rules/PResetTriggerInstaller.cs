public class PResetTriggerInstaller : PSystemTriggerInstaller { 
    public PResetTriggerInstaller() : base("重置武将牌") {
        TriggerList.Add(new PTrigger("翻面翻回") {
            IsLocked = true,
            Time = PPeriod.StartTurn.Before,
            Condition = (PGame Game) => {
                return Game.NowPlayer.BackFace;
            },
            Effect = (PGame Game) => {
                Game.ChangeFace(Game.NowPlayer);
                Game.Monitor.EndTurnDirectly = true;
            }
        });
        TriggerList.Add(new PTrigger("移回游戏") {
            IsLocked = true,
            Time = PPeriod.StartTurn.Before,
            Condition = (PGame Game) => {
                return Game.NowPlayer.OutOfGame;
            },
            Effect = (PGame Game) => {
                Game.NowPlayer.Tags.PopTag<PTag>(PTag.OutOfGameTag.Name);
            }
        });
    }
}