using System;

/// <summary>
/// 点击技能命令
/// </summary>
/// SR：当发出者为当前回合的角色且正在进行空闲时间点且空闲时，且技能为主动技能且使用condition满足时，使用技能
///     当技能为软锁定的被动技能时，切换软锁定格式并返回一个RefreshSkillOrder
public class PClickOnSkillOrder : POrder {
    public PClickOnSkillOrder() : base("click_on_skill",
        (string[] args, string IPAddress) => {
            int SkillIndex = Convert.ToInt32(args[1]);
            PGame Game = PNetworkManager.Game;
            PPlayer Player = Game.PlayerList.Find((PPlayer _Player) => _Player.IPAddress.Equals(IPAddress));
            if (Player != null && Player.IsAlive && SkillIndex < Player.General.SkillList.Count) {
                PSkill Skill = Player.General.SkillList[SkillIndex];
                if (Skill != null) {
                    bool UseSkill = false;
                    if (Game.Logic.WaitingForEndFreeTime() && Game.NowPlayer.Equals(Player) && Game.TagManager.ExistTag(PTag.FreeTimeOperationTag.Name)) {
                        PTrigger Trigger = Skill.TriggerList.Find((Func<PPlayer, PSkill, PTrigger> TriggerGenerator) => TriggerGenerator(Player, Skill).Time.Equals(Game.NowPeriod.During))?.Invoke(Player, Skill);
                        if (Trigger != null) {
                            if (Trigger.Condition(Game)) {
                                PThread.Async(() => {
                                    Game.Logic.StartSettle(new PSettle("发动技能[" + Trigger.Name + "]", Trigger.Effect));
                                });
                                UseSkill = true;
                            }
                        }
                    }
                    if (Skill.SoftLockOpen && !UseSkill) {
                        Skill.Lock = !Skill.Lock;
                        Game.Monitor.FindTriggers(Skill.Name).ForEach((PTrigger Trigger) => {
                            Trigger.IsLocked = Skill.Lock;
                        });
                        PNetworkManager.NetworkServer.TellClient(Player, new PRefreshGeneralOrder(Player));
                    }
                }
            }
        },
        null) {
    }

    public PClickOnSkillOrder(string _SkillIndex) : this() {
        args = new string[] { _SkillIndex };
    }
}
