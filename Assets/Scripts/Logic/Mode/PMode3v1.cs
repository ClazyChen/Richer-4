using System.Collections.Generic;

public class PMode3v1 : PMode {
    public PMode3v1() :
        base("神兽模式", 4, new int[] { 1, 1, 1, 2 }) {
        Bonus = 5;
        Seats[3].DefaultType = PPlayerType.AI;
        Installer.Add(new PTrigger("神兽提前获取土地和房屋") {
            IsLocked = true,
            Time = PTime.InstallModeTime,
            Effect = (PGame Game) => {
                // 随机获得8个土地和2个商业用地
                List<PBlock> LandList = Game.Map.BlockList.FindAll((PBlock _Block) => _Block.CanPurchase && !_Block.IsBusinessLand);
                List<PBlock> BusinessList = Game.Map.BlockList.FindAll((PBlock _Block) => _Block.CanPurchase && _Block.IsBusinessLand);
                PMath.Wash(LandList);
                PMath.Wash(BusinessList);
                int TotalLandCount = (LandList.Count + BusinessList.Count) / 3;
                int BusinessCount = BusinessList.Count / 3;
                int LandCount = TotalLandCount - BusinessCount;
                PLogger.Log("  获得" + LandCount.ToString() + "领地;" + BusinessCount.ToString() + "商业用地");
                List<PBlock> GotList = LandList.GetRange(0, LandCount);
                GotList.AddRange(BusinessList.GetRange(0, BusinessCount));
                GotList.ForEach((PBlock Block) => {
                    Block.Lord = Game.PlayerList[PlayerNumber - 1];
                    Block.HouseNumber = 1;
                    if (Block.IsBusinessLand) {
                        Block.BusinessType = PBusinessType.ShoppingCenter;
                    }
                    PNetworkManager.NetworkServer.TellClients(new PRefreshBlockBasicOrder(Block));
                });
            }
        });
        Installer.Add(new PTrigger("生成神兽") {
            IsLocked = true,
            Time = PTime.InstallModeTime,
            Effect = (PGame Game) => {
                List<PGeneral> PossibleBoss = new List<PGeneral>() {
                    new P_QingLong(),
                    new P_BaiHu(),
                    new P_ZhuQue(),
                    new P_XuanWu()
                };
                PMath.Wash(PossibleBoss);
                Game.PlayerList[PlayerNumber - 1].General = PossibleBoss[0];
            }
        });
    }
}