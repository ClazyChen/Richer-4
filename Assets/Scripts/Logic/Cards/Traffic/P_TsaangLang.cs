
using System.Collections.Generic;
/// <summary>
/// 苍狼
/// </summary>
public class P_TsaangLang : PEquipmentCardModel {

    public override int AIInEquipExpectation(PGame Game, PPlayer Player) {
        return 500 + 4000 * Player.Area.HandCardArea.CardList.FindAll((PCard Card) => {
            PCardModel Model = Card.Model;
            return Model is P_ManTiienKuoHai || Model is P_HsiaoLiTsaangTao || Model is P_ChiaTaoFaKuo;
        }).Count;
    }

    public readonly static string CardName = "苍狼";

    public P_TsaangLang():base(CardName, PCardType.DefensorCard) {
        Point = 3;
        Index = 57;
        foreach (PTime Time in new PTime[] {
            PTime.Injure.EmitInjure
        }) {
            MoveInEquipTriggerList.Add((PPlayer Player, PCard Card) => {
                return new PTrigger(CardName) {
                    IsLocked = false,
                    Player = Player,
                    Time = Time,
                    AIPriority = 50,
                    Condition = (PGame Game) => {
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        bool CardSource = InjureTag.InjureSource is PCard;
                        if (CardSource) {
                            PUseCardTag UseCardTag = Game.TagManager.FindPeekTag<PUseCardTag>(PUseCardTag.TagName);
                            if (UseCardTag.TargetList.Count == 1 && UseCardTag.Card.Type.Equals(PCardType.SchemeCard)) {
                                return Player.Equals(InjureTag.FromPlayer) && InjureTag.Injure > 0 && InjureTag.ToPlayer != null && InjureTag.ToPlayer.Area.OwnerCardNumber > 0;
                            }
                        }
                        return false;
                    },
                    Effect = (PGame Game ) => {
                        AnnouceUseEquipmentSkill(Player);
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        Game.GetCardFrom(Player, InjureTag.ToPlayer);
                    }
                };
            });
        }
    }
}