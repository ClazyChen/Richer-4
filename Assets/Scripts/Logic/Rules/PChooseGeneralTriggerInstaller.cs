using System;
using System.Collections.Generic;

public class PChooseGeneralTriggerInstaller : PSystemTriggerInstaller { 
    public PChooseGeneralTriggerInstaller() : base("选将") {
        TriggerList.Add(new PTrigger("选将") {
            IsLocked = true,
            Time = PTime.ChooseGeneralTime,
            Effect = (PGame Game) => {
                List<PGeneral> Generals = new List<PGeneral>();
                if (Game.PlayerNumber == 4) {
                    Generals = new List<PGeneral>() { new P_LianPo(), new P_PanYue(), new P_ChenYuanYuan(), new P_YangYuHuan() };
                    PMath.Wash(Generals);
                }

                Game.Traverse((PPlayer Player) => {
                    if (Game.PlayerNumber == 4) {
                        Player.General = Generals[Player.Index];
                    } else {
                        if (PMath.RandTest(0.25) && !Game.Teammates(Player).Exists((PPlayer _Player) => _Player.General is P_LianPo)) {
                            Player.General = new P_LianPo();
                        } else if (PMath.RandTest(1.0 / 3)) {
                            Player.General = new P_PanYue();
                        } else if (PMath.RandTest(0.5)) {
                            Player.General = new P_ChenYuanYuan();
                        } else {
                            Player.General = new P_YangYuHuan();
                        }
                    }

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