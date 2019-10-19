using System;
using System.Linq;
using System.Collections.Generic;

public class P_Faraday: PGeneral {

    public P_Faraday() : base("法拉第") {
        Sex = PSex.Male;
        Age = PAge.Industrial;
        Index = 16;
        Cost = 20;
        Tips = "定位：控制\n" +
            "难度：中等\n" +
            "史实：英国物理学家、化学家，曾提出电磁感应学说，发现了电场和磁场的联系，并发明了发电机，被称为“交流电之父”。\n" +
            "攻略：\n法拉第和时迁都是控制系武将。相对于时迁，法拉第利用的牌中包含【借尸还魂】、【抛砖引玉】、【走为上计】等价值极高的牌，在决定是否要发动【电击】的时候，会更为困难，并且也不具备时迁那样可以对对方的1点装备发动【轻敏】从而连续【顺手牵羊】的可能。\n" +
            "在1.2.3及以前的版本，法拉第只能使用3点牌发动技能，于1.2.4中得到了加强。";

        PSkill DianJi = new PSkill("电击") {
            Initiative = true
        };
        SkillList.Add(DianJi
            .AddTimeTrigger(
            new PTime[] {
                PPeriod.FirstFreeTime.During,
                PPeriod.SecondFreeTime.During
            },
            (PTime Time, PPlayer Player, PSkill Skill) => {
                return new PTrigger(DianJi.Name) {
                    IsLocked = false,
                    Player = Player,
                    Time = Time,
                    AIPriority = 200,
                    CanRepeat = true,
                    Condition = (PGame Game) => {
                        return Player.Equals(Game.NowPlayer) && 
                        (Player.IsAI || Game.Logic.WaitingForEndFreeTime()) &&
                        Game.PlayerList.Exists((PPlayer _Player) => !_Player.Equals(Player) && _Player.Distance(Player) <= 3) && 
                        Player.Area.HandCardArea.CardList.Exists((PCard Card) => Card.Point %3 == 0);
                    },
                    AICondition = (PGame Game) => {
                        int ShangWctExpect = P_ShangWuChoouTii.Target(Game, Player).Value;
                        return Player.Area.HandCardArea.CardList.Exists((PCard Card) => {
                            return Card.Point % 3 == 0 && Card.Model.AIInHandExpectation(Game, Player) < ShangWctExpect;
                        });
                    },
                    Effect = (PGame Game) => {
                        DianJi.AnnouceUseSkill(Player);
                        PCard TargetCard = null;
                        if (Player.IsAI) {
                            TargetCard = PAiCardExpectation.FindLeastValuable(Game, Player, Player, true, false, false, true, (PCard Card) => Card.Point % 3 == 0).Key;
                        } else {
                            List<PCard> Waiting = Player.Area.HandCardArea.CardList.FindAll((PCard Card) => Card.Point % 3 == 0);
                            int Result = PNetworkManager.NetworkServer.ChooseManager.Ask(Player, DianJi.Name, Waiting.ConvertAll((PCard Card) => Card.Name).Concat(new List<string> { "取消" }).ToArray());
                            if (Result >= 0 && Result < Waiting.Count) {
                                TargetCard = Waiting[Result];
                            }
                        }
                        if (TargetCard != null) {
                            TargetCard.Model = new P_ShangWuChoouTii();
                            PTrigger Trigger = TargetCard.Model.MoveInHandTriggerList.Find((Func<PPlayer, PCard, PTrigger> TriggerGenerator) => TriggerGenerator(Player, TargetCard).Time.Equals(PPeriod.FirstFreeTime.During))?.Invoke(Player, TargetCard);
                            if (Trigger != null) {
                                Game.Logic.StartSettle(new PSettle("电击[上屋抽梯]", Trigger.Effect));
                            }
                        }
                    }
                };
            }));
    }

}