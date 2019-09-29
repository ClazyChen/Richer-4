using System;
using System.Collections.Generic;
/// <summary>
/// 乐不思蜀
/// </summary>
public class P_LevPuSsuShu : PAmbushCardModel {

    public List<PPlayer> AIEmitTargets(PGame Game, PPlayer Player) {
        PPlayer Target = PMath.Max(Game.Enemies(Player).FindAll(
            (PPlayer _Player) => _Player.Area.HandCardArea.CardNumber >= 4 &&
            (_Player.Defensor == null || !(_Player.Defensor.Model is P_ChiiHsingPaao)) &&
            !_Player.Area.AmbushCardArea.CardList.Exists((PCard _Card) => 
                _Card.Model.Name.Equals(CardName)) &&
            !(_Player.General is P_LiuJi))
            , (PPlayer _Player) => _Player.Area.HandCardArea.CardNumber * 100 + PMath.RandInt(0, 10)).Key;
        return new List<PPlayer>() { Target  };
    }

    public override int AIInHandExpectation(PGame Game, PPlayer Player) {
        int Test = PMath.Max(Game.Enemies(Player), (PPlayer _Player) => {
            if (_Player.General is P_LiuJi) {
                return 0;
            }
            return _Player.Area.HandCardArea.CardNumber / 2 * 2000 * 5 / 6;
        }).Value;
        int Base = 3500;
        return Math.Max(Base, Test);
    }

    public override int AIInAmbushExpectation(PGame Game, PPlayer Player) {
        int Test = Player.Area.HandCardArea.CardNumber / 2 * 2000;
        if (Player.General is P_LiuJi) {
            return Math.Max(-Test + 1200, 600);
        }
        return - Test * 5 / 6;
    }

    public override void AnnouceInvokeJudge(PGame Game, PPlayer Player, PCard Card) {
        base.AnnouceInvokeJudge(Game, Player, Card);
        int Result = Game.Judge(Player, new Func<int>(() => {
            if (Player.Area.HandCardArea.CardNumber <= 1) {
                return 6;
            }
            return 3;
        })());
        if (Result != 3) {
            int ThrowNumber = Player.Area.HandCardArea.CardNumber / 2;
            for (int i = 0; i <ThrowNumber; ++ i) {
                Game.ThrowCard(Player, Player, true, false);
            }
        }
        Game.CardManager.MoveCard(Card, Player.Area.AmbushCardArea, Game.CardManager.ThrownCardHeap);
    }

    public readonly static string CardName = "乐不思蜀";

    public P_LevPuSsuShu():base(CardName) {
        Point = 1;
        Index = 37;
        BuildAmbush(AIEmitTargets, false, 80);
    }
}