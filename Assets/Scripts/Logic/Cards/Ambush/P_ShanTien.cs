using System;
using System.Collections.Generic;
/// <summary>
/// 闪电
/// </summary>
public class P_ShanTien : PAmbushCardModel {

    private int RoundExpect(PGame Game, PPlayer Player) {
        int Sum = 0;
        double Rate = 10.0 / 36;
        Game.Traverse((PPlayer _Player) => {
            int Base = _Player.Money <= 60000 ? 30000 : 1000;
            Base *= _Player.TeamIndex == Player.TeamIndex ? -1 : 1;
            if (_Player.Defensor == null || !(_Player.Defensor.Model is P_YinYangChing)) {
                Sum += (int)(Base * Rate);
            }
            Rate *= 26.0 / 36;
        }, Player);
        return Sum;
    }

    public List<PPlayer> AIEmitTargets(PGame Game, PPlayer Player) {
        
        return new List<PPlayer>() { RoundExpect(Game, Player) >= 500 && (Player.Defensor == null || !(Player.Defensor.Model is P_ChiiHsingPaao)) ? Player : null };
    }

    public override int AIInHandExpectation(PGame Game, PPlayer Player) {
        return RoundExpect(Game, Player);
    }

    public override int AIInAmbushExpectation(PGame Game, PPlayer Player) {
        return RoundExpect(Game, Player);
    }

    public override void AnnouceInvokeJudge(PGame Game, PPlayer Player, PCard Card) {
        base.AnnouceInvokeJudge(Game, Player, Card);
        int Result1 = Game.Judge(Player, 6);
        int Result2 = Game.Judge(Player, 6);
        if (Result1+Result2 <= 5) {
            Game.Injure(null, Player, 6000, Card);
            Game.CardManager.MoveCard(Card, Player.Area.AmbushCardArea, Game.CardManager.ThrownCardHeap);
        } else {
            if (Game.GetNextPlayer(Player).Area.AmbushCardArea.CardList.Exists((PCard _Card) => _Card.Model is P_ShanTien)) {
                Game.CardManager.MoveCard(Card, Player.Area.AmbushCardArea, Game.GetNextPlayer(Game.GetNextPlayer(Player)).Area.AmbushCardArea);
            } else {
                Game.CardManager.MoveCard(Card, Player.Area.AmbushCardArea, Game.GetNextPlayer(Player).Area.AmbushCardArea);
            }
        }
    }

    public readonly static string CardName = "闪电";

    public P_ShanTien():base(CardName) {
        Point = 4;
        Index = 40;
        BuildAmbush(AIEmitTargets, true, 10);
    }
}