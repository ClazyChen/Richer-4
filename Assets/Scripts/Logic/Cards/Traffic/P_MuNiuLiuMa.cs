using System.Linq;
using System.Collections.Generic;
/// <summary>
/// 木牛流马
/// </summary>
public class P_MuNiuLiuMa : PEquipmentCardModel {

    public override int AIInEquipExpectation(PGame Game, PPlayer Player) {
        return Game.Teammates(Player, false).Count > 0 ? 2000 : 0;
    }

    public readonly static string CardName = "木牛流马";

    public P_MuNiuLiuMa():base(CardName, PCardType.TrafficCard) {
        Point = 6;
        Index = 60;
        AnnouceOnce(CardName);
        foreach (PTime Time in new PTime[] {
            PPeriod.FirstFreeTime.During,
            PPeriod.SecondFreeTime.During
        }) {
            MoveInEquipTriggerList.Add((PPlayer Player, PCard Card) => {
                return new PTrigger(CardName) {
                    IsLocked = true,
                    Player = Player,
                    Time = Time,
                    AIPriority = 15,
                    Condition = (PGame Game) => {
                        PUsedTag UsedTag = Player.Tags.FindPeekTag<PUsedTag>(PUsedTag.TagNamePrefix + CardName);
                        return Player.Equals(Game.NowPlayer) && (Player.IsAI || Game.Logic.WaitingForEndFreeTime()) && UsedTag != null && UsedTag.Count < UsedTag.Limit && Player.Money > 2000;
                    },
                    AICondition = (PGame Game) => {
                        return Game.Teammates(Player, false).Count > 0 && (Player.Money >= 20000 || (Player.Money >= 10000 && PMath.RandTest(0.5)));
                    },
                    Effect = (PGame Game ) => {
                        AnnouceUseEquipmentSkill(Player);
                        Game.LoseMoney(Player, 2000);
                        PPlayer Target = null;
                        if (Player.IsAI) {
                            Target = PMath.Max(Game.Teammates(Player, false), (PPlayer _Player) => _Player.AiCardExpectation).Key;
                        } else {
                            Target = PNetworkManager.NetworkServer.ChooseManager.AskForTargetPlayer(Player, PTrigger.Except(Player), CardName);
                        }
                        if (Target != null) {
                            Game.GetCard(Target, 1);
                        }
                        Player.Tags.FindPeekTag<PUsedTag>(PUsedTag.TagNamePrefix + CardName).Count++;
                    }
                };
            });
        }
    }
}