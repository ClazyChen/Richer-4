using System.Collections.Generic;

/// <summary>
/// PGame类：
/// 游戏逻辑顶层模块，运行在Server上
/// </summary>
public class PGame : PGameStatus {
    public PRoom Room;
    public PGameLogic Logic { get; private set; }
    public PTriggerManager Monitor { get; private set; }
    public readonly PTagManager TagManager;

    public bool StartGameFlag { get; private set; }
    public bool EndGameFlag { get; private set; }

    /// <summary>
    /// 新建一个游戏
    /// </summary>
    /// <param name="_Map">原型地图（新建的游戏采用其复制品）</param>
    /// <param name="_GameMode">游戏模式</param>
    public PGame(PMap _Map, PMode _GameMode) : base(_Map, _GameMode) {
        Room = new PRoom(GameMode);
        PLogger.Log("新建游戏，模式：" + GameMode.Name);
        GameMode.Open(this);
        Logic = new PGameLogic(this);
        Monitor = new PTriggerManager(this);
        TagManager = new PTagManager();
        StartGameFlag = false;
        EndGameFlag = false;
    }

    /// <summary>
    /// 开始游戏
    /// </summary>
    /// 和客户端上的开始游戏不同，初始化玩家列表的时候
    /// 从Room而非客户端用于同步的CurrentRoom获得数据
    public new void StartGame() {
        if (Room.IsFull()) {
            #region 初始化玩家列表
            PlayerList = new List<PPlayer>();
            for (int i = 0; i < PlayerNumber; ++i) {
                PlayerList.Add(new PPlayer() {
                    Index = i,
                    Name = Room.PlayerList[i].PlayerType.Equals(PPlayerType.Player) ? Room.PlayerList[i].Nickname : "P" + (i + 1).ToString(),
                    IsAlive = true,
                    Money = PPlayer.Config.DefaultMoney,
                    TeamIndex = GameMode.Seats[i].Party - 1
                });
                PBlock Position = Map.BlockList.Find((PBlock Block) => Block.StartPointIndex == i % Map.StartPointNumber);
                if (Position != null) {
                    PlayerList[i].Position = Position;
                } else {
                    PlayerList[i].Position = Map.BlockList[0];
                }
            }
            #endregion
            NowPlayer = null;
            NowPeriod = null;
            StartGameFlag = true;
            PLogger.Log("开始进行规则装载");
            PObject.ListSubTypeInstances<PSystemTriggerInstaller>().ForEach((PSystemTriggerInstaller Installer) => {
                Installer.Install(Monitor);
            });
            PNetworkManager.NetworkServer.TellClients(new PStartGameOrder(Map.Name));
            Monitor.CallTime(PTime.StartGameTime);
            NowPlayer = PlayerList[0];
            NowPeriod = PPeriod.StartTurn;
            PNetworkManager.NetworkServer.TellClients(new PStartTurnOrder(NowPlayerIndex.ToString()));
            Logic.StartSettle(PPeriod.StartTurn.Execute());
        }
    }

    public void ShutDown() {
        Logic.ShutDown();
        TagManager.RemoveAll();
        PNetworkManager.NetworkServer.TellClients(new PShutDownOrder());
    }
}
