using System;
using System.Collections.Generic;

public class P_YuJi : PGeneral {

    public P_YuJi() : base("虞姬") {
        Sex = PSex.Female;
        Age = PAge.Classic;
        Index = 13;
        Cost = 15;
        Tips = "定位：攻击\n" +
            "难度：简单\n" +
            "史实：楚汉相争时期，西楚霸王项羽的美人，项羽曾为其作《垓下歌》。\n" +
            "攻略：\n虞姬是一个依赖于手牌的武将，所以研究所是她和她的队友的优先选择。对于虞姬而言，如何较为准确地衡量手牌的价值是一个比较困难的问题。对于两张点数不同的牌，两张牌自身的价值有高有低，两张牌用来【剑舞】的价值也有高有低，使用什么牌来【剑舞】，是虞姬玩家水平分别的体现。\n" +
            "通常，虞姬玩家可以弃掉自己不需要的装备、伏兵和难以发挥积极作用的【擒贼擒王】、【假痴不癫】、【空城计】等牌发动【剑舞】。虞姬玩家也可以在手上存留很多牌，当与敌人近身接触时，把所有牌都用来【剑舞】，造成巨大的输出，以求直接杀死对手。";

        PSkill JianWu = new PSkill("剑舞") {
            Initiative = true
        };
        KeyValuePair<PCard, int> JianWuTest(PGame Game, PPlayer Player) {
            KeyValuePair<PCard, int> Answer = new KeyValuePair<PCard, int>(null, 0);
            for (int i = 1; i <= 6; ++i) {
                int Expect = (int)PMath.Sum(Game.AlivePlayers().FindAll((PPlayer _Player) => {
                    return !_Player.Equals(Player) && _Player.Distance(Player) <= i;
                }).ConvertAll((PPlayer _Player) => {
                    return (double)PAiTargetChooser.InjureExpect(Game, Player, Player, _Player, 800, JianWu);
                }));
                KeyValuePair<PCard, int> Test = PMath.Max(Player.Area.HandCardArea.CardList.FindAll((PCard Card) => Card.Point == i), (PCard Card) => {
                    return Expect - Card.Model.AIInHandExpectation(Game, Player);
                }, true);
                if (Test.Value > Answer.Value) {
                    Answer = Test;
                }
            }
            return Answer;
        }
        SkillList.Add(JianWu
            .AddTimeTrigger(
            new PTime[] {
                PPeriod.FirstFreeTime.During,
                PPeriod.SecondFreeTime.During
            },
            (PTime Time, PPlayer Player, PSkill Skill) => {
                return new PTrigger(JianWu.Name) {
                    IsLocked = false,
                    Player = Player,
                    Time = Time,
                    AIPriority = 190,
                    CanRepeat = true,
                    Condition = (PGame Game) => {
                        return Player.Equals(Game.NowPlayer) && (Player.IsAI || Game.Logic.WaitingForEndFreeTime()) && Player.Area.HandCardArea.CardNumber > 0 ;
                    },
                    AICondition = (PGame Game) => {
                        return JianWuTest(Game, Player).Key != null;
                    },
                    Effect = (PGame Game) => {
                        JianWu.AnnouceUseSkill(Player);
                        PCard TargetCard = null;
                        if (Player.IsAI) {
                            TargetCard = JianWuTest(Game, Player).Key;
                            PNetworkManager.NetworkServer.TellClients(new PShowInformationOrder(Player.Name + "弃置了" + Player.Name + "的" + TargetCard.Name));
                            Game.CardManager.MoveCard(TargetCard, Player.Area.HandCardArea, Game.CardManager.ThrownCardHeap);
                        } else {
                            TargetCard = Game.ThrowCard(Player, Player, true, false);
                        }
                        if (TargetCard != null) {
                            Game.Traverse((PPlayer _Player) => {
                                if (!_Player.Equals(Player) && _Player.Distance(Player) <= TargetCard.Point) {
                                    Game.Injure(Player, _Player, 800, JianWu);
                                }
                            }, Player);
                        }
                    }
                };
            }));
    }

}