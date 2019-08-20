using System.IO;

/// <summary>
/// 用于控制读取文件的路径
/// </summary>
public class PPath {
    /// <summary>
    /// 返回实际的相对路径，若路径不存在创建之
    /// </summary>
    /// <param name="RelativePath">不考虑Build模式的相对路径"AA\\BB\\CC"或"AA\\BB.xxx"</param>
    /// <returns>考虑Build模式的相对路径</returns>
    public static string GetPath(string RelativePath) {
        #region 创建路径上的目录和文件
        string CurrentPath = Directory.GetCurrentDirectory();
        bool IsBuilding = CurrentPath.Contains("Build");
        string[] PathComponents = RelativePath.Split('\\');
        string TempPath = IsBuilding ? "..\\" : "";
        for (int i = 0; i < PathComponents.Length; ++i) {
            TempPath += PathComponents[i];
            if (PathComponents[i].Contains(".")) {
                if (!File.Exists(TempPath)) {
                    File.Create(TempPath);
                }
            } else if (!Directory.Exists(TempPath)) {
                Directory.CreateDirectory(TempPath);
            }
            TempPath += "\\";
        }
        #endregion
        #region 根据是否为Build模式返回路径
        if (IsBuilding) {
            return "..\\" + RelativePath;
        } else {
            return RelativePath;
        }
        #endregion
    }
}
