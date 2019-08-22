using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

public class PClient : PAbstractClient {
    private readonly Thread MessageProcessor = null;
    public PGameStatus GameStatus = null;

    public PClient(TcpClient _client) : base(_client) {
        #region 启动处理信息的线程
        MessageProcessor = new Thread(() => {
            while (true) {
                Thread.Sleep(PNetworkConfig.ListenerInterval);
                while (ReceiveNumber > 0) {
                    string message = Receive();
                    if (message != null) {
                        ProcessMessage(message);
                    }
                }
            }
        }) {
            IsBackground = true
        };
        MessageProcessor.Start();
        #endregion
    }

    private void ProcessMessage(string message) {
        PLogger.Log("接收信息：" + message);
        List<string> messages = new List<string>(message.Split(' '));
        messages.RemoveAll((string x) => x.Equals(string.Empty));
        string[] MessageList = messages.ToArray();
        if (MessageList.Length < 1) {
            return;
        }
        string OrderName = MessageList[0];
        try {
            POrder Order = POrder.GetOrder(OrderName);
            Order.ClientResponseAction(MessageList);
        } catch (Exception e) {
            PLogger.Log("客户端处理信息时发生错误：" + message);
            PLogger.Log(e.ToString());
        }
    }

    public new void Stop() {
        if (MessageProcessor != null && MessageProcessor.IsAlive) {
            MessageProcessor.Abort();
        }
        base.Stop();
    }
}