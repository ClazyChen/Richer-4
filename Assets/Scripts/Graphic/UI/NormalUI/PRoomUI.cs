using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PRoomUI : PAbstractUI {
    public readonly Button StartGameButton;
    public readonly Button ReturnButton;
    public readonly Text IPAddressText;

    public class PSeat : PAbstractUI {
        public readonly Image Background;
        public readonly Button PlayerButton;
        public readonly Button MoveButton;
        public readonly int Index;
        public PSeat(Transform _Background, int _Index) : base(_Background) {
            Index = _Index;
            InitializeControls<Button>();
            Background = UIBackgroundImage.GetComponent<Image>();
            Close();
        }

        public override void Open() {
            base.Open();
            SetText();
            #region 切换AI/Waiting按钮
            PlayerButton.onClick.AddListener(() => {
                if (!PSystem.CurrentRoom.PlayerList[Index].PlayerType.Equals(PPlayerType.Player)) {
                    PNetworkManager.NetworkClient.Send(new PSwitchSeatOrder(Index.ToString()));
                }
            });
            #endregion
            #region 移动位置按钮
            MoveButton.onClick.AddListener(() => {
                if (!PSystem.CurrentRoom.PlayerList[Index].PlayerType.Equals(PPlayerType.Player)) {
                    PNetworkManager.NetworkClient.Send(new PMoveSeatOrder(Index.ToString()));
                }
            });
            #endregion
        }

        public void Update() {
            if (IsActive) {
                SetText();
            }
        }

        private void SetText() {
            PRoom.PlayerInRoom Player = PSystem.CurrentRoom.GetPlayer(Index);
            if (Player != null) {
                PPlayerType playerType = Player.PlayerType;
                if (playerType.Equals(PPlayerType.Waiting)) {
                    SetText("Waiting");
                } else if (playerType.Equals(PPlayerType.AI)) {
                    if (Player.Nickname.Equals(string.Empty)) {
                        SetText("AI");
                    } else {
                        SetText("AI\n" + Player.Nickname);
                    }
                } else if (playerType.Equals(PPlayerType.Player)) {
                    SetText(Player.Nickname);
                } else {
                    Player.PlayerType = PPlayerType.Waiting;
                    SetText();
                }
            }
        }

        private void SetText(string text) {
            if (IsActive) {
                Text PlayerButtonText = PlayerButton.GetComponentInChildren<Text>();
                if (PlayerButtonText != null) {
                    PlayerButtonText.text = text;
                }
            }
        }
    }
    public List<PSeat> SeatList { get; private set; }

    private const int MaxCapacity = 8;

    public PRoomUI(Transform _Background) : base(_Background) {
        InitializeControls<Button>();
        InitializeControls<Text>();
        SeatList = new List<PSeat>();
        for (int i = 0; i < MaxCapacity; ++ i ) {
            SeatList.Add(new PSeat(UIBackgroundImage.Find("SeatImage (" + (i + 1).ToString() + ")"), i));
        }
        Close();
    }

    public override void Open() {
        #region 根据当前模式调整房间内座位数量和颜色
        for (int i = 0; i < MaxCapacity; ++ i) {
            if (i < PSystem.CurrentMode.PlayerNumber) {
                SeatList[i].Open();
            } else {
                SeatList[i].Close();
            }
        }
        for (int i = 0; i < PSystem.CurrentMode.PlayerNumber; ++i) {
            SeatList[i].Background.color = PPlayerScene.Config.PlayerColors[PSystem.CurrentMode.Seats[i].Party - 1];
        }
        #endregion
        #region 显示IP地址
        IPAddressText.text = "本机IP:" + PNetworkConfig.IP.ToString();
        #endregion
        #region 返回按钮：转到InitialUI并关闭服务器/客户端
        ReturnButton.onClick.AddListener(() => {
            PNetworkManager.AbortClient();
            if (PNetworkManager.CurrentHostType.Equals(PHostType.Server)) {
                PNetworkManager.AbortServer();
            }
            PUIManager.AddNewUIAction("返回：转到IUI", () => PUIManager.ChangeUI<PInitialUI>());
        });
        #endregion
        #region 开始游戏按钮：发送开始游戏请求
        StartGameButton.onClick.AddListener(() => {
            PNetworkManager.NetworkClient.Send(new PStartGameOrder());
        });
        #endregion
        base.Open();
    }

    public override void Close() {
        StartGameButton.interactable = false;
        foreach (PSeat Seat in SeatList) {
            Seat.Close();
        }
        base.Close();
    }
}
