using System;

public class PSettle : PObject{
    public readonly Action<PGame> SettleAction;

    public PSettle(string _Name, Action<PGame> _SettleAction) {
        Name = _Name;
        SettleAction = _SettleAction;
    }
}