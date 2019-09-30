
using System.Collections.Generic;
/// <summary>
/// 反客为主
/// </summary>
public class P_FanKeevWeiChu : PSchemeCardModel {

    public override int AIInHandExpectation(PGame Game, PPlayer Player) {
        int Basic = 5400;
        if (!Game.Map.BlockList.Exists((PBlock Block) => {
            return Block.CanPurchase && Block.HouseNumber < 2;
        })) {
            Basic = 0;
        }
        return Basic;
    }

    public readonly static string CardName = "反客为主";

    public P_FanKeevWeiChu():base(CardName) {
        Point = 5;
        Index = 30;
        foreach (PTime Time in new PTime[] {
            PTime.Injure.AfterAcceptInjure
        }) {
            MoveInHandTriggerList.Add((PPlayer Player, PCard Card) => {
                return new PTrigger(CardName) {
                    IsLocked = false,
                    Player = Player,
                    Time = Time,
                    AIPriority = 100,
                    Condition = (PGame Game) => {
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        if (!(InjureTag.InjureSource is PBlock)) {
                            return false;
                        } else {
                            PBlock Block = (PBlock)InjureTag.InjureSource;
                            return Player.Equals(InjureTag.ToPlayer) && InjureTag.Injure > 0 && InjureTag.InjureSource != null && !Player.Equals(Block.Lord) && Block.HouseNumber == 1;
                        }
                    },
                    AICondition = (PGame Game) => {
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        PBlock Block = (PBlock)InjureTag.InjureSource;
                        return Player.TeamIndex != Block.Lord.TeamIndex;
                    },
                    Effect = (PGame Game) => {
                        List<PPlayer> Targets = new List<PPlayer>();
                        Game.Monitor.CallTime(PTime.Card.AfterEmitTargetTime, new PUseCardTag(Card, Player, Targets));
                        Game.CardManager.MoveCard(Card, Player.Area.HandCardArea, Game.CardManager.SettlingArea);
                        Game.Monitor.CallTime(PTime.Card.AfterBecomeTargetTime, new PUseCardTag(Card, Player, Targets));
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        PBlock Block = (PBlock)InjureTag.InjureSource;

                        Block.Lord = Player;
                        PNetworkManager.NetworkServer.TellClients(new PHighlightBlockOrder(Block.Index.ToString()));
                        PNetworkManager.NetworkServer.TellClients(new PRefreshBlockBasicOrder(Block));

                        #region 成就：我的地盘我做主
                        if (Block.BusinessType.Equals(PBusinessType.Castle)) {
                            PArch.Announce(Game, Player, "我的地盘我做主");
                        }
                        #endregion

                        Game.CardManager.MoveCard(Card, Game.CardManager.SettlingArea, Game.CardManager.ThrownCardHeap);
                        Game.Monitor.CallTime(PTime.Card.EndSettleTime, new PUseCardTag(Card, Player, Targets));
                    }
                };
            });
        }
    }
}