using System;
using System.Collections.Generic;
using System.Linq;

public class PChooseManager {
    public int ChosenAnswer;

    public PChooseManager() {
        ChosenAnswer = -1;
    }

    public int Ask(PPlayer Player, string Title, string[] Options) {
        ChosenAnswer = -1;
        PNetworkManager.NetworkServer.TellClient(Player, new PAskOrder(Title, Options.Length, Options));
        PThread.WaitUntil(() => ChosenAnswer >= 0);
        return ChosenAnswer;
    }

    public bool AskYesOrNo(PPlayer Player, string Title) {
        ChosenAnswer = -1;
        PNetworkManager.NetworkServer.TellClient(Player, new PAskOrder(Title, 2, new string[] {
            "YES", "NO"
        }));
        PThread.WaitUntil(() => ChosenAnswer >= 0);
        return ChosenAnswer == 0;
    }

    /// <summary>
    /// 选择一个目标玩家
    /// </summary>
    /// <param name="Chooser">谁来选择</param>
    /// <param name="Condition">筛选目标玩家的函数（无满足结果返回null）</param>
    /// <param name="Title">出现在选择框标题“为Title选择目标”</param>
    /// <param name="Cancel">是否允许取消（取消返回null）</param>
    /// <returns></returns>
    public PPlayer AskForTargetPlayer(PPlayer Chooser, PTrigger.PlayerCondition Condition, string Title, bool Cancel = false) {
        PGame Game = PNetworkManager.NetworkServer.Game;
        List<PPlayer> SatisfiedPlayerList = Game.PlayerList.FindAll((PPlayer Player) => Condition(Game, Player));
        if (SatisfiedPlayerList.Count == 0) {
            return null;
        }
        if (Cancel) {
            SatisfiedPlayerList.Add(null);
        }
        int ChosenResult = Ask(Chooser, "为" + Title + "选择目标", SatisfiedPlayerList.ConvertAll((PPlayer Player) => Player == null ? "取消" : Player.Name).ToArray());
        return SatisfiedPlayerList[ChosenResult];
    }
}