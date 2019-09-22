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
        TriggerList.Add(new PTrigger("口蜜腹剑") {
            IsLocked = true,
            Time = PTime.DieTime,
            Effect = (PGame Game) => {
                PDyingTag DyingTag = Game.TagManager.FindPeekTag<PDyingTag>(PDyingTag.TagName);
                PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                if (InjureTag != null && DyingTag != null && DyingTag.Player.Equals(InjureTag.ToPlayer) && InjureTag.InjureSource is PCard && ((PCard)InjureTag.InjureSource).Model is P_HsiaoLiTsaangTao) {
                    PUseCardTag UseCardTag = Game.TagManager.FindPeekTag<PUseCardTag>(PUseCardTag.TagName);
                    if (UseCardTag != null && UseCardTag.User != null && UseCardTag.Card.Equals(InjureTag.InjureSource)) {
                        Announce(Game, UseCardTag.User, "口蜜腹剑");
                    }
                }
            }
        });

        string Chuqby = "出其不意";
        TriggerList.Add(new PTrigger("出其不意[初始化]") {
            IsLocked = true,
            Time = PPeriod.StartTurn.During,
            Effect = (PGame Game) => {
                Game.NowPlayer.Tags.CreateTag(new PUsedTag(Chuqby, 1));
            }
        });
        TriggerList.Add(new PTrigger("出其不意[使用暗度陈仓]") {
            IsLocked = true,
            Time = PTime.Card.EndSettleTime,
            Condition = (PGame Game) => {
                PUseCardTag UseCardTag = Game.TagManager.FindPeekTag<PUseCardTag>(PUseCardTag.TagName);
                return UseCardTag.TargetList.Contains(Game.NowPlayer) && UseCardTag.Card.Model is P_AnTuCheevnTsaang;
            },
            Effect = (PGame Game) => {
                Game.NowPlayer.Tags.FindPeekTag<PUsedTag>(PUsedTag.TagNamePrefix + Chuqby).Count = 1;
            }
        });
        TriggerList.Add(new PTrigger("出其不意") {
            IsLocked = true,
            Time = PTime.PurchaseLandTime,
            Condition = (PGame Game) => {
                PPurchaseLandTag PurchaseLandTag = Game.TagManager.FindPeekTag<PPurchaseLandTag>(PPurchaseLandTag.TagName);
                return PurchaseLandTag.Player.Equals(Game.NowPlayer) && PurchaseLandTag.Block.IsBusinessLand && Game.NowPlayer.Tags.FindPeekTag<PUsedTag>(PUsedTag.TagNamePrefix + Chuqby).Count > 0;
            },
            Effect = (PGame Game) => {
                Announce(Game, Game.NowPlayer, "出其不意");
            }
        });
    }
}