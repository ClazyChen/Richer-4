using System.Collections.Generic;
using System;
using System.Linq;

public abstract class PMode : PObject {

    private const int UnknownParty = -1;

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
        int PartyNumber = 1;
        List<int> PartyMarks = new List<int> { UnknownParty };
        for (int i = 0; i < PlayerNumber; ++ i) {
            int PartyTemp = PlayerNumber;
            if (i < PlayerParties.Length) {
                if (PartyMarks.Exists((int x) => x == PlayerParties[i])) {
                    PartyTemp = PartyMarks.FindIndex((int x) => x == PlayerParties[i]);
                } else {
                    PartyMarks.Add(PlayerParties[i]);
                    PartyNumber++;
                }
            } else {
                PartyMarks.Add(UnknownParty);
                PartyNumber++;
            }
            Seats[i] = new Seat() {
                DefaultType = PPlayerType.Waiting,
                Locked = false,
                Party = PartyTemp
            };
        }
    }

    virtual public void Open(PGame Game) {
        // 游戏开始时执行的操作
        // 默认没有操作
    }

    public static List<PMode> ListModes() {
        List<PMode> TempList = new List<PMode>();
        foreach (Type SubType in ListSubTypes<PMode>()) {
            try {
                PMode ModeInstance = (PMode)Activator.CreateInstance(SubType);
                TempList.Add(ModeInstance);
            } catch {
                continue;
            }
        }
        return TempList;
    }
}