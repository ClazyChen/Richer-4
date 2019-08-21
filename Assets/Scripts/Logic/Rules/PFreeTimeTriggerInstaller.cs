

public class PFreeTimeTriggerInstaller : PSystemTriggerInstaller {
    public PFreeTimeTriggerInstaller() : base("玩家空闲时间点") {
        foreach (PTime Time in new PTime[] { PPeriod.FirstFreeTime.During, PPeriod.SecondFreeTime.During}) {
            TriggerList.Add(new PTrigger("玩家的空闲时间点") {
                IsLocked = true,
                Time = Time,
                Condition = (PGame Game) => Game.NowPlayer.IsUser,
                Effect = (PGame Game) => {
                    Game.TagManager.CreateTag(PTag.FreeTimeOperationTag);
                    PThread.WaitUntil(() => !Game.TagManager.ExistTag(PTag.FreeTimeOperationTag.Name));
                }
            });
        }
    }
}