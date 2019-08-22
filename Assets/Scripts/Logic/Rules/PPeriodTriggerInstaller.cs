using System;

public class PPeriodTriggerInstaller : PSystemTriggerInstaller {
    private static readonly PPeriod[] TurnFlow = {
        PPeriod.StartTurn,
        PPeriod.PreparationStage,
        PPeriod.JudgeStage,
        PPeriod.FirstFreeTime,
        PPeriod.DiceStage,
        PPeriod.WalkingStage,
        PPeriod.SettleStage,
        PPeriod.SecondFreeTime,
        PPeriod.EndTurnStage,
        PPeriod.EndTurn
    };

    private static readonly Action<PGame> ChangePeriod = (PGame Game) => {
        PThread.Async(() => {
            PPeriod NowPeriod = Game.NowPeriod;
            PPeriod NextPeroid = NowPeriod;
            for (int i = 0; i < TurnFlow.Length; ++i) {
                if (NowPeriod.Equals(TurnFlow[i])) {
                    if (i < TurnFlow.Length - 1) {
                        Game.NowPeriod = NextPeroid = TurnFlow[i + 1];
                    } else {
                        Game.NowPeriod = NextPeroid = TurnFlow[0];
                        Game.NowPlayer = Game.GetNextPlayer(Game.NowPlayer);
                        PNetworkManager.NetworkServer.TellClients(new PStartTurnOrder(Game.NowPlayerIndex.ToString()));
                    }
                    break;
                }
            }
            PNetworkManager.NetworkServer.TellClients(new PStartPeriodOrder(Game.NowPlayerIndex.ToString(), Game.NowPeriod.Name));
            Game.Logic.ShutDown();
            Game.Logic.StartSettle(NextPeroid.Execute());
        });
    };

    public PPeriodTriggerInstaller(): base("阶段切换") {
        foreach (PPeriod Period in TurnFlow) {
            TriggerList.Add(new PTrigger(Period.Name + "结束，进入下一阶段") {
                IsLocked = true,
                Time = Period.Next,
                Effect = ChangePeriod
            });
        }
    }
}