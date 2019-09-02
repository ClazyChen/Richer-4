public class POutOfGameTriggerInstaller : PSystemTriggerInstaller { 
    public POutOfGameTriggerInstaller() : base("移出游戏[防止伤害]") {
        TriggerList.Add(new PTrigger("掷骰子") {
            IsLocked = true,
            Time = PTime.Injure.StartSettle,
            Condition = (PGame Game) => {
                PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                return (InjureTag.FromPlayer != null && InjureTag.FromPlayer.Tags.ExistTag(PTag.OutOfGameTag.Name)) || (InjureTag.ToPlayer != null && InjureTag.ToPlayer.Tags.ExistTag(PTag.OutOfGameTag.Name)) && InjureTag.Injure > 0;
            },
            Effect = (PGame Game) => {
                PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                InjureTag.Injure = 0;
                string Name = string.Empty;
                if (InjureTag.FromPlayer != null && InjureTag.FromPlayer.Tags.ExistTag(PTag.OutOfGameTag.Name)) {
                    Name = InjureTag.FromPlayer.Name;
                } else if (InjureTag.ToPlayer != null && InjureTag.ToPlayer.Tags.ExistTag(PTag.OutOfGameTag.Name)) {
                    Name = InjureTag.ToPlayer.Name;
                }
                PNetworkManager.NetworkServer.TellClients(new PShowInformationOrder("因" + Name + "移出游戏，伤害防止"));
            }
        });
    }
}