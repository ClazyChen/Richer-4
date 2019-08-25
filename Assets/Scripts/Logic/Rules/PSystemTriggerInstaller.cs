using System.Collections.Generic;
using System;

public abstract class PSystemTriggerInstaller : PObject{
    protected readonly List<PTrigger> TriggerList;
    protected readonly List<Converter<PPlayer, PTrigger>> MultiPlayerTriggerList;
    protected PSystemTriggerInstaller(string _Name) {
        Name = _Name;
        TriggerList = new List<PTrigger>();
        MultiPlayerTriggerList = new List<Converter<PPlayer, PTrigger>>();
    }
    public void Install(PMonitor Monitor) {
        TriggerList.ForEach((PTrigger Trigger) => {
            Monitor.AddTrigger(Trigger);
        });
        Monitor.Game.PlayerList.ForEach((PPlayer Player) => {
            PPlayer TargetPlayer = Player;
            MultiPlayerTriggerList.ForEach((Converter<PPlayer, PTrigger> TriggerTemplate) => {
                Monitor.AddTrigger(TriggerTemplate(TargetPlayer));
            });
        });
        PLogger.Log("  规则装载完毕：" + Name);
    }
}