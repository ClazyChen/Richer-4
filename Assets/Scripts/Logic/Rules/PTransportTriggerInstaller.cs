

public class PTransportTriggerInstaller : PSystemTriggerInstaller {
    public PTransportTriggerInstaller() : base("移动位置") {
        TriggerList.Add(new PTrigger("移动位置") {
            IsLocked = true,
            Time = PTime.MovePositionTime,
            Effect = (PGame Game) => {
                PTransportTag Tag = Game.TagManager.PopTag<PTransportTag>(PTransportTag.TagName);
                if (Tag.Player != null && Tag.Destination != null) {
                    Tag.Player.Position = Tag.Destination;
                    PNetworkManager.NetworkServer.TellClients(new PMovePositionOrder(Tag.Player.Index.ToString(), Tag.Destination.Index.ToString()));
                }
            }
        });
    }
}