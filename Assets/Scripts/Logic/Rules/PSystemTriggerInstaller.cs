using System.Collections.Generic;

public abstract class PSystemTriggerInstaller {
    protected readonly List<PTrigger> TriggerList;
    protected PSystemTriggerInstaller() {
        TriggerList = new List<PTrigger>();
    }
    public void Install(PTriggerManager Monitor) {
        TriggerList.ForEach((PTrigger Trigger) => {
            Monitor.AddTrigger(Trigger);
        });
    }
}