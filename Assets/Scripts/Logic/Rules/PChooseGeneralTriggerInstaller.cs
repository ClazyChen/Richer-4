using System;
using System.Collections.Generic;

public class PChooseGeneralTriggerInstaller : PSystemTriggerInstaller { 
    public PChooseGeneralTriggerInstaller() : base("选将") {
        TriggerList.Add(new PTrigger("选将") {
            IsLocked = true,
            Time = PTime.ChooseGeneralTime,
            Effect = (PGame Game) => {
                List<PGeneral> Generals = new List<PGeneral>();

                #region 选将卡的结算
                List<PGeneral> AvailableGenerals = new List<PGeneral>();
                AvailableGenerals = ListSubTypeInstances<PGeneral>().FindAll((PGeneral General) => !(General is P_Soldier));
                List<PAge> Ages = new List<PAge>() {
                    PAge.Classic, PAge.Medieval, PAge.Renaissance, PAge.Industrial
                };
                for (int i = 0;i < 8; ++ i) {
                    Generals.Add(new P_Soldier());
                }
                Game.Traverse((PPlayer Player) => {
                    // 现在不能主动选将
                    if (Player.IsUser && PNetworkManager.NetworkServer.ChooseManager.AskYesOrNo(Player, "是否使用点将卡？")) {
                        PAge Age = Ages[PNetworkManager.NetworkServer.ChooseManager.Ask(Player, "选择时代", Ages.ConvertAll((PAge _Age) => _Age.Name).ToArray())];
                        List<PGeneral> PossibleGenerals = AvailableGenerals.FindAll((PGeneral General) => General.Age.Equals(Age));
                        PGeneral TargetGeneral = PossibleGenerals[PNetworkManager.NetworkServer.ChooseManager.Ask(Player, "点将", PossibleGenerals.ConvertAll((PGeneral General) => General.Name).ToArray())];
                        Generals[Player.Index] = TargetGeneral;
                        PLogger.Log(Player.Index + "号玩家选择了" + TargetGeneral.Name);
                    }
                }, Game.PlayerList[0]);
                // 去掉重复的将
                AvailableGenerals.ForEach((PGeneral General) => {
                    List<int> ChosenIndex = new List<int>();
                    for (int i = 0; i < 8; ++ i) {
                        if (Generals[i].Equals(General)) {
                            ChosenIndex.Add(i);
                        }
                    }
                    if (ChosenIndex.Count > 1) {
                        PMath.Wash(ChosenIndex);
                        for (int i = 1; i < ChosenIndex.Count; ++ i) {
                            Generals[ChosenIndex[i]] = new P_Soldier();
                        }
                    }
                });
                // 剩下的将随机
                PMath.Wash(AvailableGenerals);
                for (int i = 0; i < Game.PlayerNumber; ++ i) {
                    if (Generals[i] is P_Soldier) {
                        foreach (PGeneral General in AvailableGenerals) {
                            if (!Generals.Contains(General) && (Game.PlayerList[i].IsAI || 
                            PNetworkManager.NetworkServer.ChooseManager.AskHaveGeneral(Game.PlayerList[i], General.Name))) {
                                Generals[i] = General;
                                break;
                            }
                        }
                    }
                }
                #endregion

                Game.Traverse((PPlayer Player) => {
                    Player.General = Generals[Player.Index];

                    Player.Age = Player.General.Age;
                    Player.Sex = Player.General.Sex;
                    Player.General.SkillList.ForEach((PSkill Skill) => {
                        Skill.TriggerList.ForEach((Func<PPlayer, PSkill, PTrigger> TriggerGenerator) => {
                            Game.Monitor.AddTrigger(TriggerGenerator(Player, Skill));
                        });
                    });
                    PNetworkManager.NetworkServer.TellClients(new PRefreshGeneralOrder(Player));
                }, Game.PlayerList[0]);
            }
        });
    }
}