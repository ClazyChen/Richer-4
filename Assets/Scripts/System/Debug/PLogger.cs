using System.IO;
using System.Text;

/// <summary>
/// PLogger类：
/// 表示输出到文件的日志生成器。
/// </summary>
public class PLogger {

    private class Config {
        public static string LogFileName = "debug.log";
    }

    private static bool IsValid = false;
    private static StreamWriter Writer;

    /// <summary>
    /// 启动Logger，清空原有日志文件
    /// </summary>
    public static void StartLogging(bool Valid = true, string Path = "") {
        if (Valid ) {
            IsValid = true;
            try {
                if (Writer != null) {
                    Writer.Close();
                }
                Writer = new StreamWriter(PPath.GetPath(Path.Equals(string.Empty) ? Config.LogFileName : Path), false, Encoding.UTF8) {
                    AutoFlush = true
                };
                Writer.WriteLine(PDebug.DebugHeader);
            } catch {
                IsValid = false;
            }
        } else {
            IsValid = false;
        }
        
    }

    /// <summary>
    /// 在日志文件中追加写入日志
    /// </summary>
    /// <param name="LogString">写入的日志信息</param>
    public static void Log(string LogString) {
        if (!IsValid) {
            return;
        }
        lock (Writer) {
            if (Writer != null) {
                Writer.WriteLine(LogString);
            }
        }
    }
}
