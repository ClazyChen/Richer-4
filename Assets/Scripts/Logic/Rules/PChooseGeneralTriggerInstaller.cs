using System;
using System.IO;
using System.Text;
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
                AvailableGenerals = ListSubTypeInstances<PGeneral>().FindAll((PGeneral General) => !(General is P_Soldier) && General.CanBeChoose);
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
                // 每个人有4个将可选
                // 其中最多1个免费将
                List<PGeneral> Selected = new List<PGeneral>();
                for (int i = 0; i < Game.PlayerNumber; ++ i) {
                    if (Generals[i] is P_Soldier) {
                        List<PGeneral> PossibleGenerals = new List<PGeneral>();
                        PMath.Wash(AvailableGenerals);
                        bool NoFreeGeneral = true;
                        foreach (PGeneral General in AvailableGenerals) {
                            if (!Generals.Contains(General) && !Selected.Contains(General) && (NoFreeGeneral || General.Cost != 0)) {
                                if (Game.PlayerList[i].IsAI ||
                                    PNetworkManager.NetworkServer.ChooseManager.AskHaveGeneral(Game.PlayerList[i], General.Name)) {
                                    Selected.Add(General);
                                    PossibleGenerals.Add(General);
                                    if (General.Cost == 0) {
                                        NoFreeGeneral = false;
                                    }
                                }
                            }
                            if (PossibleGenerals.Count >= 4) {
                                break;
                            }
                        }
                        if (PossibleGenerals.Count == 1) {
                            Generals[i] = PossibleGenerals[0];
                        } else {
                            if (Game.PlayerList[i].IsAI) {
                                PMath.Wash(PossibleGenerals);
                                Generals[i] = PossibleGenerals.Find((PGeneral _General) => _General.NewGeneral);
                                if (Generals[i] == null) {
                                    Generals[i] = PossibleGenerals[0];
                                }
                            } else {
                                Generals[i] = PossibleGenerals[PNetworkManager.NetworkServer.ChooseManager.Ask(Game.PlayerList[i], "点将",
                                    PossibleGenerals.ConvertAll((PGeneral General) => General.Name).ToArray())];
                            }
                        }
                    }
                }
                #endregion

                #region 全AI模式下指定武将
                if (Game.PlayerList.TrueForAll((PPlayer _Player) => _Player.IsAI)) {
                    string dataDirectory = PPath.GetPath("Data\\User\\special.txt");
                    StreamReader ArcFileReader = new StreamReader(dataDirectory, Encoding.UTF8);
                    string Line = string.Empty;
                    while ((Line = ArcFileReader.ReadLine()) != null) {
                        if (Line.Length > 0 && Line[0] != '#') {
                            string[] LineData = Line.Split('|');
                            if (LineData.Length == Game.PlayerNumber) {
                                for (int i = 0; i < Game.PlayerNumber; ++ i) {
                                    PGeneral General = AvailableGenerals.Find((PGeneral _General) => _General.Name.Equals(LineData[i]));
                                    if (General != null) {
                                        Generals[i] = General;
                                    }
                                }
                            }
                            break;
                        }
                    }
                    ArcFileReader.Close();
                }
                #endregion

                Game.Traverse((PPlayer Player) => {
                    if (Player.General is P_Soldier) {
                        Player.General = Generals[Player.Index];
                    }
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