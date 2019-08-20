using System;

public class PDebug {
    /// <summary>
    /// 返回带当前时间的日志首部
    /// </summary>
    public static string DebugHeader {
        get {
            return "日志 @当前时间：" + DateTime.Now.ToLocalTime().ToString();
        }
    }
}