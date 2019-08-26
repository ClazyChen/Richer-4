using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;
using System;

/*
 * PServer类：
 * 用于实现一个服务器
 * 其中游戏本体逻辑是跑在服务器上的
 */

public class PServer {
    public int maxConnectionNumber = 8;
    public PGame Game = null;
    public PChooseManager ChooseManager = null;

    /// <summary>
    /// 用于和客户端直接通信的TCP客户端
    /// </summary>
    public class PClientCommander : PAbstractClient {
        public string RemoteIPAddress { get; private set; }
        public PClientCommander(TcpClient _client) : base(_client) {
            RemoteIPAddress = _client.Client.RemoteEndPoint.ToString();
        }

        public new void Send(string message) {
            base.Send(message);
            PLogger.Log("发送给 (" + RemoteIPAddress + "):" + message);
        }
    }

    private TcpListener Listener = null;
    private Thread ServerThread = null;
    private volatile List<PClientCommander> CommanderList = new List<PClientCommander>();
    private Thread MessageProcessor = null;
    private Thread ProtectorThread = null;

    public PServer() {
        #region 启动服务器线程，侦听客户端的连接请求
        ServerThread = new Thread(() => {
            #region 获取IP地址并启动监听器
            IPAddress IPv4Address = PNetworkConfig.IP;
            Listener = new TcpListener(IPv4Address, PNetworkConfig.ServerPort);
            Listener.Start();
            #endregion
            while (true) {
                TcpClient client = Listener.AcceptTcpClient();
                PLogger.Log("收到连接请求：" + client.Client.RemoteEndPoint.ToString());
                #region 判断列表是否已满，发送接受或拒绝命令给请求连接的客户端
                if (CommanderList.Count < maxConnectionNumber) {
                    #region 发送接受命令，建立和该客户端通信的命令器
                    PClientCommander commander = new PClientCommander(client);
                    commander.Send(new PAcceptOrder());
                    lock (CommanderList) {
                        CommanderList.Add(commander);
                    }
                    #endregion
                } else {
                    #region 建立一个临时的命令器发送拒绝命令
                    new Thread(() => {
                        PClientCommander tempCommander = new PClientCommander(client);
                        tempCommander.Send(new PRejectOrder());
                        Thread.Sleep(20);
                        tempCommander.Stop();
                    }) {
                        IsBackground = true
                    }.Start();
                    #endregion
                }
                #endregion
            }
        }) {
            IsBackground = true
        };
        ServerThread.Start();
        #endregion
        #region 启动信息处理线程，每10ms处理一次来自其他客户端的消息
        MessageProcessor = new Thread(() => {
            while (true) {
                Thread.Sleep(10);
                foreach (PClientCommander commander in CommanderList) {
                    while (commander.ReceiveNumber > 0) {
                        string message = commander.Receive();
                        if (message != null) {
                            ProcessMessage(message, commander.RemoteIPAddress);
                        }
                    }
                }
            }
        }) {
            IsBackground = true
        };
        MessageProcessor.Start();
        #endregion
        #region 启动保护线程检测掉线事件
        ProtectorThread = new Thread(() => {
            while (true) {
                bool Disconnect = false;
                for (int i = CommanderList.Count - 1; i >= 0; --i) {
                    if (CommanderList[i] != null) {
                        if (!CommanderList[i].Connected) {
                            CommanderList[i].Stop();
                            string DisconnectIP = CommanderList[i].RemoteIPAddress;
                            PLogger.Log("网络断开：" + DisconnectIP);
                            PNetworkManager.Game.Room.RemovePlayer(CommanderList[i].RemoteIPAddress);
                            CommanderList.RemoveAt(i);
                            Disconnect = true;
                        }
                    }
                }
                if (Disconnect) {
                    if (Game.StartGameFlag) {
                        Game.ShutDown();
                        PNetworkManager.AbortServer();
                        PUIManager.AddNewUIAction("客户端断开-返回InitialUI", () => {
                            PUIManager.ChangeUI<PInitialUI>();
                        });
                    } else {
                        TellClients(new PRoomDataOrder(Game.Room.ToString()));
                    }
                }
                Thread.Sleep(20);
            }
        }) {
            IsBackground = true
        };
        ProtectorThread.Start();
        #endregion
        ChooseManager = new PChooseManager();
    }

    private void ProcessMessage(string message, string IPAddress) {
        PLogger.Log("接收信息 (" + IPAddress + "):" + message);
        List<string> messages = new List<string>(message.Split(' '));
        messages.RemoveAll((string x) => x.Equals(string.Empty));
        string[] MessageList = messages.ToArray();
        if (MessageList.Length < 1) {
            return;
        }
        string OrderName = MessageList[0];
        try {
            POrder Order = POrder.GetOrder(OrderName);
            Order.ServerResponseAction(MessageList, IPAddress);
        } catch (Exception e) {
            PLogger.Log("服务器处理信息时发生错误：" + message);
            PLogger.Log(e.ToString());
        }
    }


    #region 发送消息的方法

    /// <summary>
    /// 向所有的客户端发送命令
    /// </summary>
    /// <param name="order">命令</param>
    public void TellClients(POrder order) {
        foreach (PClientCommander commander in CommanderList) {
            commander.Send(order);
        }
    }
    /// <summary>
    /// 向客户端发送命令
    /// </summary>
    /// <param name="IPAddress">目标客户端的IP地址</param>
    /// <param name="order">命令</param>
    public void TellClient(string IPAddress, POrder order) {
        foreach (PClientCommander commander in CommanderList) {
            if (commander.RemoteIPAddress.Equals(IPAddress)) {
                commander.Send(order);
                break;
            }
        }
    }
    /// <summary>
    /// 向客户端发送命令
    /// </summary>
    /// <param name="Player">目标客户端扮演的玩家</param>
    /// <param name="order">命令</param>
    public void TellClient(PPlayer Player, POrder order) {
        if (Player.IsUser) {
            TellClient(Player.IPAddress, order);
        }
    }
    #endregion

    /// <summary>
    /// 关闭服务器：关闭服务器线程、侦听器、TCP客户端和游戏逻辑体
    /// </summary>
    public void Close() {
        if (ServerThread != null && ServerThread.IsAlive) {
            ServerThread.Abort();
        }
        if (ProtectorThread != null && ProtectorThread.IsAlive) {
            ProtectorThread.Abort();
        }
        if (MessageProcessor != null && MessageProcessor.IsAlive) {
            MessageProcessor.Abort();
        }
        //ShutDownGameLogicThread();
        if (Listener != null) {
            Listener.Stop();
        }
        foreach (PClientCommander commander in CommanderList) {
            if (commander != null) {
                commander.Stop();
            }
        }
    }
}
