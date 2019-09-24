using System;
using System.Collections.Generic;
/// <summary>
/// 指桑骂槐
/// </summary>
public class P_ChihSangMaHuai: PSchemeCardModel {

    private List<PPlayer> AIEmitTargets(PGame Game, PPlayer Player) {
        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
        PPlayer Target = PAiTargetChooser.InjureTarget(Game, Player, InjureTag.FromPlayer, (PGame _Game, PPlayer _Player) => {
            return _Player.IsAlive && !_Player.Equals(Player) && !_Player.Equals(InjureTag.FromPlayer);
        }, InjureTag.Injure, InjureTag.InjureSource, true);
        return new List<PPlayer>() { Target };
    }

    public override int AIInHandExpectation(PGame Game, PPlayer Player) {
        if (Game.Enemies(Player).Count < 2) {
            if (Game.Teammates(Player).Count < 2) {
                return 0;
            }
            return 1000;
        }
        return 6000;
    }

    public readonly static string CardName = "指桑骂槐";

    public P_ChihSangMaHuai():base(CardName) {
        Point = 5;
        Index = 26;
        foreach (PTime Time in new PTime[] {
            PTime.Injure.AcceptInjure
        }) {
            MoveInHandTriggerList.Add((PPlayer Player, PCard Card) => {
                return new PTrigger(CardName) {
                    IsLocked = false,
                    Player = Player,
                    Time = Time,
                    AIPriority = 200,
                    Condition = (PGame Game) => {
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        return Player.Equals(InjureTag.ToPlayer) && InjureTag.FromPlayer != null && InjureTag.Injure > 0 && Game.PlayerList.Exists((PPlayer _Player) => !_Player.Equals(InjureTag.FromPlayer) && !_Player.Equals(Player));
                    },
                    AICondition = (PGame Game) => {
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        return Player.Money <= InjureTag.Injure || (InjureTag.FromPlayer.TeamIndex != Player.TeamIndex && Game.Enemies(Player).Count < 2 && InjureTag.Injure >= 3000) && AIEmitTargets(Game, Player)[0] != null;
                    },
                    Effect = MakeNormalEffect(Player, Card, AIEmitTargets, (PGame Game, PPlayer _Player) => {
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        return !_Player.Equals(InjureTag.FromPlayer) && !_Player.Equals(Player);
                    },
                        (PGame Game, PPlayer User, PPlayer Target) => {
                            PNetworkManager.NetworkServer.TellClients(new PPushTextOrder(User.Index.ToString(), "转移伤害给" + Target.Name, PPushType.Information.Name));
                            Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName).ToPlayer = Target;
                        })
                };
            });
        }
    }
}