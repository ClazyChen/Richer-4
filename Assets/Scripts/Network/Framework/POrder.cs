using System;
using System.Linq;

public class POrder : PObject {
    /// <summary>
    /// 服务器收到该命令的响应函数
    /// </summary>
    public readonly Action<string[], string> ServerResponseAction;
    /// <summary>
    /// 客户顿收到该命令的响应函数
    /// </summary>
    public readonly Action<string[]> ClientResponseAction;
    public string[] args = { };

    override public string ToString() {
        return Name + " " + string.Join(" ", args);
    }

    protected POrder() { }
    protected POrder(string _Name, Action<string[], string> _ServerResponseAction = null, Action<string[]> _ClientResponseAction = null) {
        Name = _Name;
        ServerResponseAction = (string[] args, string IPAddress) => { };
        if (_ServerResponseAction != null) {
            ServerResponseAction = _ServerResponseAction;
        }
        ClientResponseAction = (string[] args) => { };
        if (_ClientResponseAction != null) {
            ClientResponseAction = _ClientResponseAction;
        }
    }

    /// <summary>
    /// 根据命令名返回命令
    /// </summary>
    /// <param name="OrderName">命令名</param>
    /// <returns>所查询的命令，如果命令名不存在返回null</returns>
    public static POrder GetOrder(string OrderName) {
        foreach (Type SubType in typeof(POrder).Assembly.GetTypes().Where((Type TempType) => TempType.IsSubclassOf(typeof(POrder)))) {
            try {
                POrder OrderInstance = (POrder)Activator.CreateInstance(SubType);
                if (OrderInstance.Name.Equals(OrderName)) {
                    return OrderInstance;
                }
            } catch {
                continue;
            }
        }
        return null;
    }
}
