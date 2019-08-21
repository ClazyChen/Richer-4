using System.Collections.Generic;

public abstract class PSystemTriggerInstaller : PObject{
    protected readonly List<PTrigger> TriggerList;
    protected PSystemTriggerInstaller(string _Name) {
        Name = _Name;
        TriggerList = new List<PTrigger>();
    }
    public void Install(PTriggerManager Monitor) {
        TriggerList.ForEach((PTrigger Trigger) => {
            Monitor.AddTrigger(Trigger);
        });
        PLogger.Log("  规则装载完毕：" + Name);
    }
}