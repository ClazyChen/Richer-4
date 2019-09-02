
using System.Collections.Generic;
/// <summary>
/// 釜底抽薪
/// </summary>
public class P_FuTiChoouHsin : PSchemeCardModel {

    public override int AIInHandExpectation(PGame Game, PPlayer Player) {
        int Basic = 5400;
        return Basic;
    }

    public readonly static string CardName = "釜底抽薪";

    public P_FuTiChoouHsin():base(CardName) {
        Point = 4;
        Index = 19;
        foreach (PTime Time in new PTime[] {
            PTime.Injure.AfterAcceptInjure
        }) {
            MoveInHandTriggerList.Add((PPlayer Player, PCard Card) => {
                return new PTrigger(CardName) {
                    IsLocked = false,
                    Player = Player,
                    Time = Time,
                    AIPriority = 60,
                    Condition = (PGame Game) => {
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        return Player.Equals(InjureTag.ToPlayer) && InjureTag.FromPlayer != null && InjureTag.Injure > 0 && InjureTag.InjureSource != null && (InjureTag.InjureSource is PBlock);
                    },
                    AICondition = (PGame Game) => {
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        PBlock Block = (PBlock)InjureTag.InjureSource;
                        return Block.Lord != null && Block.Lord.TeamIndex != Player.TeamIndex && (PAiMapAnalyzer.HouseValue(Game, Block.Lord, Block) >= 5400 || Player.Money <= 3000);
                    },
                    Effect = (PGame Game) => {
                        List<PPlayer> Targets = new List<PPlayer>();
                        Game.Monitor.CallTime(PTime.Card.AfterEmitTargetTime, new PUseCardTag(Card, Player, Targets));
                        Game.CardManager.MoveCard(Card, Player.Area.HandCardArea, Game.CardManager.SettlingArea);
                        Game.Monitor.CallTime(PTime.Card.AfterBecomeTargetTime, new PUseCardTag(Card, Player, Targets));
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        PBlock Block = (PBlock)InjureTag.InjureSource;
                        Game.LoseHouse(Block, Block.HouseNumber);
                        Game.CardManager.MoveCard(Card, Game.CardManager.SettlingArea, Game.CardManager.ThrownCardHeap);
                        Game.Monitor.CallTime(PTime.Card.EndSettleTime, new PUseCardTag(Card, Player, Targets));
                    }
                };
            });
        }
    }
}