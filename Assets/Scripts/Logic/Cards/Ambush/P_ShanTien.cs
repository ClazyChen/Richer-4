using System;
using System.Collections.Generic;
/// <summary>
/// 闪电
/// </summary>
public class P_ShanTien : PAmbushCardModel {

    public List<PPlayer> AIEmitTargets(PGame Game, PPlayer Player) {
        
        return new List<PPlayer>() { Game.Enemies(Player).Count > Game.Teammates(Player).Count && (PMath.Min(Game.Teammates(Player), (PPlayer _Player) => _Player.Money).Value > 6000 || PMath.Min(Game.Enemies(Player), (PPlayer _Player) => _Player.Money).Value <= 6000) && (Player.Defensor == null || !(Player.Defensor.Model is P_ChiiHsingPaao)) ? Player : null };
    }

    public override int AIInHandExpectation(PGame Game, PPlayer Player) {
        return AIInAmbushExpectation(Game, Player);
    }

    public override int AIInAmbushExpectation(PGame Game, PPlayer Player) {
        return 6000 * 10 / 36 * (Game.Enemies(Player).Count - Game.Teammates(Player).Count + 4 * (Game.Enemies(Player).FindAll((PPlayer _Player) => _Player.Money <= 6000).Count - Game.Teammates(Player).FindAll((PPlayer _Player) => _Player.Money <= 6000).Count));
    }

    public override void AnnouceInvokeJudge(PGame Game, PPlayer Player, PCard Card) {
        base.AnnouceInvokeJudge(Game, Player, Card);
        int Result1 = Game.Judge(Player);
        int Result2 = Game.Judge(Player);
        if (Result1+Result2 <= 5) {
            Game.Injure(null, Player, 6000, Card);
            Game.CardManager.MoveCard(Card, Player.Area.AmbushCardArea, Game.CardManager.ThrownCardHeap);
        } else {
            Game.CardManager.MoveCard(Card, Player.Area.AmbushCardArea, Game.GetNextPlayer(Player).Area.AmbushCardArea);
        }
    }

    public readonly static string CardName = "闪电";

    public P_ShanTien():base(CardName) {
        Point = 4;
        Index = 40;
        BuildAmbush(AIEmitTargets, false, 10);
    }
}