using System;
using System.Collections.Generic;


/// <summary>
/// PAmbushCardModel：伏兵牌的模型基类
/// </summary>
public abstract class PAmbushCardModel : PCardModel {

    public virtual void AnnouceInvokeJudge(PGame Game, PPlayer Player, PCard Card) {
        PNetworkManager.NetworkServer.TellClients(new PShowInformationOrder("伏兵[" + Card.Name + "]生效"));
    }

    /// <summary>
    /// 往手牌触发器列表中加入使用伏兵的函数
    /// </summary>
    /// <param name="AITargetChooser">AI指定目标的函数</param>
    /// <param name="SelfOnly">是否只能指定自己为目标，false表示不能指定自己</param>
    /// <param name="AIPriority">AI优先级</param>
    protected void BuildAmbush(TargetChooser AITargetChooser, bool SelfOnly, int AIPriority) {

        foreach (PTime Time in new PTime[] {
            PPeriod.FirstFreeTime.During,
            PPeriod.SecondFreeTime.During
        }) {
            MoveInHandTriggerList.Add((PPlayer Player, PCard Card) => {
                return new PTrigger("使用伏兵") {
                    IsLocked = false,
                    Player = Player,
                    Time = Time,
                    AIPriority = AIPriority,
                    Condition = (PGame Game) => {
                        bool Existed = false;
                        if (SelfOnly) {
                            Existed = Player.Area.AmbushCardArea.CardList.Exists((PCard _Card) => _Card.Model.Name.Equals(Card.Name));
                        } else {
                            Existed = Game.PlayerList.FindAll((PPlayer _Player) => _Player.IsAlive && !_Player.Equals(Player)).TrueForAll((PPlayer _Player) => _Player.Area.AmbushCardArea.CardList.Exists((PCard _Card) => _Card.Model.Name.Equals(Card.Name)));
                        }
                        return Player.Equals(Game.NowPlayer) && (Player.IsAI || Game.Logic.WaitingForEndFreeTime()) && !Existed;
                    },
                    AICondition = (PGame Game) => {
                        return AITargetChooser(Game, Player)[0] != null;
                    },
                    Effect = (PGame Game) => {
                        List<PPlayer> Targets = new List<PPlayer> { null };
                        if (Player.IsAI) {
                            Targets = AITargetChooser(Game, Player);
                        } else {
                            if (SelfOnly) {
                                Targets = new List<PPlayer> { Player };
                            } else {
                                Targets = new List<PPlayer> { PNetworkManager.NetworkServer.ChooseManager.AskForTargetPlayer(Player, (PGame _Game, PPlayer _Player) => {
                                    return _Player.IsAlive && !_Player.Equals(Player) && !_Player.Area.AmbushCardArea.CardList.Exists((PCard _Card) => _Card.Model.Name.Equals(Card.Name));
                                }, Card.Name) };
                            }
                        }
                        Targets.RemoveAll((PPlayer _Player) => _Player == null);
                        if (Targets.Count == 0) { return; }
                        Game.Monitor.CallTime(PTime.Card.AfterEmitTargetTime, new PUseCardTag(Card, Player, Targets));
                        Game.Monitor.CallTime(PTime.Card.AfterBecomeTargetTime, new PUseCardTag(Card, Player, Targets));
                        if (Targets.Count > 0) {
                            Game.CardManager.MoveCard(Card, Player.Area.HandCardArea, Targets[0].Area.AmbushCardArea);
                        } else {
                            Game.CardManager.MoveCard(Card, Player.Area.HandCardArea, Game.CardManager.ThrownCardHeap);
                        }
                        Game.Monitor.CallTime(PTime.Card.EndSettleTime, new PUseCardTag(Card, Player, Targets));
                    }
                };
            });
        }
    }

    protected PAmbushCardModel(string _Name):base(_Name) {
        Type = PCardType.AmbushCard;
    }
}