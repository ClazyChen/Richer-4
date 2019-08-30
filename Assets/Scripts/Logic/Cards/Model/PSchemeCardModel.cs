using System;
using System.Collections.Generic;


/// <summary>
/// PSchemeCardModel类：计策牌的模型基类
/// </summary>
public abstract class PSchemeCardModel: PCardModel {

    public PSchemeCardModel(string _Name):base(_Name) {
        Type = PCardType.SchemeCard;
    }

    protected Action<PGame> MakeNormalEffect(PPlayer Player, PCard Card, TargetChooser AITargetChooser, TargetChooser PlayerTargetChooser, EffectFunc Effect) {
        return (PGame Game) => {
            List<PPlayer> Targets = Player.IsAI ? AITargetChooser(Game, Player) : PlayerTargetChooser(Game,Player) ;
            Targets.RemoveAll((PPlayer _Player) => _Player == null);
            if (Targets.Count == 0) { return; }
            Game.Monitor.CallTime(PTime.Card.AfterEmitTargetTime, new PUseCardTag(Card, Player, Targets));
            Game.CardManager.MoveCard(Card, Player.Area.HandCardArea, Game.CardManager.SettlingArea);
            List<PPlayer> EffectTargets = CloneList(Targets);
            Game.Monitor.CallTime(PTime.Card.StartSettleTime, new PUseCardTag(Card, Player, EffectTargets));
            EffectTargets.ForEach((PPlayer Target) => {
                if (Target != null && Target.IsAlive) {
                    Effect(Game, Player, Target);
                }
            });
            Game.CardManager.MoveCard(Card, Game.CardManager.SettlingArea, Game.CardManager.ThrownCardHeap);
            Game.Monitor.CallTime(PTime.Card.EndSettleTime, new PUseCardTag(Card, Player, Targets));
        };
    }

    protected Action<PGame> MakeNormalEffect(PPlayer Player, PCard Card, TargetChooser AITargetChooser, PTrigger.PlayerCondition TargetCondition, EffectFunc Effect) {
        return MakeNormalEffect(Player, Card, AITargetChooser, (PGame Game, PPlayer _Player) => {
            return new List<PPlayer> { PNetworkManager.NetworkServer.ChooseManager.AskForTargetPlayer(_Player, TargetCondition, Card.Name) };
        }, Effect);
    }
}