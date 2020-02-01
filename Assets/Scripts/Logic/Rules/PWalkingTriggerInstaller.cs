

public class PWalkingTriggerInstaller : PSystemTriggerInstaller {
    public PWalkingTriggerInstaller() : base("行走") {
        TriggerList.Add(new PTrigger("行走") {
            IsLocked = true,
            Time = PPeriod.WalkingStage.During,
            Effect = (PGame Game) => {
                PStepCountTag Tag = Game.TagManager.PopTag<PStepCountTag>(PStepCountTag.TagName);
                int RemainStepCount = (Tag != null ? Tag.StepCount : 0);
                Game.MoveForward(Game.NowPlayer, RemainStepCount);
                PNetworkManager.NetworkServer.TellClients(new PCloseDiceOrder());
            }
        });
    }
}