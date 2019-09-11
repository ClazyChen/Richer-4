using System.Collections.Generic;
using System.Linq;

public class PMonitor {
    public readonly PGame Game;
    public bool EndTurnDirectly;
    private readonly List<PTrigger> TriggerList;

    public PMonitor(PGame _Game) {
        Game = _Game;
        EndTurnDirectly = false;
        TriggerList = new List<PTrigger>();
    }

    public void AddTrigger(PTrigger Trigger) {
        TriggerList.Add(Trigger);
    }

    public void RemoveAll() {
        EndTurnDirectly = false;
        TriggerList.Clear();
    }

    public bool RemoveTrigger(PTrigger Trigger) {
        return TriggerList.Remove(Trigger);
    }

    public T CallTime<T>(PTime Time, T OriginalTag) where T : PTag{
        Game.TagManager.CreateTag(OriginalTag);
        CallTime(Time);
        return Game.TagManager.PopTag<T>(OriginalTag.Name);
    }

    private List<PPlayer> SettleSequence() {
        List<PPlayer> Sequence = new List<PPlayer>() { null };
        for (int i = Game.NowPlayerIndex; ; ++ i) {
            PPlayer Player = Game.PlayerList[i % Game.PlayerNumber];
            if (Player.IsAlive) {
                if (Sequence.Contains(Player)) {
                    break;
                } else {
                    Sequence.Add(Player);
                }
            }
        }
        return Sequence;
    }

    // 宣布一个时机的到来
    public void CallTime(PTime Time) {
        PLogger.Log("时机到来：" + Time.Name);
        if (Time.IsPeroidTime() && EndTurnDirectly) {
            PLogger.Log("阶段立刻结束");
            return;
        }
        List<PPlayer> Sequence = SettleSequence();
        List<PTrigger> ValidTriggerList;
        List<PTrigger> AlreadyTriggerList = new List<PTrigger>();
        foreach (PPlayer Judger in Sequence) {
            AlreadyTriggerList.Clear();
            while ((ValidTriggerList = TriggerList.FindAll((PTrigger Trigger) => {
                return Trigger.Time.Equals(Time) && Trigger.Player == Judger && !AlreadyTriggerList.Contains(Trigger) && Trigger.Condition(Game) && (Judger == null || Judger.IsAlive) && (Judger == null || Judger.IsUser || Trigger.AICondition(Game));
            })).Count > 0) {
                PTrigger ChosenTrigger = null;
                if (Judger == null || Judger.IsAI) {
                    ChosenTrigger = PMath.Max(ValidTriggerList, (PTrigger Trigger) => Trigger.AIPriority).Key;
                } else {
                    if (ValidTriggerList.Count == 1 && ValidTriggerList[0].IsLocked) {
                        ChosenTrigger = ValidTriggerList[0];
                    } else {
                        string[] TriggerNames = ValidTriggerList.ConvertAll((PTrigger Trigger) => Trigger.Name + (Trigger.IsLocked ? "(锁定)" : string.Empty)).ToArray();
                        if (ValidTriggerList.TrueForAll((PTrigger Trigger) => !Trigger.IsLocked)) {
                            TriggerNames = TriggerNames.Concat(new string[] { "取消" }).ToArray();
                        }
                        int ChosenResult = PNetworkManager.NetworkServer.ChooseManager.Ask(Judger, "选择以下效果发动", TriggerNames);
                        if (ChosenResult < ValidTriggerList.Count) {
                            ChosenTrigger = ValidTriggerList[ChosenResult];
                        }
                    }
                }
                if (ChosenTrigger != null) {
                    Game.Logic.StartSettle(new PSettle("于[" + Time.Name + "]触发[" + ChosenTrigger.Name + "]", (PGame Game) => {
                        ChosenTrigger.Effect(Game);
                    }));
                    if (!ChosenTrigger.CanRepeat) {
                        AlreadyTriggerList.Add(ChosenTrigger);
                    }
                    if (Time.IsPeroidTime() && EndTurnDirectly) {
                        PLogger.Log("阶段立刻结束");
                        return;
                    }
                    if (Time.Equals(PTime.EnterDyingTime)) {
                        PDyingTag DyingTag = Game.TagManager.FindPeekTag<PDyingTag>(PDyingTag.TagName);
                        if (DyingTag.Player != null && DyingTag.Player.Money > 0) {
                            PLogger.Log("脱离濒死状态");
                            return;
                        }
                    }
                } else {
                    break;
                }
            }
        }
        PLogger.Log("时机结束：" + Time.Name);
    }

}