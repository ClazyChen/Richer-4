public class POutOfGameTriggerInstaller : PSystemTriggerInstaller { 
    public POutOfGameTriggerInstaller() : base("防止伤害") {
        TriggerList.Add(new PTrigger("移出游戏[防止伤害]") {
            IsLocked = true,
            Time = PTime.Injure.StartSettle,
            Condition = (PGame Game) => {
                PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                return (InjureTag.FromPlayer != null && InjureTag.FromPlayer.OutOfGame) || (InjureTag.ToPlayer != null && InjureTag.ToPlayer.OutOfGame) && InjureTag.Injure > 0;
            },
            Effect = (PGame Game) => {
                PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                InjureTag.Injure = 0;
                string Name = string.Empty;
                if (InjureTag.FromPlayer != null && InjureTag.FromPlayer.OutOfGame) {
                    Name = InjureTag.FromPlayer.Name;
                } else if (InjureTag.ToPlayer != null && InjureTag.ToPlayer.OutOfGame) {
                    Name = InjureTag.ToPlayer.Name;
                }
                PNetworkManager.NetworkServer.TellClients(new PShowInformationOrder("因" + Name + "移出游戏，伤害防止"));
            }
        });
        TriggerList.Add(new PTrigger("空城[防止伤害]") {
            IsLocked = true,
            Time = PTime.Injure.StartSettle,
            Condition = (PGame Game) => {
                PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                return  (InjureTag.ToPlayer != null && InjureTag.ToPlayer.Tags.ExistTag(PKuungCheevngChiTag.TagName)) && InjureTag.Injure > 0;
            },
            Effect = (PGame Game) => {
                PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                InjureTag.Injure = 0;
                PNetworkManager.NetworkServer.TellClients(new PShowInformationOrder("因" + InjureTag.ToPlayer.Name + "空城，伤害防止"));
            }
        });
    }
}