using System;
using System.Collections.Generic;
/// <summary>
/// 草木皆兵
/// </summary>
public class P_TsaaoMuChiehPing : PAmbushCardModel {

    public int AIExpect(PGame Game, PPlayer Player, PBlock Block) {
        int[] dx = { 1, -1, 0, 0 };
        int[] dy = { 0, 0, 1, -1 };
        int OriginalX = Block.X;
        int OriginalY = Block.Y;
        int sum, x, y;
        sum = PAiMapAnalyzer.HouseValue(Game, Player, Block);
        for (int i = 0; i < 4; ++i) {
            x = OriginalX;
            y = OriginalY;
            do {
                x += dx[i];
                y += dy[i];
                PBlock TempBlock = Game.Map.FindBlockByCoordinate(x, y);
                if (TempBlock != null) {
                    if (TempBlock.Lord != null) {
                        sum += PAiMapAnalyzer.HouseValue(Game, Player, TempBlock);
                    }
                } else {
                    break;
                }
            } while (true);
        }
        return sum;
    }


    public List<PPlayer> AIEmitTargets(PGame Game, PPlayer Player) {
        
        return new List<PPlayer>() { PMath.Max(Game.PlayerList.FindAll((PPlayer _Player) => _Player.IsAlive && !_Player.Equals(Player)), (PPlayer _Player) => AIExpect(Game, _Player, _Player.Position) - 4000, true).Key };
    }

    public override int AIInHandExpectation(PGame Game, PPlayer Player) {
        return 3500;
    }

    public override int AIInAmbushExpectation(PGame Game, PPlayer Player) {
        return AIExpect(Game, Player, Player.Position);
    }

    public override void AnnouceInvokeJudge(PGame Game, PPlayer Player, PCard Card) {
        base.AnnouceInvokeJudge(Game, Player, Card);
        int Result = Game.Judge(Player);
        if (Result != 1) {
            PBlock Block = Player.Position;
            int[] dx = { 1, -1, 0, 0 };
            int[] dy = { 0, 0, 1, -1 };
            int OriginalX = Block.X;
            int OriginalY = Block.Y;
            int x, y;
            Game.GetHouse(Block, 1);
            for (int i = 0; i < 4; ++i) {
                x = OriginalX;
                y = OriginalY;
                do {
                    x += dx[i];
                    y += dy[i];
                    PBlock TempBlock = Game.Map.FindBlockByCoordinate(x, y);
                    if (TempBlock != null) {
                        if (TempBlock.Lord != null) {
                            Game.GetHouse(TempBlock, 1);
                        }
                    } else {
                        break;
                    }
                } while (true);
            }
        }
        Game.CardManager.MoveCard(Card, Player.Area.AmbushCardArea, Game.CardManager.ThrownCardHeap);
    }

    public readonly static string CardName = "草木皆兵";

    public P_TsaaoMuChiehPing():base(CardName) {
        Point = 3;
        Index = 39;
        BuildAmbush(AIEmitTargets, false, 60);
    }
}