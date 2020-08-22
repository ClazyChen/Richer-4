using System.Collections.Generic;

public class PMode3v2 : PMode {
    public PMode3v2() :
        base("约会模式", 5, new int[] { 2, 2, 1, 1, 1 }) {
        Bonus = 20;
        Seats[0].DefaultType = PPlayerType.AI;
        Seats[0].Name = "诱宵美九";
        Seats[1].DefaultType = PPlayerType.AI;
        Seats[1].Name = "破军歌姬";
        Installer.Add(new PTrigger("破军歌姬启动") {
            IsLocked = true,
            Time = PTime.InstallModeTime,
            Effect = (PGame Game) => {
                Game.PlayerList[1].General = new P_Gabriel();
                Game.PlayerList[0].General = new P_IzayoiMiku();
            }
        });
    }
}