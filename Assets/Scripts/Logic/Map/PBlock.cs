using System.Collections.Generic;

public class PBlock : PObject {
    public int Index;
    public int X, Y;
    public int StartPointIndex;
    public int GetMoneyPassSolid;
    public int GetMoneyPassPercent;
    public int GetMoneyStopSolid;
    public int GetMoneyStopPercent;
    public int GetCardPass;
    public int GetCardStop;
    public int Price;
    public int HouseNumber;
    public PPlayer Lord;
    public List<PBlock> NextBlockList;
    public List<PBlock> PortalBlockList;
    public bool IsBusinessLand;
    public bool CanPurchase;
    public PBusinessType BusinessType = PBusinessType.NoType;
    public PBlock() {
        NextBlockList = new List<PBlock>();
        PortalBlockList = new List<PBlock>();
    }
    public PBlock NextBlock {
        get {
            if (NextBlockList.Count > 0) {
                return NextBlockList[0];
            } else {
                return this;
            }
        }
    }

    public int HousePrice {
        get {
            if (BusinessType.Equals(PBusinessType.Park)) {
                return 0;
            } else {
                return PMath.Percent(Price, 50);
            }
        }
    }
}