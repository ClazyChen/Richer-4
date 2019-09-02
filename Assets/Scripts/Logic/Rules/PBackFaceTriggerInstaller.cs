public class PBackFaceTriggerInstaller : PSystemTriggerInstaller { 
    public PBackFaceTriggerInstaller() : base("翻面翻回") {
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
    }
}