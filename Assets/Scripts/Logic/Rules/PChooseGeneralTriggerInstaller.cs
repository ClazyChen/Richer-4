using System;
public class PChooseGeneralTriggerInstaller : PSystemTriggerInstaller { 
    public PChooseGeneralTriggerInstaller() : base("选将") {
        TriggerList.Add(new PTrigger("选将") {
            IsLocked = true,
            Time = PTime.StartGameTime,
            Effect = (PGame Game) => {
                Game.Traverse((PPlayer Player) => {
                    if (PMath.RandTest(0.25) && !Game.Teammates(Player).Exists((PPlayer _Player) => _Player.General is P_LianPo)) {
                        Player.General = new P_LianPo();
                    } else if (PMath.RandTest(1.0/3)) {
                        Player.General = new P_PanYue();
                    } else if (PMath.RandTest(0.5)) {
                        Player.General = new P_ChenYuanYuan();
                    } else {
                        Player.General = new P_YangYuHuan();
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