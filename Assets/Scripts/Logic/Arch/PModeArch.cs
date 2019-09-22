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
    }
}