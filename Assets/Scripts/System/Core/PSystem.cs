using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class PSystem: MonoBehaviour {
    public static List<PMap> MapList { get; private set; }
    public static PRoom CurrentRoom = null;
    public static PMode CurrentMode = null;
    public static int PlayerIndex = 0;

    void Start() {
        PLogger.StartLogging(true);
        #region ≥ı ºªØµÿÕºø‚
        string MapDirectory = PPath.GetPath("Data\\Maps");
        string[] MapFileNames = Directory.GetFiles(MapDirectory, "*.xml");
        MapList = new List<PMap>();
        foreach (string MapFileName in MapFileNames) {
            MapList.Add(new PMap(MapFileName));
        }
        #endregion
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            if (PUIManager.IsCurrentUI<PMapUI>()) {
                PUIManager.GetUI<PMapUI>().Space();
            }
        }
    }
}