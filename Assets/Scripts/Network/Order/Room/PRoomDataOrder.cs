using System;

/// <summary>
/// 房间数据命令+房间数据信息
/// </summary>
/// CR：处理房间数据，如果RUI没打开就打开，然后刷新RUI上的数据
public class PRoomDataOrder : POrder {
    public PRoomDataOrder() : base("room",
        null,
        (string[] args) => {
            try {
                int Capacity = int.Parse(args[1]);
                PSystem.CurrentRoom = new PRoom(Capacity);
                #region 分析房间的属性
                int Index = 2;
                for (int i = 0; i < Capacity; ++i) {
                    PPlayerType playerType = FindInstance<PPlayerType>(args[Index++]);
                    if (playerType != null) {
                        PSystem.CurrentRoom.PlayerList[i].PlayerType = playerType;
                        PSystem.CurrentRoom.PlayerList[i].Nickname = args[Index++];
                        if (PSystem.CurrentRoom.PlayerList[i].Nickname.Equals("&")) {
                            PSystem.CurrentRoom.PlayerList[i].Nickname = string.Empty;
                        }
                    }
                }
                #endregion
                #region 更新RUI的数据（未打开则先打开）及开始游戏按钮可交互性
                PUIManager.AddNewUIAction("RoomData-更新RUI数据", () => {
                    if (!PUIManager.IsCurrentUI<PRoomUI>()) {
                        PUIManager.ChangeUI<PRoomUI>();
                        //如果游戏已经开始，把游戏关闭
                    }
                    PUIManager.GetUI<PRoomUI>().SeatList.ForEach((PRoomUI.PSeat Seat) => {
                        Seat.Update();
                    });
                    PUIManager.GetUI<PRoomUI>().StartGameButton.interactable = PSystem.CurrentRoom.IsFull();
                });
                #endregion
            } catch (Exception e) {
                PLogger.Log("房间数据错误");
                PLogger.Log(e.ToString());
            }
        }) {
    }
    public PRoomDataOrder(string _RoomData) : this() {
        args = new string[] { _RoomData };
    }
}
