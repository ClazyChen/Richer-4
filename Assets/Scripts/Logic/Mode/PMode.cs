using System.Collections.Generic;
using System;

public abstract class PMode : PObject {

    private const int UnknownParty = -1;
    public int Bonus = 0;

    public class Seat {
        public PPlayerType DefaultType;
        public bool Locked;
        public int Party;
    }

    public readonly Seat[] Seats;

    public int PlayerNumber {
        get {
            return Seats.Length;
        }
    }

    protected PMode(string _Name, int PlayerNumber, int[] PlayerParties) {
        Name = _Name;
        if (PlayerNumber > 8) {
            PlayerNumber = 8;
        }
        if (PlayerNumber < 2) {
            PlayerNumber = 2;
        }
        Seats = new Seat[PlayerNumber];
        for (int i = 0; i < PlayerNumber; ++ i) {
            Seats[i] = new Seat() {
                DefaultType = PPlayerType.Waiting,
                Locked = false,
                Party = PlayerParties[i]
            };
        }
    }

    virtual public void Open(PGame Game) {
        // 游戏开始时执行的操作
        // 默认没有操作
    }

    public static List<PMode> ListModes() {
        return ListSubTypeInstances<PMode>();
    }
}