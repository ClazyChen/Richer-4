using System;
using System.Collections.Generic;
/// <summary>
/// 陷阱
/// </summary>
public class P_HsienChing : PAmbushCardModel {

    private int RoundExpect(PGame Game, PPlayer Player) {
        int Sum = 0;
        double Rate = 5.0 / 6;
        Game.Traverse((PPlayer _Player) => {
            Sum +=(int)( PAiCardExpectation.FindLeastValuable(Game, _Player, _Player, false, true, false, true).Value * Rate);
            Rate *= 5.0 / 6;
        }, Player);
        return Sum;
    }

    public List<PPlayer> AIEmitTargets(PGame Game, PPlayer Player) {
        
        return new List<PPlayer>() { RoundExpect(Game, Player) >= 1000 ? Player : null };
    }

    public override int AIInHandExpectation(PGame Game, PPlayer Player) {
        return Math.Max(1000, RoundExpect(Game, Player));
    }

    public override int AIInAmbushExpectation(PGame Game, PPlayer Player) {
        return RoundExpect(Game, Player);
    }

    public override void AnnouceInvokeJudge(PGame Game, PPlayer Player, PCard Card) {
        base.AnnouceInvokeJudge(Game, Player, Card);
        int Result = Game.Judge(Player);
        if (Result != 6) {
            if (Player.Area.EquipmentCardArea.CardNumber > 0) {
                Game.ThrowCard(Player, Player, false, true);
            }
            Game.CardManager.MoveCard(Card, Player.Area.AmbushCardArea, Game.GetNextPlayer(Player).Area.AmbushCardArea);
        } else {
            Game.CardManager.MoveCard(Card, Player.Area.AmbushCardArea, Game.CardManager.ThrownCardHeap);
        }
    }

    public readonly static string CardName = "陷阱";

    public P_HsienChing():base(CardName) {
        Point = 5;
        Index = 41;
        BuildAmbush(AIEmitTargets, false, 10);
    }
}