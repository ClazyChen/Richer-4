using System.Collections.Generic;

/// <summary>
/// PGame类：
/// 游戏逻辑顶层模块，运行在Server上
/// </summary>
public class PGame : PGameStatus {
    public PRoom Room;
    public PGameLogic Logic { get; private set; }

    public bool StartGameFlag { get; private set; }
    public bool EndGameFlag { get; private set; }

    //public PMarkManager GlobalMarks;
    //public PTimeManager Monitor;

    private const float AIOperationDelay = 0.2f;

    /// <summary>
    /// 新建一个游戏
    /// </summary>
    /// <param name="_Map">原型地图（新建的游戏采用其复制品）</param>
    /// <param name="_GameMode">游戏模式</param>
    public PGame(PMap _Map, PMode _GameMode) : base(_Map, _GameMode) {
        Room = new PRoom(GameMode);
        GameMode.Open(this);
        Logic = new PGameLogic();
        //GlobalMarks = new PMarkManager();
        //Monitor = new PTimeManager(this);
        StartGameFlag = false;
        EndGameFlag = false;
    }

    /*

    /// <summary>
    /// 生成预置的效果组加入到Monitor中
    /// </summary>
    private void GenerateSystemEffects() {
        foreach (PPlayer Player in PlayerList) {
            if (Player.IsUser) {
                Monitor.AddEffect(PTime.FreeTime, PEffect.FreeTimeOperation(Player));
            }
        }
        new PDiceEffectInstaller(this).InstallEffects();
        new PWalkEffectInstaller(this).InstallEffects();
        new PTransportEffectInstaller(this).InstallEffects();
    }
    */

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
            StartGameFlag = true;
            PNetworkManager.NetworkServer.TellClients(new PStartGameOrder(Map.Name));
            Logic.StartGameLogicThread();
            //GlobalMarks.RemoveAll();
            //Monitor.Reset();
            //GenerateSystemEffects();
            Logic.EnqueueAction("游戏开始时", () => {
                //StartGameTime();
            });
        }
    }

    public void ShutDown() {
        Logic.ShutDownGameLogicThread();
        PNetworkManager.NetworkServer.TellClients(new PShutDownOrder());
    }

    //以下操作均在GLT中进行

        /*

    /// <summary>
    /// 游戏开始时
    /// </summary>
    public void StartGameTime() {
        Monitor.CallTime(PTime.StartGame);
        Logic.EnqueueAction("回合开始时", () => {
            ProcessStage(PTime.StartMain, PlayerList[0]);
        });
    }

    private readonly PTime[] TurnFlow = {
        PTime.StartMain,
        PTime.PreparationStage,
        PTime.JudgeStage,
        PTime.FirstFreeTime,
        PTime.DiceStage,
        PTime.WalkingStage,
        PTime.SettleStage,
        PTime.SecondFreeTime,
        PTime.EndMainStage,
        PTime.EndMain
    };

    /// <summary>
    /// 获取下一个阶段
    /// </summary>
    /// <param name="Stage">这一个阶段</param>
    /// <returns>紧跟在Stage之后的下一个阶段（回合结束接回合开始）</returns>
    public PTime GetNextStage(PTime Stage) {
        for (int i = 0; i < TurnFlow.Length - 1; ++i) {
            if (TurnFlow[i].Equals(Stage)) {
                return TurnFlow[i + 1];
            }
        }
        return PTime.StartMain;
    }

    /// <summary>
    /// 获取下一个玩家
    /// </summary>
    /// <param name="Player">当前玩家</param>
    /// <returns>当前玩家的下家</returns>
    public PPlayer GetNextPlayer(PPlayer Player) {
        int Index = Player.Index + 1;
        if (Index >= PlayerNumber) {
            Index = 0;
        }
        while (!PlayerList[Index].IsAlive) {
            ++Index;
            if (Index == Player.Index + 1) {
                return Player;
            } else if (Index >= PlayerNumber) {
                Index = 0;
            }
        }
        return PlayerList[Index];
    }

    /// <summary>
    /// 处理一个阶段的结算
    /// </summary>
    /// <param name="Stage">进行的阶段</param>
    /// <param name="Player">进行阶段的玩家</param>
    /// 回合开始和回合结束的时候需要广播
    public void ProcessStage(PTime Stage, PPlayer Player) {
        if (Stage.Equals(PTime.StartMain)) {
            NowPlayer = Player;
            NowPlayer.TurnNumber++;
            PNetworkManager.NetworkServer.TellClients(new PStartMainOrder(NowPlayerIndex.ToString()));
        } else if (Stage.Equals(PTime.EndMain)) {
            PNetworkManager.NetworkServer.TellClients(new PEndMainOrder(NowPlayerIndex.ToString()));
        }
        Monitor.CallTime(Stage);
        PTime NextStage = GetNextStage(Stage);
        PPlayer NextPlayer = NextStage.Equals(PTime.StartMain) ? GetNextPlayer(Player) : Player;
        Logic.EnqueueAction(NextStage.Name, () => {
            ProcessStage(NextStage, NextPlayer);
        });
    }
    */
}
