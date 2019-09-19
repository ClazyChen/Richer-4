using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PTestUI : PAbstractUI {
    public readonly Button EnterButton;
    public readonly Button ReturnButton;
    public readonly InputField CodeInputField;
    public readonly InputField ResultInputField;

    public PTestUI(Transform _Background) : base(_Background) {
        InitializeControls<Button>();
        InitializeControls<InputField>();
        Close();
    }

    private List<PGeneral> GenerateGenerals(string Config) {
        string[] Parts = CodeInputField.text.Split(';');
        bool Random = Parts[Parts.Length - 2].Equals("Random");
        List<PGeneral> GeneralList = new List<PGeneral>();
        for (int i = 0; i < PSystem.CurrentMode.PlayerNumber; ++i) {
            if (i >= Parts.Length - 2) {
                GeneralList.Add(new P_Soldier());
            } else {
                PGeneral General = ListSubTypeInstances<PGeneral>().Find((PGeneral _General) => _General.Name.Equals(Parts[i]));
                if (General != null) {
                    GeneralList.Add(General);
                } else {
                    GeneralList.Add(new P_Soldier());
                }
            }
        }
        if (Random) {
            PMath.Wash(GeneralList);
        }
        return GeneralList;
    }

    public override void Open() {
        base.Open();
        #region 返回按钮：回到InitialUI
        ReturnButton.onClick.AddListener(() => {
            PNetworkManager.AbortServer();
            PUIManager.AddNewUIAction("返回：转到IUI", () => PUIManager.ChangeUI<PInitialUI>());
        });
        #endregion
        #region 确定按钮：创建服务器
        EnterButton.onClick.AddListener(() => {
            string[] Parts = CodeInputField.text.Split(';');
            if (Parts.Length <= 2) {
                return;
            }
            int Times = Math.Min(100,Math.Max(1,Convert.ToInt32(Parts[Parts.Length - 1])));
            PSystem.AllAiConfig = CodeInputField.text;
            PNetworkManager.CreateSingleServer(PSystem.CurrentMap, PSystem.CurrentMode);
            PThread.Async(() => {
                for (int i = 0; i < Times; ++ i) {
                    string Time = DateTime.Now.ToLocalTime().ToString();
                    //PLogger.StartLogging(false);
                    List<PGeneral> GeneralList = GenerateGenerals(PSystem.AllAiConfig);
                    PNetworkManager.Game.Room.PlayerList.ForEach((PRoom.PlayerInRoom Player) => Player.PlayerType = PPlayerType.AI);
                    PNetworkManager.Game.StartGame(GeneralList);
                    PThread.WaitUntil(() => PNetworkManager.Game.ReadyToStartGameFlag);
                    PUIManager.AddNewUIAction("增加结果序列", () => {
                        PUIManager.GetUI<PTestUI>().ResultInputField.text += "Time: " + Time + "; Position: " + string.Join(",", PNetworkManager.Game.PlayerList.ConvertAll((PPlayer Player) => Player.General.Name)) + "; Winners: " + PNetworkManager.Game.Winners(true) + "\n";
                    });
                }
            });
        });
        #endregion
    }
}