﻿using System.Collections.Generic;
using System.Linq;

/// <summary>
/// PGame类：
/// 游戏逻辑顶层模块，运行在Server上
/// </summary>
public class PGame : PGameStatus {
    public PRoom Room;
    public PGameLogic Logic { get; private set; }
    public PMonitor Monitor { get; private set; }
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
        Monitor = new PMonitor(this);
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
            PNetworkManager.NetworkServer.Game.PlayerList.ForEach((PPlayer Player) => {
                PNetworkManager.NetworkServer.TellClient(Player, new PStartGameOrder(Map.Name, Player.Index.ToString()));
            });
            Monitor.CallTime(PTime.StartGameTime);
            NowPlayer = PlayerList[0];
            NowPeriod = PPeriod.StartTurn;
            PNetworkManager.NetworkServer.TellClients(new PStartTurnOrder(NowPlayerIndex.ToString()));
            PThread.Async(() => {
                Logic.StartSettle(PPeriod.StartTurn.Execute());
            });
        }
    }

    public void ShutDown() {
        Logic.ShutDown();
        TagManager.RemoveAll();
        PNetworkManager.NetworkServer.TellClients(new PShutDownOrder());
    }

    public void Toll(PPlayer FromPlayer, PPlayer ToPlayer, PBlock Block) {
        PNetworkManager.NetworkServer.TellClients(new PShowInformationOrder(FromPlayer.Name + "向" + ToPlayer.Name + "收取过路费"));
        PTollTag TollTag = Monitor.CallTime(PTime.Toll.AfterEmitTarget, new PTollTag(FromPlayer, ToPlayer, Block.Toll));
        if (TollTag.ToPlayer != null && TollTag.ToPlayer.IsAlive && TollTag.Toll > 0) {
            TollTag = Monitor.CallTime(PTime.Toll.AfterAcceptTarget, TollTag);
            if (TollTag.ToPlayer != null && TollTag.ToPlayer.IsAlive && TollTag.Toll > 0) {
                Injure(TollTag.FromPlayer, TollTag.ToPlayer, TollTag.Toll);
            }
        }
    }

    public void Injure(PPlayer FromPlayer, PPlayer ToPlayer, int Count) {
        PNetworkManager.NetworkServer.TellClients(new PShowInformationOrder((FromPlayer == null ? "null" : FromPlayer.Name) + "对" + ToPlayer.Name + "造成" + Count + "点伤害"));
        PInjureTag InjureTag = Monitor.CallTime(PTime.Injure.StartSettle, new PInjureTag(FromPlayer, ToPlayer, Count));
        foreach (PTime InjureTime in new PTime[] {
            PTime.Injure.BeforeEmitInjure,
            PTime.Injure.BeforeAcceptInjure,
            PTime.Injure.EmitInjure,
            PTime.Injure.AcceptInjure,
            PTime.Injure.AfterEmitInjure,
            PTime.Injure.AfterAcceptInjure
        }) {
            if (InjureTag.ToPlayer != null && InjureTag.ToPlayer.IsAlive && InjureTag.Injure > 0) {
                if (InjureTime.Equals(PTime.Injure.AfterEmitInjure)) {
                    if (InjureTag.FromPlayer != null && InjureTag.FromPlayer.IsAlive) {
                        GetMoney(InjureTag.FromPlayer, InjureTag.Injure);
                    }
                } else if (InjureTime.Equals(PTime.Injure.AfterAcceptInjure)) {
                    LoseMoney(InjureTag.ToPlayer, InjureTag.Injure, true);
                }
                InjureTag = Monitor.CallTime(InjureTime, InjureTag);
            } else {
                break;
            }
        }
        Monitor.CallTime(PTime.Injure.EndSettle, InjureTag);
    }

    public void GetMoney(PPlayer Player, int Money) {
        PGetMoneyTag GetMoneyTag = Monitor.CallTime(PTime.GetMoneyTime, new PGetMoneyTag(Player, Money));
        PPlayer GetMoneyPlayer = GetMoneyTag.Player;
        int MoneyCount = GetMoneyTag.Money;
        if (GetMoneyPlayer != null && MoneyCount > 0) {
            GetMoneyPlayer.Money += MoneyCount;
            PNetworkManager.NetworkServer.TellClients(new PPushTextOrder(GetMoneyPlayer.Index.ToString(), "+" + MoneyCount.ToString(), PPushType.Heal.Name));
            PNetworkManager.NetworkServer.TellClients(new PShowInformationOrder(GetMoneyPlayer.Name + "获得金钱" + MoneyCount.ToString()));
            PNetworkManager.NetworkServer.TellClients(new PRefreshMoneyOrder(Player));
        }
    }

    public void LoseMoney(PPlayer Player, int Money, bool IsInjure = false) {
        PLoseMoneyTag LoseMoneyTag = Monitor.CallTime(PTime.LoseMoneyTime, new PLoseMoneyTag(Player, Money, IsInjure));
        PPlayer LoseMoneyPlayer = LoseMoneyTag.Player;
        int MoneyCount = LoseMoneyTag.Money;
        if (LoseMoneyPlayer != null && MoneyCount > 0) {
            LoseMoneyPlayer.Money -= MoneyCount;
            PNetworkManager.NetworkServer.TellClients(new PPushTextOrder(LoseMoneyPlayer.Index.ToString(), "-" + MoneyCount.ToString(), LoseMoneyTag.IsInjure ? PPushType.Injure.Name : PPushType.Throw.Name));
            PNetworkManager.NetworkServer.TellClients(new PShowInformationOrder(LoseMoneyPlayer.Name + "失去金钱" + MoneyCount.ToString()));
            PNetworkManager.NetworkServer.TellClients(new PRefreshMoneyOrder(Player));
        }
    }

    public void GetHouse(PBlock Block, int HouseCount) {
        PGetHouseTag GetHouseTag = Monitor.CallTime(PTime.GetHouseTime, new PGetHouseTag(Block, HouseCount));
        PBlock GetHouseBlock = GetHouseTag.Block;
        int GetHouseCount = GetHouseTag.House;
        if (GetHouseBlock != null && GetHouseCount > 0) {
            GetHouseBlock.HouseNumber += GetHouseCount;
            PNetworkManager.NetworkServer.TellClients(new PRefreshBlockBasicOrder(Block));
        }
    }

    public void PurchaseHouse(PPlayer Player, PBlock Block) {
        PPurchaseHouseTag PurchaseHouseTag = Monitor.CallTime(PTime.PurchaseHouseTime, new PPurchaseHouseTag(Player, Block));
        Player = PurchaseHouseTag.Player;
        Block = PurchaseHouseTag.Block;
        if (Player != null && Block != null) {
            if (Block.Price > 0) {
                LoseMoney(Player, Block.Price);
            }
            GetHouse(Block, 1);
            PNetworkManager.NetworkServer.TellClients(new PRefreshBlockBasicOrder(Block));
        }
    }

    public void PurchaseLand(PPlayer Player, PBlock Block) {
        PPurchaseLandTag PurchaseLandTag = Monitor.CallTime(PTime.PurchaseLandTime, new PPurchaseLandTag(Player, Block));
        Player = PurchaseLandTag.Player;
        Block = PurchaseLandTag.Block;
        if (Player != null && Block != null) {
            LoseMoney(Player, Block.Price);
            Block.Lord = Player;
            GetHouse(Block, 1);
            PNetworkManager.NetworkServer.TellClients(new PRefreshBlockBasicOrder(Block));
            if (Block.IsBusinessLand && Block.BusinessType.Equals(PBusinessType.NoType)) {
                PBusinessType ChosenType = PBusinessType.NoType;
                List<PBusinessType> Types = new List<PBusinessType>() { PBusinessType.ShoppingCenter, PBusinessType.Institute,
                            PBusinessType.Pawnshop, PBusinessType.Castle, PBusinessType.Park};
                if (Player.IsUser) {
                    ChosenType = Types[PNetworkManager.NetworkServer.ChooseManager.Ask(Player, "选择商业用地的发展方向", Types.ConvertAll((PBusinessType BusinessType) => BusinessType.Name).ToArray())];
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
        Monitor.CallTime(PTime.MovePositionTime, new PTransportTag(Player, Source, Destination));
    }
}
