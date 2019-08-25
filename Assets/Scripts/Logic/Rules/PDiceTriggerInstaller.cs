public class PDiceTriggerInstaller : PSystemTriggerInstaller { 
    public PDiceTriggerInstaller() : base("掷骰子") {
        TriggerList.Add(new PTrigger("掷骰子") {
            IsLocked = true,
            Time = PPeriod.DiceStage.During,
            Effect = (PGame Game) => {
                int DiceResult = PMath.RandInt(1, 6);
                PNetworkManager.NetworkServer.TellClients(new PDiceResultOrder(DiceResult.ToString()));
                Game.TagManager.CreateTag(new PDiceResultTag(DiceResult));
            }
        });
        TriggerList.Add(new PTrigger("点数转为步数") {
            IsLocked = true,
            Time = PPeriod.WalkingStage.Before,
            Effect = (PGame Game) => {
                PDiceResultTag Tag = Game.TagManager.PopTag<PDiceResultTag>(PDiceResultTag.TagName);
                int DiceResult = (Tag != null ? Tag.DiceResult : 0);
                Game.TagManager.CreateTag(new PStepCountTag(DiceResult));
            }
        });
    }
}