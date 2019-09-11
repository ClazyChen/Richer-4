using System;
using System.Collections.Generic;
/// <summary>
/// PCard：一个具体的牌
/// </summary>
public class PCard : PCardModel {
    public PCardModel Model; // 牌的原型

    public new List<PTrigger> MoveInHandTriggerList;
    public new List<PTrigger> MoveInEquipTriggerList;

    public PTrigger FindTrigger(PTime Time) {
        return MoveInHandTriggerList.Find((PTrigger Trigger) => Trigger.Time.Equals(Time));
    }

    public PTrigger FindEquipmentTrigger(PTime Time) {
        return MoveInEquipTriggerList.Find((PTrigger Trigger) => Trigger.Time.Equals(Time));
    }

    public PCard(PCardModel Prototype): base(Prototype.Name) {
        Model = Prototype;
        Type = Prototype.Type;
        Point = Prototype.Point;
        Index = Prototype.Index;
        MoveInHandTriggerList = new List<PTrigger>();
        MoveInEquipTriggerList = new List<PTrigger>();
    }
}