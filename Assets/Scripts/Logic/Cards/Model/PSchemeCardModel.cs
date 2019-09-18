using System;
using System.Collections.Generic;


/// <summary>
/// PSchemeCardModel类：计策牌的模型基类
/// </summary>
public abstract class PSchemeCardModel: PCardModel {

    public PSchemeCardModel(string _Name):base(_Name) {
        Type = PCardType.SchemeCard;
    }

    private static List<PPlayer> ArrangeTargets(PGame Game, List<PPlayer> Targets, PPlayer Starter) {
        List<PPlayer> Answers = new List<PPlayer>();
        Game.Traverse((PPlayer Player) => {
            if (Targets.Contains(Player)) {
                Answers.Add(Player);
            }
        }, Starter);
        return Answers;
    }

    protected static Action<PGame> MakeNormalEffect(PPlayer Player, PCard Card, TargetChooser AITargetChooser, TargetChooser PlayerTargetChooser, EffectFunc Effect, Action<PGame,PPlayer,List<PPlayer>> StartAction = null, Action<PGame, PPlayer, List<PPlayer>> EndAction = null) {
        return (PGame Game) => {
            List<PPlayer> Targets = Player.IsAI ? AITargetChooser(Game, Player) : PlayerTargetChooser(Game,Player) ;
            Targets.RemoveAll((PPlayer _Player) => _Player == null);
            if (Targets.Count == 0) { return; }
            Game.Monitor.CallTime(PTime.Card.AfterEmitTargetTime, new PUseCardTag(Card, Player, Targets));
            Game.CardManager.MoveCard(Card, Player.Area.HandCardArea, Game.CardManager.SettlingArea);
            Game.Monitor.CallTime(PTime.Card.AfterBecomeTargetTime, new PUseCardTag(Card, Player, Targets));
            Targets = ArrangeTargets(Game, Targets, Player);
            Game.TagManager.CreateTag(new PUseCardTag(Card, Player, Targets));
            StartAction?.Invoke(Game, Player, Targets);
            Targets.ForEach((PPlayer Target) => {
                if (Target != null && Target.IsAlive) {
                    Effect(Game, Player, Target);
                }
            });
            EndAction?.Invoke(Game, Player, Targets);
            Game.TagManager.PopTag<PUseCardTag>(PUseCardTag.TagName);
            Game.CardManager.MoveCard(Card, Game.CardManager.SettlingArea, Game.CardManager.ThrownCardHeap);
            Game.Monitor.CallTime(PTime.Card.EndSettleTime, new PUseCardTag(Card, Player, Targets));
        };
    }

    protected static Action<PGame> MakeNormalEffect(PPlayer Player, PCard Card, TargetChooser AITargetChooser, PTrigger.PlayerCondition TargetCondition, EffectFunc Effect) {
        return MakeNormalEffect(Player, Card, AITargetChooser, (PGame Game, PPlayer _Player) => {
            return new List<PPlayer> { PNetworkManager.NetworkServer.ChooseManager.AskForTargetPlayer(_Player, TargetCondition, Card.Name) };
        }, Effect);
    }

    protected static Action<PGame> MakeMultiTargetNormalEffect(PPlayer Player, PCard Card, TargetChooser AITargetChooser, PTrigger.PlayerCondition TargetCondition, EffectFunc Effect, int MaxNumber = -1) {
        return MakeNormalEffect(Player, Card, AITargetChooser, (PGame Game, PPlayer _Player) => {
            return PNetworkManager.NetworkServer.ChooseManager.AskForTargetPlayers(_Player, TargetCondition, Card.Name, MaxNumber) ;
        }, Effect);
    }
}