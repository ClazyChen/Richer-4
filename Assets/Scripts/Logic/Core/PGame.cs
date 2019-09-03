using System.Collections.Generic;
using System.Linq;
using System;

/// <summary>
/// PGame类：
/// 游戏逻辑顶层模块，运行在Server上
/// </summary>
public class PGame : PGameStatus {
    public PRoom Room;
    public PGameLogic Logic { get; private set; }
    public PMonitor Monitor { get; private set; }
    public readonly PTagManager TagManager;
    public readonly PCardManager CardManager;

    public bool StartGameFlag { get; private set; }
    public bool EndGameFlag { get; private set; }
    public bool ReadyToStartGameFlag { get; private set; }

    public List<bool> PreparedList;

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
        Monitor = new PMonitor(this);
        TagManager = new PTagManager();
        CardManager = new PCardManager(this);
        StartGameFlag = false;
        EndGameFlag = false;
        ReadyToStartGameFlag = true;
        PreparedList = new List<bool>();
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
                PPlayer Player = new PPlayer() {
                    Index = i,
                    Name = Room.PlayerList[i].PlayerType.Equals(PPlayerType.Player) ? Room.PlayerList[i].Nickname : "P" + (i + 1).ToString(),
                    IsAlive = true,
                    Money = PPlayer.Config.DefaultMoney,
                    TeamIndex = GameMode.Seats[i].Party - 1
                };
                Player.Tags = new PTagManager(Player);
                PlayerList.Add(Player);
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
            EndGameFlag = false;
            ReadyToStartGameFlag = false;
            CardManager.InitializeCardHeap();
            PLogger.Log("开始进行规则装载");
            PObject.ListSubTypeInstances<PSystemTriggerInstaller>().ForEach((PSystemTriggerInstaller Installer) => {
                Installer.Install(Monitor);
            });
            PNetworkManager.NetworkServer.Game.PlayerList.ForEach((PPlayer Player) => {
                PNetworkManager.NetworkServer.TellClient(Player, new PStartGameOrder(Map.Name, Player.Index.ToString()));
            });
            NowPlayer = PlayerList[0];
            Monitor.CallTime(PTime.StartGameTime);
            NowPeriod = PPeriod.StartTurn;
            PNetworkManager.NetworkServer.TellClients(new PStartTurnOrder(NowPlayerIndex.ToString()));
            Logic.StartLogic(PPeriod.StartTurn.Execute());
        }
    }

    public void ShutDown() {
        Logic.ShutDown();
        Monitor.RemoveAll();
        TagManager.RemoveAll();
        CardManager.Clear();
        StartGameFlag = false;
        EndGameFlag = false;
        ReadyToStartGameFlag = false;
        PNetworkManager.NetworkServer.TellClients(new PShutDownOrder());
    }

    public void Toll(PPlayer FromPlayer, PPlayer ToPlayer, PBlock Block) {
        PNetworkManager.NetworkServer.TellClients(new PShowInformationOrder(FromPlayer.Name + "向" + ToPlayer.Name + "收取过路费"));
        PTollTag TollTag = Monitor.CallTime(PTime.Toll.AfterEmitTarget, new PTollTag(FromPlayer, ToPlayer, Block.Toll, Block));
        if (TollTag.ToPlayer != null && TollTag.ToPlayer.IsAlive && TollTag.Toll > 0) {
            TollTag = Monitor.CallTime(PTime.Toll.AfterAcceptTarget, TollTag);
            if (TollTag.ToPlayer != null && TollTag.ToPlayer.IsAlive && TollTag.Toll > 0) {
                Injure(TollTag.FromPlayer, TollTag.ToPlayer, TollTag.Toll, TollTag.Block);
            }
        }
    }

    public void Injure(PPlayer FromPlayer, PPlayer ToPlayer, int Count, PObject InjureSource) {
        PNetworkManager.NetworkServer.TellClients(new PShowInformationOrder((FromPlayer == null ? "null" : FromPlayer.Name) + "对" + ToPlayer.Name + "造成" + Count + "点伤害"));
        PInjureTag InjureTag = Monitor.CallTime(PTime.Injure.StartSettle, new PInjureTag(FromPlayer, ToPlayer, Count, InjureSource));
        foreach (PTime InjureTime in new PTime[] {
            PTime.Injure.BeforeEmitInjure,
            PTime.Injure.BeforeAcceptInjure,
            PTime.Injure.EmitInjure,
            PTime.Injure.AcceptInjure,
            PTime.Injure.AfterEmitInjure,
            PTime.Injure.AfterAcceptInjure
        }) {
            if (InjureTag.ToPlayer != null && InjureTag.Injure > 0) {
                if (InjureTime.Equals(PTime.Injure.AfterEmitInjure)) {
                    if (InjureTag.FromPlayer != null && InjureTag.FromPlayer.IsAlive) {
                        GetMoney(InjureTag.FromPlayer, InjureTag.Injure);
                    }
                } else if (InjureTime.Equals(PTime.Injure.AfterAcceptInjure)) {
                    if (InjureTag.ToPlayer.IsAlive) {
                        LoseMoney(InjureTag.ToPlayer, InjureTag.Injure, true);
                    }
                }
                InjureTag = Monitor.CallTime(InjureTime, InjureTag);
            } else {
                break;
            }
        }
        if (InjureTag.ToPlayer != null && InjureTag.Injure > 0) {
            Monitor.CallTime(PTime.Injure.EndSettle, InjureTag);
        }
    }

    public void GetMoney(PPlayer Player, int Money) {
        PGetMoneyTag GetMoneyTag = Monitor.CallTime(PTime.GetMoneyTime, new PGetMoneyTag(Player, Money));
        PPlayer GetMoneyPlayer = GetMoneyTag.Player;
        int MoneyCount = GetMoneyTag.Money;
        if (GetMoneyPlayer != null && GetMoneyPlayer.IsAlive && MoneyCount > 0) {
            GetMoneyPlayer.Money += MoneyCount;
            PNetworkManager.NetworkServer.TellClients(new PPushTextOrder(GetMoneyPlayer.Index.ToString(), "+" + MoneyCount.ToString(), PPushType.Heal.Name));
            PNetworkManager.NetworkServer.TellClients(new PShowInformationOrder(GetMoneyPlayer.Name + "获得金钱" + MoneyCount.ToString()));
            PNetworkManager.NetworkServer.TellClients(new PRefreshMoneyOrder(GetMoneyPlayer));
        }
    }

    public void LoseMoney(PPlayer Player, int Money, bool IsInjure = false) {
        PLoseMoneyTag LoseMoneyTag = Monitor.CallTime(PTime.LoseMoneyTime, new PLoseMoneyTag(Player, Money, IsInjure));
        PPlayer LoseMoneyPlayer = LoseMoneyTag.Player;
        int MoneyCount = LoseMoneyTag.Money;
        if (LoseMoneyPlayer != null && LoseMoneyPlayer.IsAlive && MoneyCount > 0) {
            LoseMoneyPlayer.Money -= MoneyCount;
            PNetworkManager.NetworkServer.TellClients(new PPushTextOrder(LoseMoneyPlayer.Index.ToString(), "-" + MoneyCount.ToString(), LoseMoneyTag.IsInjure ? PPushType.Injure.Name : PPushType.Throw.Name));
            PNetworkManager.NetworkServer.TellClients(new PShowInformationOrder(LoseMoneyPlayer.Name + "失去金钱" + MoneyCount.ToString()));
            PNetworkManager.NetworkServer.TellClients(new PRefreshMoneyOrder(LoseMoneyPlayer));
            if (LoseMoneyPlayer.Money <= 0) {
                Dying(LoseMoneyPlayer);
            }
        }
    }

    public void Dying(PPlayer Player) {
        PNetworkManager.NetworkServer.TellClients(new PShowInformationOrder(Player.Name + "进入濒死状态"));
        Monitor.CallTime(PTime.EnterDyingTime, new PDyingTag(Player));
        if (Player.Money <= 0) {
            Die(Player);
        }
    }

    public void Die(PPlayer Player) {
        PNetworkManager.NetworkServer.TellClients(new PShowInformationOrder(Player.Name + "死亡！"));
        Map.BlockList.ForEach((PBlock Block) => {
            if (Player.Equals(Block.Lord)) {
                Block.Lord = null;
                Block.HouseNumber = 0;
                Block.BusinessType = PBusinessType.NoType;
                PNetworkManager.NetworkServer.TellClients(new PRefreshBlockBasicOrder(Block));
            }
        });
        CardManager.ThrowAll(Player.Area);
        Player.IsAlive = false;
        PNetworkManager.NetworkServer.TellClients(new PDieOrder(Player.Index.ToString()));
        if (GameOver()) {
            PLogger.Log("游戏结束");
            EndGame();
        } else if (!NowPlayer.IsAlive) {
            PLogger.Log("因为当前玩家死亡，回合中止");
            TagManager.PopTag<PStepCountTag>(PStepCountTag.TagName);
            TagManager.PopTag<PDiceResultTag>(PDiceResultTag.TagName);
            Monitor.EndTurnDirectly = true;
        }
    }

    private bool GameOver() {
        List<int> LivingTeam = new List<int>();
        PlayerList.ForEach((PPlayer Player) => {
            if (Player.IsAlive && !LivingTeam.Contains(Player.TeamIndex)) {
                LivingTeam.Add(Player.TeamIndex);
            }
        });
        return LivingTeam.Count <= 1;
    }

    private string Winners() {
        List<int> LivingTeam = new List<int>();
        PlayerList.ForEach((PPlayer Player) => {
            if (Player.IsAlive && !LivingTeam.Contains(Player.TeamIndex)) {
                LivingTeam.Add(Player.TeamIndex);
            }
        });
        if (LivingTeam.Count < 1) {
            return "null";
        } else {
            int WinnerTeam = LivingTeam[0];
            string WinnerNames = string.Empty;
            PlayerList.ForEach((PPlayer Player) => {
                if (Player.TeamIndex == WinnerTeam) {
                    WinnerNames += "," + Player.Name;
                }
            });
            if (WinnerNames.Length > 0) {
                return WinnerNames.Substring(1);
            } else {
                return "null";
            }
        }
    }

    private void EndGame() {
        if (EndGameFlag) {
            return;
        }
        EndGameFlag = true;
        ReadyToStartGameFlag = false;
        PreparedList = new List<bool>();
        PlayerList.ForEach((PPlayer Player) => {
            PreparedList.Add(Player.IsAI);
        });
        PThread.Async(() => {
            Logic.ShutDown();
            Monitor.RemoveAll();
            TagManager.RemoveAll();
            PNetworkManager.NetworkServer.TellClients(new PGameOverOrder(Winners()));
            ReadyToStartGameFlag = true;
        });
    }

    public void Prepared(string IPAddress) {
        PPlayer TargetPlayer = PNetworkManager.NetworkServer.Game.PlayerList.Find((PPlayer Player) => Player.IPAddress.Equals(IPAddress));
        if (TargetPlayer != null) {
            lock (PreparedList) {
                PreparedList[TargetPlayer.Index] = true;
            }
            if (PreparedList.TrueForAll((bool x)=>x)) {
                PThread.Async(() => {
                    PThread.WaitUntil(() => ReadyToStartGameFlag);
                    StartGame();
                });
            }
        }
    }

    public void GetHouse(PBlock Block, int HouseCount) {
        PGetHouseTag GetHouseTag = Monitor.CallTime(PTime.GetHouseTime, new PGetHouseTag(Block, HouseCount));
        PBlock GetHouseBlock = GetHouseTag.Block;
        int GetHouseCount = GetHouseTag.House;
        if (GetHouseBlock != null && GetHouseCount > 0) {
            GetHouseBlock.HouseNumber += GetHouseCount;
            PNetworkManager.NetworkServer.TellClients(new PRefreshBlockBasicOrder(GetHouseBlock));
        }
    }

    public void LoseHouse(PBlock Block, int HouseCount) {
        PLoseHouseTag LoseHouseTag = Monitor.CallTime(PTime.LoseHouseTime, new PLoseHouseTag(Block, HouseCount));
        PBlock LoseHouseBlock = LoseHouseTag.Block;
        int LoseHouseCount = LoseHouseTag.House;
        if (LoseHouseBlock != null && LoseHouseCount > 0) {
            LoseHouseBlock.HouseNumber -= LoseHouseCount;
            LoseHouseBlock.HouseNumber = Math.Max(0, LoseHouseBlock.HouseNumber);
            PNetworkManager.NetworkServer.TellClients(new PRefreshBlockBasicOrder(LoseHouseBlock));
        }
    }

    public void PurchaseHouse(PPlayer Player, PBlock Block) {
        PPurchaseHouseTag PurchaseHouseTag = Monitor.CallTime(PTime.PurchaseHouseTime, new PPurchaseHouseTag(Player, Block));
        Player = PurchaseHouseTag.Player;
        Block = PurchaseHouseTag.Block;
        if (Player != null && Block != null && Player.IsAlive) {
            PNetworkManager.NetworkServer.TellClients(new PShowInformationOrder(Player.Name + "购买了1座房屋"));
            PNetworkManager.NetworkServer.TellClients(new PHighlightBlockOrder(Block.Index.ToString()));
            if (Block.HousePrice > 0) {
                LoseMoney(Player, Block.HousePrice);
            }
            GetHouse(Block, 1);
            PNetworkManager.NetworkServer.TellClients(new PRefreshBlockBasicOrder(Block));
        }
    }

    public void PurchaseLand(PPlayer Player, PBlock Block) {
        PPurchaseLandTag PurchaseLandTag = Monitor.CallTime(PTime.PurchaseLandTime, new PPurchaseLandTag(Player, Block));
        Player = PurchaseLandTag.Player;
        Block = PurchaseLandTag.Block;
        if (Player != null && Block != null && Player.IsAlive) {
            LoseMoney(Player, Block.Price);
            Block.Lord = Player;
            GetHouse(Block, 1);
            PNetworkManager.NetworkServer.TellClients(new PRefreshBlockBasicOrder(Block));
            if (Block.IsBusinessLand && Block.BusinessType.Equals(PBusinessType.NoType)) {
                PBusinessType ChosenType = PBusinessType.NoType;
                List<PBusinessType> Types = new List<PBusinessType>() { PBusinessType.ShoppingCenter, PBusinessType.Institute,
                            PBusinessType.Pawnshop, PBusinessType.Castle, PBusinessType.Park};
                if (Player.IsUser) {
                    ChosenType = Types[PNetworkManager.NetworkServer.ChooseManager.Ask(Player, "选择商业用地的发展方向", Types.ConvertAll((PBusinessType BusinessType) => BusinessType.Name).ToArray(), Types.ConvertAll((PBusinessType BusinessType) => BusinessType.ToolTip).ToArray())];
                } else {
                    ChosenType = PAiBusinessChooser.ChooseDirection(this, Player, Block);
                }
                Block.BusinessType = ChosenType;
                if (ChosenType.Equals(PBusinessType.Park)) {
                    GetMoney(Player, PMath.Percent(Block.Price, 50));
                } else if (ChosenType.Equals(PBusinessType.Castle)) {
                    GetHouse(Block, GetBonusHouseNumberOfCastle(Player, Block));
                }
                PNetworkManager.NetworkServer.TellClients(new PRefreshBlockBasicOrder(Block));
            } else {
                Block.BusinessType = PBusinessType.NoType;
            }
            PNetworkManager.NetworkServer.TellClients(new PShowInformationOrder(Player.Name + "购买了" + Block.Name));
        }
    }

    public void MovePosition(PPlayer Player, PBlock Source, PBlock Destination) {
        if (Player == null || !Player.IsAlive) {
            return;
        }
        Monitor.CallTime(PTime.MovePositionTime, new PTransportTag(Player, Source, Destination));
    }

    public int Judge(PPlayer Player) {
        if (!Player.IsAlive) {
            return 7;
        }
        int Result = PMath.RandInt(1, 6);
        PNetworkManager.NetworkServer.TellClients(new PShowInformationOrder(Player.Name + "的判定结果：" + Result));
        PNetworkManager.NetworkServer.TellClients(new PPushTextOrder(Player.Index.ToString(), "判定结果：" + Result, PPushType.Information.Name));
        return Result;
    }

    /// <summary>
    /// 拼点
    /// </summary>
    /// <param name="FromPlayer">发起拼点的玩家</param>
    /// <param name="ToPlayer">接受拼点的玩家</param>
    /// <returns>拼点获胜1，失败-1，平均0</returns>
    public int PkPoint(PPlayer FromPlayer, PPlayer ToPlayer) {
        int FromPoint = Judge(FromPlayer);
        int ToPoint = Judge(ToPlayer);
        int Result = 0;
        if (FromPoint > ToPoint) {
            Result = 1;
        } else if (FromPoint < ToPoint) {
            Result = -1;
        }
        PNetworkManager.NetworkServer.TellClients(new PShowInformationOrder(FromPlayer.Name + FromPoint.ToString() + ":" + ToPoint + ToPlayer.Name));
        PNetworkManager.NetworkServer.TellClients(new PPushTextOrder(FromPlayer.Index.ToString(), Result > 0 ? "赢" : "没赢", Result > 0 ? PPushType.Heal.Name : PPushType.Injure.Name));
        return Result;
    }

    public void GetCard(PPlayer Player, int Count) {
        for (int i = 0; i < Count;++i) {
            GetCard(Player);
        }
    }

    public void GetCard(PPlayer Player) {
        if (Player == null || !Player.IsAlive) {
            return;
        }
        if (CardManager.CardHeap.CardNumber == 0 && CardManager.ThrownCardHeap.CardNumber > 0) {
            CardManager.CardHeap.CardList.AddRange(CardManager.ThrownCardHeap.CardList);
            CardManager.ThrownCardHeap.CardList.Clear();
            CardManager.CardHeap.Wash();
        }
        if (CardManager.CardHeap.CardNumber > 0) {
            CardManager.MoveCard(CardManager.CardHeap.TopCard, CardManager.CardHeap, Player.Area.HandCardArea);
        }
        if (NowPeriod != null) {
            PNetworkManager.NetworkServer.TellClients(new PShowInformationOrder(Player.Name + "摸了1张牌"));
        }
    }

    private PCard ChooseCard(PPlayer Player, PPlayer TargetPlayer, string Title, bool AllowEquipment = true, bool AllowJudge = false) {
        PCard TargetCard = null;
        if (Player.IsUser) {
            if (Player.Equals(TargetPlayer)) {
                TargetCard = PNetworkManager.NetworkServer.ChooseManager.AskToChooseOwnCard(Player, Title, AllowEquipment, AllowJudge);
            } else {
                if (!AllowEquipment) {
                    TargetCard = TargetPlayer.Area.HandCardArea.RandomCard();
                } else {
                    TargetCard = PNetworkManager.NetworkServer.ChooseManager.AskToChooseOthersCard(Player, TargetPlayer, Title, AllowJudge);
                }
            }
        } else {
            if (Player.TeamIndex == TargetPlayer.TeamIndex) {
                TargetCard = PAiCardExpectation.FindLeastValuable(this, Player, TargetPlayer, AllowEquipment, AllowJudge, Player.Equals(TargetPlayer)).Key;
            } else {
                TargetCard = PAiCardExpectation.FindMostValuable(this, Player, TargetPlayer, AllowEquipment, AllowJudge).Key;
            }
        }
        return TargetCard;
    }

    /// <summary>
    /// 弃牌行为
    /// </summary>
    /// <param name="Player">发起弃牌方</param>
    /// <param name="TargetPlayer">被弃牌方</param>
    public void ThrowCard(PPlayer Player, PPlayer TargetPlayer, bool AllowEquipment = true, bool AllowJudge = false) {
        string Title = "请选择弃置一张手牌" + (AllowEquipment ? "或装备" : string.Empty) + (AllowJudge ? "或伏兵" : string.Empty);
        PCard TargetCard = ChooseCard(Player, TargetPlayer, Title, AllowEquipment, AllowJudge);
        if (TargetCard != null) {
            PNetworkManager.NetworkServer.TellClients(new PShowInformationOrder(Player.Name + "弃置了" + TargetPlayer.Name + "的" + TargetCard.Name));
            CardManager.MoveCard(TargetCard, TargetPlayer.Area.HandCardArea, CardManager.ThrownCardHeap);
        }
    }

    /// <summary>
    /// 获得牌行为
    /// </summary>
    /// <param name="Player">获得牌的玩家</param>
    /// <param name="TargetPlayer">被获得牌的玩家</param>
    public void GetCardFrom(PPlayer Player, PPlayer TargetPlayer, bool AllowEquipment = true, bool AllowJudge = false) {
        string Title = "请选择获得一张手牌" + (AllowEquipment ? "或装备" : string.Empty) + (AllowJudge ? "或伏兵" : string.Empty);
        PCard TargetCard = ChooseCard(Player, TargetPlayer, Title, AllowEquipment, AllowJudge);
        if (TargetCard != null) {
            PNetworkManager.NetworkServer.TellClients(new PShowInformationOrder(Player.Name + "获得了" + TargetPlayer.Name + "的" + (TargetPlayer.Area.HandCardArea.CardList.Contains(TargetCard) ? "1张手牌" : TargetCard.Name)));
            CardManager.MoveCard(TargetCard, TargetPlayer.Area.HandCardArea, Player.Area.HandCardArea);
        }
    }

    public void GiveCardTo(PPlayer Player, PPlayer TargetPlayer, bool AllowEquipment = true, bool AllowJudge = false) {
        string Title = "请选择给出一张手牌" + (AllowEquipment ? "或装备" : string.Empty) + (AllowJudge ? "或伏兵" : string.Empty);
        PCard Card = null;
        if (Player.IsAI) {
            if (Player.TeamIndex == TargetPlayer.TeamIndex) {
                Card = PAiCardExpectation.FindMostValuable(this, TargetPlayer, Player, AllowEquipment, AllowJudge, true).Key;
            } else {
                Card = PAiCardExpectation.FindLeastValuable(this, TargetPlayer, Player, AllowEquipment, AllowJudge, true).Key;
            }
        } else {
            Card = PNetworkManager.NetworkServer.ChooseManager.AskToChooseOwnCard(Player, Title, AllowEquipment, AllowJudge);
        }
        if (Card != null) {
            PNetworkManager.NetworkServer.TellClients(new PShowInformationOrder(Player.Name + "交给了" + TargetPlayer.Name + (Player.Area.HandCardArea.CardList.Contains(Card) ? "1张手牌" : Card.Name)));
            CardManager.MoveCard(Card, Player.Area.HandCardArea, TargetPlayer.Area.HandCardArea);
        }
    }

    public void ChangeFace(PPlayer Player) {
        PNetworkManager.NetworkServer.TellClients(new PPushTextOrder(Player.Index.ToString(), "翻面", PPushType.Information.Name));
        if (Player.BackFace) {
            Player.Tags.PopTag<PTag>(PTag.BackFaceTag.Name);
        } else {
            Player.Tags.CreateTag(PTag.BackFaceTag);
        }
        Monitor.CallTime(PTime.ChangeFaceTime, new PChangeFaceTag(Player));
    }
}
