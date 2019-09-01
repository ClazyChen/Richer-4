using System;

/// <summary>
/// 刷新格子基本信息命令+格子+领主Index+房屋数量+商业用地类型
/// </summary>
/// CR：刷新格子并显示在MUI
public class PRefreshBlockBasicOrder : POrder {
    public PRefreshBlockBasicOrder() : base("refresh_block_basic",
        null,
        (string[] args) => {
            int BlockIndex = Convert.ToInt32(args[1]);
            int LordIndex = Convert.ToInt32(args[2]);
            int HouseNumber = Convert.ToInt32(args[3]);
            PBusinessType BusinessType = FindInstance<PBusinessType>(args[4]);
            PBlock Block = PNetworkManager.NetworkClient.GameStatus.Map.FindBlock(BlockIndex);
            PPlayer Lord = PNetworkManager.NetworkClient.GameStatus.FindPlayer(LordIndex);
            if (Block != null && BusinessType != null) {
                PPlayer OriginalLord = Block.Lord;
                Block.Lord = Lord;
                Block.HouseNumber = HouseNumber;
                Block.BusinessType = BusinessType;
                PAnimation.AddAnimation("刷新格子基本信息", () => {
                    PUIManager.GetUI<PMapUI>().Scene.BlockGroup.GroupUIList[BlockIndex].InitializeBlock(Block);
                    if (OriginalLord != null) {
                        if (Block.IsBusinessLand) {
                            OriginalLord.BusinessLandNumber--;
                        } else {
                            OriginalLord.NormalLandNumber--;
                        }
                        PUIManager.GetUI<PMapUI>().PlayerInformationGroup.Update(OriginalLord.Index);
                    }
                    if (Lord != null) {
                        if (Block.IsBusinessLand) {
                            Lord.BusinessLandNumber++;
                        } else {
                            Lord.NormalLandNumber++;
                        }
                        PUIManager.GetUI<PMapUI>().PlayerInformationGroup.Update(Lord.Index);
                    }
                });
            }
        }) {
    }

    public PRefreshBlockBasicOrder(string _BlockIndex, string _LordIndex, string _HouseNumber, string _BusinessType) : this() {
        args = new string[] { _BlockIndex, _LordIndex, _HouseNumber, _BusinessType };
    }

    public PRefreshBlockBasicOrder(PBlock Block) : this(Block.Index.ToString(), Block.Lord == null ? "-1" : Block.Lord.Index.ToString(), Block.HouseNumber.ToString(), Block.BusinessType.Name) {

    }
}
