

public class PWalkingTriggerInstaller : PSystemTriggerInstaller {
    public PWalkingTriggerInstaller() : base("行走") {
        TriggerList.Add(new PTrigger("行走") {
            IsLocked = true,
            Time = PPeriod.WalkingStage.During,
            Effect = (PGame Game) => {
                PStepCountTag Tag = Game.TagManager.PopTag<PStepCountTag>(PStepCountTag.TagName);
                int RemainStepCount = (Tag != null ? Tag.StepCount : 0);
                while (RemainStepCount-- > 0) {
                    Game.Monitor.CallTime(PTime.MovePositionTime, new PTransportTag(Game.NowPlayer, Game.NowPlayer.Position, Game.NowPlayer.Position.NextBlock));
                    if (RemainStepCount > 0) {
                        Game.Monitor.CallTime(PTime.PassBlockTime, new PPassBlockTag(Game.NowPlayer, Game.NowPlayer.Position));
                    }
                }
                PNetworkManager.NetworkServer.TellClients(new PCloseDiceOrder());
            }
        });
    }
}