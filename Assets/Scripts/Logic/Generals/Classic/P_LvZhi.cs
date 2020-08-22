using System;
using System.Collections.Generic;

public class P_LvZhi: PGeneral {

    const int ZhenShaInjure = 1500;

    public P_LvZhi() : base("吕雉") {
        Sex = PSex.Female;
        Age = PAge.Classic;
        Index = 29;
        Cost = 30;
        NewGeneral = true;
        Tips = "定位：攻击\n" +
            "难度：简单\n" +
            "史实：汉高祖刘邦的皇后，惠帝、二少帝时的皇太后。统治时期实行与民休息的政策，为文景之治奠定了基础。\n" +
            "攻略：\n-";

        PSkill ZhenSha = new PSkill("鸩杀") {
            Initiative = true
        };
        PPlayer ZhenShaTarget(PGame Game, PPlayer Player) {
            List<PPlayer> PossibleTargets = Game.AlivePlayers(Player).FindAll((PPlayer _Player) => _Player.Distance(Player) <= 1);
            PPlayer Target = PMath.Max(PossibleTargets, (PPlayer _Player) => {
                int InjureValue = PAiTargetChooser.InjureExpect(Game, Player, Player, _Player, ZhenShaInjure, ZhenSha);
                if (_Player.TeamIndex == Player.TeamIndex) {
                    KeyValuePair<PCard, int> MaxValueCard = PAiCardExpectation.FindMostValuable(Game, _Player, Player, true, false, false, true);
                    int MaxValue = MaxValueCard.Value - MaxValueCard.Key.AIInHandExpectation(Game, Player);
                    return MaxValue + InjureValue;
                } else {
                    KeyValuePair<PCard, int> MinValueCard = PAiCardExpectation.FindLeastValuable(Game, _Player, Player, true, false, false, true);
                    int MinValue = MinValueCard.Value + MinValueCard.Key.AIInHandExpectation(Game, Player);
                    return -MinValue + InjureValue;
                }
            }, true).Key;
            return Target;
        }
        SkillList.Add(ZhenSha
            .AnnounceTurnOnce()
            .AddTimeTrigger(
            new PTime[] {
                PPeriod.FirstFreeTime.During,
                PPeriod.SecondFreeTime.During
            },
            (PTime Time, PPlayer Player, PSkill Skill) => {
                return new PTrigger(ZhenSha.Name) {
                    IsLocked = false,
                    Player = Player,
                    Time = Time,
                    AIPriority = 40,
                    CanRepeat = true,
                    Condition = (PGame Game) => {
                        return Player.Equals(Game.NowPlayer) && (Player.IsAI || Game.Logic.WaitingForEndFreeTime()) && Player.RemainLimit(ZhenSha.Name) && Player.Area.HandCardArea.CardNumber > 0 && Game.AlivePlayers(Player).Exists((PPlayer _Player) => _Player.Distance(Player) <= 1);
                    },
                    AICondition = (PGame Game) => {
                        return ZhenShaTarget(Game, Player) != null;
                    },
                    Effect = (PGame Game) => {
                        ZhenSha.AnnouceUseSkill(Player);
                        PPlayer Target = null;
                        if (Player.IsAI) {
                            Target = ZhenShaTarget(Game, Player);
                        } else {
                            Target = PNetworkManager.NetworkServer.ChooseManager.AskForTargetPlayer(Player, (PGame _Game, PPlayer _Player) => _Player.Distance(Player) <= 1 && !_Player.Equals(Player), ZhenSha.Name, true);
                        }
                        if (Target != null) {
                            Game.GiveCardTo(Player, Target, true, false);
                            Game.Injure(Player, Target, 1500, ZhenSha);
                            ZhenSha.DeclareUse(Player);
                        }
                    }
                };
            }));
        PSkill XuMou = new PSkill("蓄谋") {
            Lock = true
        };
        SkillList.Add(XuMou
            .AddTrigger(
            (PPlayer Player, PSkill Skill) => {
                return new PTrigger(XuMou.Name) {
                    IsLocked = true,
                    Player = Player,
                    Time = PTime.LeaveDyingTime,
                    AIPriority = 100,
                    Condition = (PGame Game) => {
                        PDyingTag DyingTag = Game.TagManager.FindPeekTag<PDyingTag>(PDyingTag.TagName);
                        return Player.Equals(DyingTag.Killer) && !Player.Equals(DyingTag.Player);
                    },
                    Effect = (PGame Game) => {
                        XuMou.AnnouceUseSkill(Player);
                        PDyingTag DyingTag = Game.TagManager.FindPeekTag<PDyingTag>(PDyingTag.TagName);
                        DyingTag.Player.Money = 0;
                        Game.Die(DyingTag.Player, DyingTag.Killer);
                    }
                };
            }));
    }

}