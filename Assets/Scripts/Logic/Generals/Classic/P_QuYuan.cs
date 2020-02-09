using System;
using System.Collections.Generic;

public class P_QuYuan : PGeneral {

    public class PSaoTag : PNumberedTag {
        public static string TagName = "骚";
        public PSaoTag(int Number) : base(TagName, Number) {

        }
    }

    public P_QuYuan() : base("屈原") {
        Sex = PSex.Male;
        Age = PAge.Classic;
        Index = 25;
        Cost = 25;
        NewGeneral = true;
        Tips = "定位：全能\n" +
            "难度：待定\n" +
            "史实：战国时期楚国诗人、政治家。中国浪漫主义文学的奠基人，“楚辞”的代表作家。代表作《离骚》。\n" +
            "攻略：\n暂无";

        PSkill LiSao = new PSkill("离骚") {
            SoftLockOpen = true
        };
        SkillList.Add(LiSao
             .AddTrigger(
            (PPlayer Player, PSkill Skill) => {
                return new PTrigger(LiSao.Name) {
                    IsLocked = false,
                    Player = Player,
                    Time = PTime.Injure.AfterAcceptInjure,
                    AIPriority = 280,
                    Condition = (PGame Game) => {
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        return Player.Equals(InjureTag.ToPlayer) && InjureTag.Injure > 0;
                    },
                    AICondition = (PGame Game) => {
                        if (Player.Money <= 900) {
                            return false;
                        }
                        PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                        if (InjureTag.Injure >= Player.Money) {
                            return true;
                        } else if (InjureTag.Injure + 1800 >= Player.Money) {
                            return false;
                        }
                        return true;
                    },
                    Effect = (PGame Game) => {
                        LiSao.AnnouceUseSkill(Player);
                        int SaoNumber = 0;
                        int Line = 0;
                        int Count = 0;
                        int Last = 0;
                        int LLast = 0;
                        while (true) {
                            int Test = Game.Judge(Player, 6);
                            Count++;
                            if ((Test-Last)*Line < 0) {
                                Player.Tags.PopTag<PSaoTag>(PSaoTag.TagName);
                                break;
                            } else {
                                if (Test != Last && Last != 0) {
                                    Line = Test - Last;
                                }
                                LLast = Last;
                                Last = Test;
                            }
                            int Delta = SaoNumber * 9 + Test;
                            Player.Tags.CreateTag(new PSaoTag(Delta));
                            SaoNumber += Delta;
                        }
                        Game.LoseMoney(Player, 300 * Count);
                        Game.GetCard(Player);
                    }
                };
            }));
    }

}