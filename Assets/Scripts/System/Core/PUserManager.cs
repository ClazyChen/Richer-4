using System;
using System.Text;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// PUserManager：用于管理用户信息
/// </summary>
public class PUserManager {
    public string Nickname = "聪明的傻子";
    public int Money = 20;
    public int ArchPoint = -1;
    public int Lucky = 0;
    public int ChooseGeneral = 0;

    public List<string> GeneralList;
    public List<string> RecordList;

    public PUserManager() {
        GeneralList = new List<string>();
        RecordList = new List<string>();
    }
    public void Read() {
        string dataDirectory = PPath.GetPath("Data\\User\\user.ric");
        StreamReader ArcFileReader = new StreamReader(dataDirectory, Encoding.UTF8);
        string Line = string.Empty;
        while ((Line = ArcFileReader.ReadLine()) != null) {
            if (Line.Length > 0) {
                string[] LineData = Line.Split(' ');
                if (LineData.Length > 1) {
                    string Key = LineData[0];
                    if (Key.Equals("Nickname")) {
                        Nickname = LineData[1];
                    } else if (Key.Equals("Money")) {
                        Money = Convert.ToInt32(LineData[1]);
                    } else if (Key.Equals("ArchPoint")) {
                        ArchPoint = Convert.ToInt32(LineData[1]);
                    } else if (Key.Equals("ChooseGeneral")) {
                        ChooseGeneral = Convert.ToInt32(LineData[1]);
                    } else if (Key.Equals("Lucky")) {
                        Lucky = Convert.ToInt32(LineData[1]);
                    } else if (Key.Equals("General")) {
                        GeneralList.Add(LineData[1]);
                    } else if (Key.Equals("Record")) {
                        /*
                         * 记录格式：
                         * Record <使用的武将> Win/Lose <模式> <从1号位起的每名武将>
                         */ 
                        RecordList.Add(LineData[1]);
                    }
                }
            } else {
                break;
            }
        }
        ArcFileReader.Close();
        if (ArchPoint < 0) {
            // 初始化文件
            ArchPoint = (int)PMath.Sum(PSystem.ArchManager.ArchList.ConvertAll((string ArchName) => (double)PObject.ListInstance<PArchInfo>().Find((PArchInfo ArchInfo) => ArchInfo.Name.Equals(ArchName)).ArchPoint));
            GeneralList = new List<string>() {
                "廉颇", "潘岳", "杨玉环", "陈圆圆",
                "王诩", "赵云", "时迁", "张三丰"
            };
            Write();
        }
    }
    public void Write() {
        string dataDirectory = PPath.GetPath("Data\\User\\user.ric");
        lock (this) {
            StreamWriter ArcWriter = new StreamWriter(dataDirectory, false, Encoding.UTF8);
            ArcWriter.WriteLine("Nickname " + Nickname);
            ArcWriter.WriteLine("Money " + Money);
            ArcWriter.WriteLine("ArchPoint " + ArchPoint);
            ArcWriter.WriteLine("ChooseGeneral " + ChooseGeneral);
            ArcWriter.WriteLine("Lucky " + Lucky);
            GeneralList.ForEach((string GeneralName) => {
                ArcWriter.WriteLine("General " + GeneralName);
            });
            RecordList.ForEach((string Record) => {
                ArcWriter.WriteLine("Record " + Record);
            });
            ArcWriter.Flush();
            ArcWriter.Close();
        }
    }
}
