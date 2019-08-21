using System.Collections.Generic;
using System.Linq;

public class PTriggerManager {
    private readonly PGame Game;
    private readonly List<PTrigger> TriggerList;

    public PTriggerManager(PGame _Game) {
        Game = _Game;
        TriggerList = new List<PTrigger>();
    }

    public void AddTrigger(PTrigger Trigger) {
        TriggerList.Add(Trigger);
    }

    public bool RemoveTrigger(PTrigger Trigger) {
        return TriggerList.Remove(Trigger);
    }

    public PTag CallTime(PTime Time, PTag OriginalTag) {
        Game.TagManager.CreateTag(OriginalTag);
        CallTime(Time);
        return Game.TagManager.PopTag<PTag>(OriginalTag.Name);
    }

    // 宣布一个时机的到来
    public void CallTime(PTime Time) {
        PLogger.Log("时机到来：" + Time.Name);
        List<PTrigger> AvailableTriggerList = TriggerList.FindAll((PTrigger Trigger) => Trigger.Time.Equals(Time));
        AvailableTriggerList.Sort((PTrigger x, PTrigger y) => PTrigger.ComparePriority(Game, x, y));
        for (int i = 0; i < AvailableTriggerList.Count;) {
            int Count = 0;
            for (++Count; i + Count < AvailableTriggerList.Count; ++Count) {
                if (AvailableTriggerList[i].Player != AvailableTriggerList[i+Count].Player || (AvailableTriggerList[i].Player == null && AvailableTriggerList[i].AIPriority != AvailableTriggerList[i+Count].AIPriority)) { // 优先级不同
                    break;
                }
            }
            List<PTrigger> CurrentTriggerList = AvailableTriggerList.GetRange(i, Count);
            i += Count;
            PPlayer Judger = CurrentTriggerList[0].Player;
            if (Judger != null && !Judger.IsAlive) {
                continue;
            }
            List<PTrigger> ValidTriggerList;
            while ((ValidTriggerList = CurrentTriggerList.FindAll((PTrigger Trigger) => Trigger.Condition(Game) && (Judger == null || Judger.IsUser || Trigger.AICondition(Game)))).Count > 0) {
                PTrigger ChosenTrigger = null;
                #region 系统和AI直接选择，玩家利用选择管理器选择
                if (Judger == null || Judger.IsAI) {
                    ChosenTrigger = ValidTriggerList[0];
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
                #endregion
                if (ChosenTrigger != null) {
                    Game.Logic.StartSettle(new PSettle("于[" + Time.Name + "]触发[" + ChosenTrigger.Name + "]", (PGame Game) => {
                        ChosenTrigger.Effect(Game);
                    }));
                    CurrentTriggerList.Remove(ChosenTrigger);
                } else {
                    break;
                }
            }
        }
        PLogger.Log("时机结束：" + Time.Name);
    }

}