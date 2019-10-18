public class PModeArch : PArch { 
    public PModeArch() : base("模式类成就") {
        TriggerList.Add(new PTrigger("独孤求败") {
            IsLocked = true,
            Time = PTime.EndGameTime,
            Effect = (PGame Game) => {
                Game.GetWinner().ForEach((PPlayer Player) => {
                    if (Game.GameMode is PMode8p) {
                        Announce(Game, Player, "独孤求败");
                    }
                });
            }
        });
        string Juecfs = "绝处逢生";
        TriggerList.Add(new PTrigger("绝处逢生[达到条件]") {
            IsLocked = true,
            Time = PTime.AfterDieTime,
            Effect = (PGame Game) => {
                if (Game.GameMode is PMode4v4) {
                    Game.AlivePlayers().ForEach((PPlayer Player) => {
                        if (Game.Teammates(Player).Count == 1 && Game.Enemies(Player).Count == 4) {
                            Player.Tags.CreateTag(new PTag(Juecfs) {
                                Visible = false
                            });
                        }
                    });
                }
            }
        });
        TriggerList.Add(new PTrigger("绝处逢生") {
            IsLocked = true,
            Time = PTime.EndGameTime,
            Effect = (PGame Game) => {
                Game.GetWinner().ForEach((PPlayer Player) => {
                    if (Player.Tags.ExistTag(Juecfs)) {
                        Announce(Game, Player, "绝处逢生");
                    }
                });
            }
        });
    }
}