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
            int Cof = (Player.TeamIndex == _Player.TeamIndex ? -1 : 1);
            if (_Player.General is P_LiuJi) {
                Sum -= 1200 * Cof;
                Rate = 0;
            } else {
                Sum += (int)(PAiCardExpectation.FindLeastValuable(Game, _Player, _Player, false, true, false, true).Value * Rate) * Cof;
                Rate *= 5.0 / 6;
            }
        }, Player);
        return Sum;
    }

    public List<PPlayer> AIEmitTargets(PGame Game, PPlayer Player) {
        
        return new List<PPlayer>() { RoundExpect(Game, Player) >= 1000 && (Player.Defensor == null || !(Player.Defensor.Model is P_ChiiHsingPaao)) ? Player : null };
    }

    public override int AIInHandExpectation(PGame Game, PPlayer Player) {
        int Basic = Math.Max(1000, RoundExpect(Game, Player));
        return Math.Max(Basic, base.AIInHandExpectation(Game, Player));
    }

    public override int AIInAmbushExpectation(PGame Game, PPlayer Player) {
        return RoundExpect(Game, Player);
    }

    public override void AnnouceInvokeJudge(PGame Game, PPlayer Player, PCard Card) {
        base.AnnouceInvokeJudge(Game, Player, Card);
        int Result = Game.Judge(Player, new Func<int>(() => {
            if (Player.Area.EquipmentCardArea.CardNumber > 0) {
                return 6;
            } else {
                return 5;
            }
        })());
        if (Result != 6) {
            if (Player.Area.EquipmentCardArea.CardNumber > 0) {
                Game.ThrowCard(Player, Player, false, true);
            }
            if (Game.GetNextPlayer(Player).Area.AmbushCardArea.CardList.Exists((PCard _Card) => _Card.Model is P_HsienChing)) {
                Game.CardManager.MoveCard(Card, Player.Area.AmbushCardArea, Game.GetNextPlayer(Game.GetNextPlayer(Player)).Area.AmbushCardArea);
            } else {
                Game.CardManager.MoveCard(Card, Player.Area.AmbushCardArea, Game.GetNextPlayer(Player).Area.AmbushCardArea);
            }
            
        } else {
            Game.CardManager.MoveCard(Card, Player.Area.AmbushCardArea, Game.CardManager.ThrownCardHeap);
        }
    }

    public readonly static string CardName = "陷阱";

    public P_HsienChing():base(CardName) {
        Point = 6;
        Index = 42;
        BuildAmbush(AIEmitTargets, true, 10);
    }
}