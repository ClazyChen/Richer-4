using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public abstract class PAbstractClient {
    /// <summary>
    /// 被封装的TCP客户端
    /// </summary>
    public TcpClient Client { get; private set; }
    private volatile NetworkStream stream;
    private Queue<string> recvQueue = new Queue<string>();
    private Queue<string> sendQueue = new Queue<string>();
    /// <summary>
    /// 上一条未完成发送的字符串
    /// </summary>
    private string ReceiveBufferString = string.Empty;

    private volatile bool _Connected = false;
    private volatile bool StopConnection = false;
    public bool Connected { get { return _Connected; } }
    /// <summary>
    /// 监听线程
    /// </summary>
    private Thread Listener = null;

    private bool Connecting() {
        try {
            if (((Client.Client.Poll(1000, SelectMode.SelectRead) && (Client.Client.Available == 0)) || !Client.Client.Connected))
                return false;
        } catch (Exception e) {
            PLogger.Log("网络错误");
            PLogger.Log(e.ToString());
            return false;
        }
        return true;
    }

    public PAbstractClient(TcpClient _client) {
        Client = _client;
        stream = Client.GetStream();
        #region 启动监听线程
        Listener = new Thread(() => {
            byte[] recvBuffer = new byte[PNetworkConfig.MaxBufferSizeNetwork];
            while (!StopConnection) {
                _Connected = Connecting();
                #region 检查发出的消息队列
                if (sendQueue.Count > 0 && _Connected) {
                    string StringToSend = string.Empty;
                    if (sendQueue.Count > 0) {
                        StringToSend += sendQueue.Dequeue();
                    }
                    byte[] buffer = Encoding.UTF8.GetBytes(StringToSend);
                    stream.Write(buffer, 0, buffer.Length);
                    stream.Flush();
                }
                #endregion
                #region 检查接收的消息
                if (stream.DataAvailable && _Connected) {
                    int bytesRead = stream.Read(recvBuffer, 0, PNetworkConfig.MaxBufferSizeNetwork);
                    if (bytesRead > 0) {
                        string message = Encoding.UTF8.GetString(recvBuffer).Substring(0, bytesRead);
                        #region 将消息拆成逻辑语义的信息
                        int startCutIndex = 0;
                        while (startCutIndex < message.Length) {
                            int firstStartFlagIndex = message.IndexOf( PNetworkConfig.MessageStartFlag, startCutIndex);
                            int firstEndFlagIndex = message.IndexOf( PNetworkConfig.MessageEndFlag, startCutIndex);
                            if (!ReceiveBufferString.Equals(string.Empty)) {
                                if (firstStartFlagIndex < 0 && firstEndFlagIndex < 0) {
                                    ReceiveBufferString += message.Substring(startCutIndex);
                                    startCutIndex = message.Length;
                                } else if (firstStartFlagIndex < 0 || firstEndFlagIndex < firstStartFlagIndex) {
                                    recvQueue.Enqueue(ReceiveBufferString + message.Substring(startCutIndex, firstEndFlagIndex - startCutIndex));
                                    startCutIndex = firstEndFlagIndex + 1;
                                    ReceiveBufferString = string.Empty;
                                } else {
                                    startCutIndex = firstStartFlagIndex;
                                    ReceiveBufferString = string.Empty;
                                }
                            } else {
                                if (firstStartFlagIndex < 0) {
                                    startCutIndex = message.Length;
                                } else if (firstEndFlagIndex < 0) {
                                    int lastStartFlagIndex = message.LastIndexOf( PNetworkConfig.MessageStartFlag);
                                    ReceiveBufferString = message.Substring(lastStartFlagIndex + 1);
                                    startCutIndex = message.Length;
                                } else if (firstEndFlagIndex < firstStartFlagIndex) {
                                    startCutIndex = firstStartFlagIndex;
                                } else {
                                    recvQueue.Enqueue(message.Substring(firstStartFlagIndex + 1, firstEndFlagIndex - firstStartFlagIndex - 1));
                                    startCutIndex = firstEndFlagIndex + 1;
                                }
                            }
                        }
                        #endregion
                    }
                }
                #endregion
                Thread.Sleep(PNetworkConfig.ListenerInterval);
            }
        });
        Listener.Start();
        #endregion
    }

    /// <summary>
    /// 发送一个字符串
    /// </summary>
    /// <param name="message">发送的字符串信息</param>
    protected virtual void Send(string message) {
        PLogger.Log("发送消息：" + message);
        sendQueue.Enqueue( PNetworkConfig.MessageStartFlag + message +  PNetworkConfig.MessageEndFlag);
    }

    /// <summary>
    /// 发送一条命令
    /// </summary>
    /// <param name="order">命令</param>
    public virtual void Send(POrder order) {
        Send(order.ToString());
    }

    /// <summary>
    /// 接收队列的长度
    /// </summary>
    public int ReceiveNumber {
        get {
            return recvQueue.Count;
        }
    }

    /// <summary>
    /// 从接收队列中获取一个逻辑语义字符串
    /// </summary>
    /// <returns>接收的字符串</returns>
    public string Receive() {
        if (ReceiveNumber <= 0)
            return null;
        return recvQueue.Dequeue();
    }

    /// <summary>
    /// 强行停止客户端的工作
    /// </summary>
    public void Stop() {
        StopConnection = true;
        stream.Close();
        Client.Close();
        if (Listener != null && Listener.IsAlive)
            Listener.Abort();
    }
}
