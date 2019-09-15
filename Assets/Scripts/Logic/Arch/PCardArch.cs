public class PCardArch : PArch { 
    public PCardArch() : base("卡牌类成就") {
        TriggerList.Add(new PTrigger("吃掉电脑屏幕") {
            IsLocked = true,
            Time = PTime.EndGameTime,
            Effect = (PGame Game) => {
                Game.GetWinner().ForEach((PPlayer Player) => {
                    if (Player.Area.HandCardArea.CardNumber >= 17) {
                        Announce(Game, Player, "吃掉电脑屏幕");
                    }
                });
            }
        });
        TriggerList.Add(new PTrigger("海天一色") {
            IsLocked = true,
            Time = PTime.DieTime,
            Effect = (PGame Game) => {
                PDyingTag DyingTag = Game.TagManager.FindPeekTag<PDyingTag>(PDyingTag.TagName);
                PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                if (InjureTag != null && DyingTag != null && DyingTag.Player.Equals(InjureTag.ToPlayer) && InjureTag.InjureSource is PCard && ((PCard)InjureTag.InjureSource).Model is P_ManTiienKuoHai) {
                    PUseCardTag UseCardTag = Game.TagManager.FindPeekTag<PUseCardTag>(PUseCardTag.TagName);
                    if (UseCardTag != null && UseCardTag.User != null && UseCardTag.Card.Equals(InjureTag.InjureSource)) {
                        Announce(Game, UseCardTag.User, "海天一色");
                    }
                }
            }
        });
    }
}