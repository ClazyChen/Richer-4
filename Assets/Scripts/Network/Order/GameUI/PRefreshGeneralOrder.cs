using System;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// 更新武将命令+玩家编号+武将名+技能数量+[技能名+技能类型]*N
/// </summary>
/// CR：所有玩家更新武将头像。同一玩家对于更新技能按钮的图标
public class PRefreshGeneralOrder : POrder {
    public PRefreshGeneralOrder() : base("refresh_general",
        null,
        (string[] args) => {
            List<PSkillInfo> SkillInfoList = new List<PSkillInfo>();
            int PlayerIndex = Convert.ToInt32(args[1]);
            string GeneralName = args[2];
            int SkillCount = Convert.ToInt32(args[3]);
            for (int i = 4; i + 1 < args.Length; i += 2) {
                PSkillInfo SkillInfo = FindInstance<PSkillInfo>(args[i])?.Copy();
                if (SkillInfo != null) {
                    SkillInfo.Type = FindInstance<PSkillType>(args[i + 1]);
                    SkillInfoList.Add(SkillInfo);
                }
            }
            if (PlayerIndex == PSystem.PlayerIndex) {
                PUIManager.AddNewUIAction("更新技能栏", () => {
                    PUIManager.GetUI<PMapUI>().InitializeSkillButton(SkillInfoList);
                });
            }
            PUIManager.AddNewUIAction("更新武将头像", () => {
                PNetworkManager.NetworkClient.GameStatus.PlayerList[PlayerIndex].General = ListSubTypeInstances<PGeneral>().Find((PGeneral General) => General.Name.Equals(GeneralName));
                PNetworkManager.NetworkClient.GameStatus.PlayerList[PlayerIndex].General.SkillInfoList = SkillInfoList;
                PUIManager.GetUI<PMapUI>().PlayerInformationGroup.GroupUIList[PlayerIndex].UpdateInformation();
            });
        }) {
    }

    public PRefreshGeneralOrder(PPlayer Player) : this() {
        int SkillCount = Player.General.SkillList.Count;
        List<string> Answer = new List<string> { Player.Index.ToString(), Player.General.Name,  SkillCount.ToString() };
        for (int i= 0; i < SkillCount; ++ i) {
            PSkill Skill = Player.General.SkillList[i];
            Answer.Add(Skill.Name);
            if (Skill.SoftLockOpen) {
                if (Skill.Lock) {
                    Answer.Add(PSkillType.SoftLock.Name);
                } else {
                    Answer.Add(PSkillType.SoftLockUnlock.Name);
                }
            } else {
                if (Skill.Lock) {
                    Answer.Add(PSkillType.Lock.Name);
                } else {
                    if (Skill.Initiative) {
                        if (Player.RemainLimitForAlivePlayers(Skill.Name, PNetworkManager.NetworkServer.Game) && Player.RemainLimit(Skill.Name, true)) {
                            Answer.Add(PSkillType.Initiative.Name);
                        } else {
                            Answer.Add(PSkillType.InitiativeInactive.Name);
                        }
                    } else {
                        Answer.Add(PSkillType.Passive.Name);
                    }
                }
            }
        }
        args = Answer.ToArray();
    }
}
