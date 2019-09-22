using System;
using System.Collections.Generic;

public class PChooseGeneralTriggerInstaller : PSystemTriggerInstaller { 
    public PChooseGeneralTriggerInstaller() : base("选将") {
        TriggerList.Add(new PTrigger("选将") {
            IsLocked = true,
            Time = PTime.ChooseGeneralTime,
            Effect = (PGame Game) => {
                List<PGeneral> Generals = new List<PGeneral>();
                Generals = new List<PGeneral>() { new P_LianPo(), new P_PanYue(), new P_ChenYuanYuan(), new P_YangYuHuan(),
                                                      new P_ShiQian(), new P_ZhangSanFeng(), new P_WangXu(), new P_ZhaoYun() };
                PMath.Wash(Generals);

                Game.Traverse((PPlayer Player) => {
                    Player.General = Generals[Player.Index];

                    Player.Age = Player.General.Age;
                    Player.Sex = Player.General.Sex;
                    Player.General.SkillList.ForEach((PSkill Skill) => {
                        Skill.TriggerList.ForEach((Func<PPlayer, PSkill, PTrigger> TriggerGenerator) => {
                            Game.Monitor.AddTrigger(TriggerGenerator(Player, Skill));
                        });
                    });
                    PNetworkManager.NetworkServer.TellClients(new PRefreshGeneralOrder(Player));
                }, Game.PlayerList[0]);
            }
        });
    }
}