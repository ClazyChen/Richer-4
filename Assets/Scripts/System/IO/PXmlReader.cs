using System;
using System.Xml;

public class PXmlReader {
    private XmlDocument Document;
    private string Anchor = string.Empty;

    private string LastReadAnchor = string.Empty;
    private XmlNodeList NodeList = null;
    private XmlNode CurrentNode = null;

    public PXmlReader(string XmlFileName) {
        PLogger.Log("读入地图：" + XmlFileName);
        Document = new XmlDocument();
        Document.Load(XmlFileName);
    }

    /// <summary>
    /// 打开节点（列），即锚进入到该节点（列），不会判断是否存在
    /// </summary>
    /// <param name="NodeName">要打开的节点（列）的名字</param>
    public void OpenNode(string NodeName) {
        Anchor += "/" + NodeName;
    }

    /// <summary>
    /// 关闭节点（列）
    /// </summary>
    public void CloseNode() {
        int LastPathIndex = Anchor.LastIndexOf('/');
        Anchor = Anchor.Substring(0, LastPathIndex);
    }

    /// <summary>
    /// 对当前节点进行处理
    /// </summary>
    /// <param name="Processor">处理节点的函数，在函数内使用GetInt/GetString可以获得当前节点的属性值</param>
    /// <returns>返回是否成功（即节点存在）</returns>
    public bool Process(Action<XmlNode> Processor) {
        if (!Anchor.Equals(LastReadAnchor)) {
            LastReadAnchor = Anchor;
            NodeList = Document.SelectNodes(Anchor);
        }
        if (NodeList != null) {
            foreach (XmlNode Node in NodeList) {
                CurrentNode = Node;
                Processor(Node);
            }
            CurrentNode = null;
            return true;
        } else {
            return false;
        }
    }

    /// <summary>
    /// 需要在Process的Processor里使用，获取当前节点的属性值，返回int
    /// </summary>
    /// <param name="AttributeName">属性名</param>
    /// <returns>当前节点对应属性的整数值，非法情形及缺省返回0</returns>
    public int GetInt(string AttributeName) {
        try {
            return Convert.ToInt32(CurrentNode.Attributes[AttributeName].Value);
        } catch (Exception) {
            return 0;
        }
    }

    /// <summary>
    /// 需要在Process的Processor里使用，获取当前节点的属性值，返回string
    /// </summary>
    /// <param name="AttributeName">属性名</param>
    /// <returns>当前节点对应属性，缺省为空串</returns>
    public string GetString(string AttributeName) {
        try {
            return CurrentNode.Attributes[AttributeName].Value;
        } catch (Exception) {
            return string.Empty;
        }
    }
}