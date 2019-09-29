using System;

public class PPeriodTriggerInstaller : PSystemTriggerInstaller {
    private class PInPortalTag : PTag {
        public static string TagName = "位于传送门内";
        public PInPortalTag() : base(TagName) {

        }
    }

    public static readonly PPeriod[] TurnFlow = {
        PPeriod.StartTurn,
        PPeriod.PreparationStage,
        PPeriod.AmbushStage,
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
                    #region 传送门结算
                    if (NowPeriod.Equals(PPeriod.SettleStage)) {
                        if (Game.TagManager.ExistTag(PInPortalTag.TagName)) {
                            Game.TagManager.PopTag<PInPortalTag>(PInPortalTag.TagName);
                        } else if (Game.NowPlayer.Position.PortalBlockList.Count > 0 && Game.NowPlayer.IsAlive && !Game.Monitor.EndTurnDirectly) {
                            Game.TagManager.CreateTag(new PInPortalTag());
                            PBlock ChosenTarget = Game.NowPlayer.Position.PortalBlockList[0];
                            if (Game.NowPlayer.Position.PortalBlockList.Count > 1) {
                                if (Game.NowPlayer.IsUser) {
                                    ChosenTarget = Game.NowPlayer.Position.PortalBlockList[PNetworkManager.NetworkServer.ChooseManager.Ask(Game.NowPlayer, "选择传送门目标", Game.NowPlayer.Position.PortalBlockList.ConvertAll((PBlock Block) => Block.Name).ToArray())];
                                } else {
                                    ChosenTarget = PMath.Max(Game.NowPlayer.Position.PortalBlockList, (PBlock Block) => {
                                        return PAiMapAnalyzer.StartFromExpect(Game, Game.NowPlayer, Block);
                                    }).Key;
                                }
                            }
                            if (ChosenTarget != null) {
                                Game.MovePosition(Game.NowPlayer, Game.NowPlayer.Position, ChosenTarget);
                                break;
                            }
                        }
                    }
                    #endregion
                    if (i < TurnFlow.Length - 1) {
                        Game.NowPeriod = NextPeroid = TurnFlow[i + 1];
                    } else {
                        Game.NowPeriod = NextPeroid = TurnFlow[0];
                        Game.NowPlayer = Game.GetNextPlayer(Game.NowPlayer);
                        Game.Monitor.EndTurnDirectly = false;
                        PNetworkManager.NetworkServer.TellClients(new PStartTurnOrder(Game.NowPlayerIndex.ToString()));
                    }
                    break;
                }
            }

            // 防卡死机制
            PThread.WaitUntil(() => {
                return PUIManager.IsAvailable(PAnimation.IDCount);
            });

            PNetworkManager.NetworkServer.TellClients(new PStartPeriodOrder(Game.NowPlayerIndex.ToString(), Game.NowPeriod.Name));
            PLogger.Log(Game.NowPlayer.Name + "【手牌】" + string.Join(";",Game.NowPlayer.Area.HandCardArea.ToStringArray()));
            Game.Logic.ShutDown();
            Game.Logic.StartLogic(NextPeroid.Execute());
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