
using System.Collections.Generic;
/// <summary>
/// 银月枪
/// </summary>
public class P_YinYooehChiiang : PEquipmentCardModel {

    public override int AIInEquipExpectation(PGame Game, PPlayer Player) {
        return 500 + 1500 * Player.Area.HandCardArea.CardList.FindAll((PCard Card) => Card.MoveInHandTriggerList.Exists((PTrigger Trigger) => !Trigger.Time.IsPeroidTime())).Count;
    }

    public readonly static string CardName = "银月枪";

    public P_YinYooehChiiang():base(CardName, PCardType.WeaponCard) {
        Point = 3;
        Index = 45;
        foreach (PTime Time in new PTime[] {
            PTime.Card.EndSettleTime
        }) {
            MoveInEquipTriggerList.Add((PPlayer Player, PCard Card) => {
                return new PTrigger(CardName) {
                    IsLocked = false,
                    Player = Player,
                    Time = Time,
                    Condition = (PGame Game) => {
                        PUseCardTag UseCardTag = Game.TagManager.FindPeekTag<PUseCardTag>(PUseCardTag.TagName);
                        return !Game.NowPlayer.Equals(Player) && UseCardTag.User.Equals(Player);
                    },
                    Effect = (PGame Game) => {
                        AnnouceUseEquipmentSkill(Player);
                        PPlayer TargetPlayer = null;
                        if (Player.IsAI) {
                            TargetPlayer = PAiTargetChooser.InjureTarget(Game, Player, Player, PTrigger.Except(Player), 1000, Card);
                        } else {
                            TargetPlayer = PNetworkManager.NetworkServer.ChooseManager.AskForTargetPlayer(Player, PTrigger.Except(Player), CardName);
                        }
                        if (TargetPlayer != null) {
                            Game.Injure(Player, TargetPlayer, 1000, Card);
                        }
                    }
                };
            });
        }
    }
}