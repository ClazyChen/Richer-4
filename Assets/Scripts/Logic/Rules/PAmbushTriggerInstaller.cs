public class PAmbushTriggerInstaller : PSystemTriggerInstaller { 
    public PAmbushTriggerInstaller() : base("触发伏兵") {
        TriggerList.Add(new PTrigger("触发伏兵") {
            IsLocked = true,
            Time = PPeriod.AmbushStage.During,
            Condition = (PGame Game) => {
                return Game.NowPlayer.Area.AmbushCardArea.CardNumber > 0;
            },
            Effect = (PGame Game) => {
                for (int i = Game.NowPlayer.Area.AmbushCardArea.CardNumber - 1; i >= 0; -- i) {
                    PCard AmbushCard = Game.NowPlayer.Area.AmbushCardArea.CardList[i];
                    if (AmbushCard != null) {
                        ((PAmbushCardModel)AmbushCard.Model).AnnouceInvokeJudge(Game, Game.NowPlayer, AmbushCard);
                    }
                }
            }
        });
    }
}