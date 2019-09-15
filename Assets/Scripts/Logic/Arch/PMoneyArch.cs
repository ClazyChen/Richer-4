public class PMoneyArch : PArch { 
    public PMoneyArch() : base("金钱类成就") {
        TriggerList.Add(new PTrigger("开始游戏") {
            IsLocked = true,
            Time = PTime.StartGameTime,
            Effect = (PGame Game) => {
                Game.PlayerList.ForEach((PPlayer Player) => {
                    Announce(Game, Player, "开始游戏");
                });
            }
        });
        TriggerList.Add(new PTrigger("炸弹") {
            IsLocked = true,
            Time = PTime.Injure.EmitInjure,
            Condition = (PGame Game) => {
                PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                return InjureTag.Injure >= 10000 && InjureTag.FromPlayer != null;
            },
            Effect = (PGame Game) => {
                PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                Announce(Game, InjureTag.FromPlayer, "炸弹");
            }
        });
        TriggerList.Add(new PTrigger("踩到地雷") {
            IsLocked = true,
            Time = PTime.Injure.EmitInjure,
            Condition = (PGame Game) => {
                PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                return InjureTag.Injure >= 10000 && InjureTag.ToPlayer != null;
            },
            Effect = (PGame Game) => {
                PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                Announce(Game, InjureTag.ToPlayer, "踩到地雷");
            }
        });
        TriggerList.Add(new PTrigger("疯狂试探") {
            IsLocked = true,
            Time = PTime.EndGameTime,
            Effect = (PGame Game) => {
                Game.GetWinner().ForEach((PPlayer Player) => {
                    if (Player.IsAlive && Player.Money > 0 && Player.Money < 3000) {
                        Announce(Game, Player, "疯狂试探");
                    }
                });
            }
        });
        TriggerList.Add(new PTrigger("叫我爸爸") {
            IsLocked = true,
            Time = PPeriod.StartTurn.During,
            Condition = (PGame Game) => {
                return Game.NowPlayer.Money >= 88000;
            },
            Effect = (PGame Game) => {
                Announce(Game, Game.NowPlayer, "叫我爸爸");
            }
        });

    }
}