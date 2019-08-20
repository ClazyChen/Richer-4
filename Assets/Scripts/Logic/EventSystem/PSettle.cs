using System;

public class PSettle : PObject{
    public readonly Action SettleAction;

    public PSettle(string _Name, Action _SettleAction) {
        Name = _Name;
        SettleAction = _SettleAction;
    }
}