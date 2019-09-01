using System;

/// <summary>
/// 试图使用卡牌命令
/// </summary>
/// SR：当发出者为当前回合的角色且正在进行空闲时间点且空闲时，且卡牌的使用condition满足时，使用对应的卡牌
public class PClickOnBlockOrder : POrder {
    public PClickOnBlockOrder() : base("click_on_block",
        (string[] args, string IPAddress) => {
            int BlockIndex = Convert.ToInt32(args[1]);
            PGame Game = PNetworkManager.Game;
            PChooseBlockTag ChooseBlockTag = Game.TagManager.FindPeekTag<PChooseBlockTag>(PChooseBlockTag.TagName);
            if (ChooseBlockTag != null && ChooseBlockTag.Player.IPAddress.Equals(IPAddress)) {
                ChooseBlockTag.Block = Game.Map.FindBlock(BlockIndex);
            }
        },
        null) {
    }

    public PClickOnBlockOrder(string _BlockIndex) : this() {
        args = new string[] { _BlockIndex };
    }
}
