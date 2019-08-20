using System.Net.Sockets;
using System.Threading;

/// <summary>
/// PNetworkManager类：
/// 管理网络模块的顶层类。
/// </summary>
/// 这个类设置了HostType，表示服务器模式还是客户端模式
/// 服务器模式下，主机建立一个服务器和一个客户端
/// 客户端模式下，主机只建立一个客户端
public class PNetworkManager {
    public static PHostType CurrentHostType = PHostType.NoType;
    public static string CurrentNickname = string.Empty;

    private static PClient _NetworkClient = null;
    private static PServer _NetworkServer = null;

    /// <summary>
    /// 网络客户端，服务器和客户端模式均可以访问
    /// </summary>
    public static PClient NetworkClient {
        get {
            if (!CurrentHostType.Equals(PHostType.NoType)) {
                return _NetworkClient;
            } else {
                return null;
            }
        }
    }

    /// <summary>
    /// 网络服务器，只有服务器模式可访问
    /// </summary>
    public static PServer NetworkServer {
        get {
            if (CurrentHostType.Equals(PHostType.Server)) {
                return _NetworkServer;
            } else {
                return null;
            }
        }
    }

    /// <summary>
    /// 游戏对象，只有服务器模式可访问
    /// </summary>
    public static PGame Game {
        get {
            if (CurrentHostType.Equals(PHostType.Server)) {
                return _NetworkServer.Game;
            } else {
                return null;
            }
        }
    }

    /// <summary>
    /// 创建一个客户端，如果之前为服务器则将其关闭
    /// </summary>
    /// <param name="ServerIP">服务器的IP地址</param>
    /// <param name="Nickname">客户端的昵称</param>
    /// <returns>是否创建成功（IP地址正确、昵称长度不大于8且网络正常）</returns>
    public static bool CreateClient(string ServerIP, string Nickname) {
        if (Nickname.Length > PNetworkConfig.MaxNicknameLength) {
            return false;
        }
        AbortServer();
        try {
            CurrentHostType = PHostType.Client;
            _NetworkClient = new PClient(new TcpClient(ServerIP, PNetworkConfig.ServerPort));
        } catch {
            return false;
        }
        CurrentNickname = Nickname;
        return true;
    }

    /// <summary>
    /// 建立服务器，同时建立和它连接的客户端
    /// </summary>
    /// <param name="GameMap">游戏地图</param>
    /// <param name="GameMode">游戏模式</param>
    public static void CreateServer(PMap GameMap, PMode GameMode) {
        AbortClient();
        CurrentHostType = PHostType.Server;
        PThread.Async(() => {
            _NetworkServer = new PServer() {
                maxConnectionNumber = GameMode.PlayerNumber,
                Game = new PGame(GameMap, GameMode)
            };
            Thread.Sleep(PNetworkConfig.ListenerInterval);
            try {
                _NetworkClient = new PClient(new TcpClient(PNetworkConfig.IP.ToString(), PNetworkConfig.ServerPort));
            } catch {
                PLogger.Log("服务器客户端创建错误");
            }
            CurrentNickname = PNetworkConfig.DefaultNickname;
        });
    }

    /// <summary>
    /// 关闭服务器（及其关联的客户端），如果不为服务器模式则不操作
    /// </summary>
    public static void AbortServer() {
        if (CurrentHostType.Equals(PHostType.Server)) {
            if (NetworkClient != null) {
                NetworkClient.Stop();
                _NetworkClient = null;
            }
            if (NetworkServer != null) {
                NetworkServer.Close();
                _NetworkServer = null;
            }
            CurrentHostType = PHostType.NoType;
        }
    }

    /// <summary>
    /// 关闭客户端，若为客户端模式，取消模式
    /// </summary>
    public static void AbortClient() {
        if (NetworkClient != null) {
            NetworkClient.Stop();
            _NetworkClient = null;
        }
        if (CurrentHostType.Equals(PHostType.Client)) {
            CurrentHostType = PHostType.NoType;
        }
    }
}
