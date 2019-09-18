
using System.Collections.Generic;
/// <summary>
/// 投石机
/// </summary>
public class P_ToouShihChi : PEquipmentCardModel {

    public override int AIInEquipExpectation(PGame Game, PPlayer Player) {
        return 500 + 5000 * Game.Map.BlockList.FindAll((PBlock Block) => Block.Lord != null && Block.Lord.TeamIndex != Player.TeamIndex && Block.HouseNumber > 1 && Block.BusinessType.Equals(PBusinessType.Castle)).Count;
    }

    public readonly static string CardName = "投石机";

    public P_ToouShihChi():base(CardName, PCardType.WeaponCard) {
        Point = 6;
        Index = 48;
        AnnouceOnce(CardName);
        foreach (PTime Time in new PTime[] {
            PPeriod.FirstFreeTime.During,
            PPeriod.SecondFreeTime.During
        }) {
            MoveInEquipTriggerList.Add((PPlayer Player, PCard Card) => {
                return new PTrigger(CardName) {
                    IsLocked = false,
                    Player = Player,
                    Time = Time,
                    CanRepeat = true,
                    Condition = (PGame Game) => {
                        PUsedTag UsedTag = Player.Tags.FindPeekTag<PUsedTag>(PUsedTag.TagNamePrefix + CardName);
                        if (UsedTag == null) {
                            Player.Tags.CreateTag(UsedTag = new PUsedTag(CardName, 1));
                        }
                        return Player.Equals(Game.NowPlayer) && (Player.IsAI || Game.Logic.WaitingForEndFreeTime()) && UsedTag != null && UsedTag.Count < UsedTag.Limit && Player.Money > 3000 && Game.Map.BlockList.Exists((PBlock Block ) => Block.HouseNumber > 0);
                    },
                    AICondition = (PGame Game) => {
                        return Game.Map.BlockList.Exists((PBlock Block) => Block.Lord != null && Block.Lord.TeamIndex != Player.Index && Block.HouseNumber > 1 && Block.BusinessType.Equals(PBusinessType.Castle)) && Player.Money >= 6000;
                    },
                    Effect = (PGame Game) => {
                        AnnouceUseEquipmentSkill(Player);
                        PBlock TargetBlock = null;
                        if (Player.IsAI) {
                            TargetBlock = PMath.Max(Game.Map.BlockList.FindAll((PBlock Block) => Block.Lord != null && Block.Lord.TeamIndex != Player.TeamIndex && Block.BusinessType.Equals(PBusinessType.Castle)), (PBlock Block) => Block.HouseNumber).Key;
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