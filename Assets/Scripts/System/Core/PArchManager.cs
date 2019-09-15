using System.Text;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// PArchManager：用于管理成就列表
/// </summary>
public class PArchManager {
    public List<string> ArchList;

    public PArchManager() {
        ArchList = new List<string>();
    }
    public void Read() {
        string dataDirectory = PPath.GetPath("Data\\Arcs\\arc.ric");
        StreamReader ArcFileReader = new StreamReader(dataDirectory, Encoding.UTF8);
        string Line = string.Empty;
        while ((Line = ArcFileReader.ReadLine()) != null) {
            if (Line.Length > 0) {
                ArchList.Add(Line);
            } else {
                break;
            }
        }
        ArcFileReader.Close();
    }
    public void Write() {
        lock (ArchList) {
            string dataDirectory = PPath.GetPath("Data\\Arcs\\arc.ric");
            StreamWriter ArcWriter = new StreamWriter(dataDirectory, false, Encoding.UTF8);
            ArchList.ForEach((string s) => ArcWriter.WriteLine(s));
            ArcWriter.Flush();
            ArcWriter.Close();
        }
    }
    public bool AnnounceArch(PArchInfo ArchInfo) {
        if (ArchInfo == null) {
            return false;
        }
        bool Refresh = false;
        lock (ArchList) {
            if (!ArchList.Exists((string x) => ArchInfo.Name.Equals(x))) {
                ArchList.Add(ArchInfo.Name);
                Refresh = true;
            }
        }
        if (Refresh) {
            PThread.Async(() => Write());
        }
        return Refresh;
    }
}
