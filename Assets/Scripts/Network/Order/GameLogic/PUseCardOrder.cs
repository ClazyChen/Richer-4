using System;

/// <summary>
/// 试图使用卡牌命令
/// </summary>
/// SR：当发出者为当前回合的角色且正在进行空闲时间点且空闲时，且卡牌的使用condition满足时，使用对应的卡牌
public class PUseCardOrder : POrder {
    public PUseCardOrder() : base("use_card",
        (string[] args, string IPAddress) => {
            int CardIndex = Convert.ToInt32(args[1]);
            PGame Game = PNetworkManager.Game;
            PChooseCardTag ChooseCardTag = Game.TagManager.FindPeekTag<PChooseCardTag>(PChooseCardTag.TagName);
            if (ChooseCardTag != null && ChooseCardTag.Player.IPAddress.Equals(IPAddress)) {
                ChooseCardTag.Card = ChooseCardTag.Player.Area.GetCard(CardIndex);
            } else if (Game.Logic.WaitingForEndFreeTime() && Game.NowPlayer.IPAddress.Equals(IPAddress) && Game.TagManager.ExistTag(PTag.FreeTimeOperationTag.Name) && Game.NowPlayer.IsAlive) {
                PCard Card = Game.NowPlayer.Area.HandCardArea.GetCard(CardIndex);
                if (Card != null) {
                    PTrigger Trigger = Card.FindTrigger(PPeriod.FirstFreeTime.During);
                    if (Trigger != null) {
                        if (Trigger.Condition(Game)) {
                            PThread.Async(() => {
                                Game.Logic.StartSettle(new PSettle("主动触发" + Trigger.Name, Trigger.Effect));
                            });
                        }
                    }
                }
            }
        },
        null) {
    }

    public PUseCardOrder(string _CardIndex) : this() {
        args = new string[] { _CardIndex };
    }
}
