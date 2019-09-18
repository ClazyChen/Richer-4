using System;
using System.Collections.Generic;
/// <summary>
/// 兵粮寸断
/// </summary>
public class P_PingLiangTsuunTuan : PAmbushCardModel {

    public List<PPlayer> AIEmitTargets(PGame Game, PPlayer Player) {
        PPlayer Target = PMath.Max(Game.Enemies(Player).FindAll((PPlayer _Player) => _Player.HasHouse && (_Player.Defensor == null || !(_Player.Defensor.Model is P_ChiiHsingPaao)) && !_Player.Area.AmbushCardArea.CardList.Exists((PCard _Card) => _Card.Model.Name.Equals(CardName))), (PPlayer _Player) => PAiMapAnalyzer.MinValueHouse(Game, _Player).Value + PMath.RandInt(-10, 10)).Key;
        return new List<PPlayer>() { Target};
    }

    public override int AIInHandExpectation(PGame Game, PPlayer Player) {
        return 1000;
    }

    public override int AIInAmbushExpectation(PGame Game, PPlayer Player) {
        if (Player.HasHouse) {
            return -PAiMapAnalyzer.MinValueHouse(Game, Player).Value * 5 / 6;
        } else {
            return 0;
        }
    }

    public override void AnnouceInvokeJudge(PGame Game, PPlayer Player, PCard Card) {
        base.AnnouceInvokeJudge(Game, Player, Card);
        int Result = Game.Judge(Player);
        if (Result != 4) {
            Game.ThrowHouse(Player, Player, CardName);
        }
        Game.CardManager.MoveCard(Card, Player.Area.AmbushCardArea, Game.CardManager.ThrownCardHeap);
    }

    public readonly static string CardName = "兵粮寸断";

    public P_PingLiangTsuunTuan():base(CardName) {
        Point = 2;
        Index = 38;
        BuildAmbush(AIEmitTargets, false, 60);
    }
}