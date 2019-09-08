
using System.Collections.Generic;
/// <summary>
/// 投石车
/// </summary>
public class P_ToouShihCheev : PEquipmentCardModel {

    public override int AIInEquipExpectation(PGame Game, PPlayer Player) {
        return 500 + 5000 * Game.Map.BlockList.FindAll((PBlock Block) => Block.Lord != null && Block.Lord.TeamIndex != Player.Index && Block.HouseNumber > 1 && Block.BusinessType.Equals(PBusinessType.Castle)).Count;
    }

    public readonly static string CardName = "投石车";

    public P_ToouShihCheev():base(CardName, PCardType.WeaponCard) {
        Point = 6;
        Index = 48;
        MoveInEquipTriggerList.Add((PPlayer Player, PCard Card) => {
            return new PTrigger(CardName) {
                IsLocked = true,
                Player = Player,
                Time = PPeriod.StartTurn.Start,
                Condition = (PGame Game) => {
                    return Player.Equals(Game.NowPlayer);
                },
                Effect = (PGame Game) => {
                    Player.Tags.CreateTag(new PUsedTag(CardName, 1));
                }
            };
        });
        foreach (PTime Time in new PTime[] {
            PPeriod.FirstFreeTime.During,
            PPeriod.SecondFreeTime.During
        }) {
            MoveInEquipTriggerList.Add((PPlayer Player, PCard Card) => {
                return new PTrigger(CardName) {
                    IsLocked = false,
                    Player = Player,
                    Time = Time,
                    Condition = (PGame Game) => {
                        PUsedTag UsedTag = Player.Tags.FindPeekTag<PUsedTag>(PUsedTag.TagNamePrefix + CardName);
                        return Player.Equals(Game.NowPlayer) && (Player.IsAI || Game.Logic.WaitingForEndFreeTime()) && UsedTag != null && UsedTag.Count < UsedTag.Limit && Player.Money > 3000;
                    },
                    AICondition = (PGame Game) => {
                        return Game.Map.BlockList.Exists((PBlock Block) => Block.Lord != null && Block.Lord.TeamIndex != Player.Index && Block.HouseNumber > 1 && Block.BusinessType.Equals(PBusinessType.Castle)) && Player.Money >= 6000;
                    },
                    Effect = (PGame Game) => {
                        PBlock TargetBlock = null;
                        if (Player.IsAI) {
                            TargetBlock = PMath.Max(Game.Map.BlockList.FindAll((PBlock Block) => Block.Lord != null && Block.Lord.TeamIndex != Player.Index && Block.BusinessType.Equals(PBusinessType.Castle)), (PBlock Block) => Block.HouseNumber).Key;
                        } else {
                            TargetBlock = PNetworkManager.NetworkServer.ChooseManager.AskToChooseBlock(Player, CardName + "之目标", (PBlock Block) => Block.HouseNumber > 0);
                        }
                        if (TargetBlock != null) {
                            Game.LoseMoney(Player, 3000);
                            Game.LoseHouse(TargetBlock, 1);
                            if (TargetBlock.BusinessType.Equals(PBusinessType.Castle)) {
                                Game.LoseHouse(TargetBlock, Game.Judge(Player));
                            }
                            Player.Tags.FindPeekTag<PUsedTag>(PUsedTag.TagNamePrefix + CardName).Count++;
                        }
                    }
                };
            });
        }
    }
}