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
        List<PPlayer> SatisfiedPlayerList = Game.PlayerList.FindAll((PPlayer Player) => Player.IsAlive && Condition(Game, Player));
        if (SatisfiedPlayerList.Count == 0) {
            return null;
        }
        if (Cancel) {
            SatisfiedPlayerList.Add(null);
        }
        int ChosenResult = Ask(Chooser, "为" + Title + "选择目标", SatisfiedPlayerList.ConvertAll((PPlayer Player) => Player == null ? "取消" : Player.Name).ToArray());
        return SatisfiedPlayerList[ChosenResult];
    }

    public List<PPlayer> AskForTargetPlayers(PPlayer Chooser, PTrigger.PlayerCondition Condition, string Title) {
        List<PPlayer> PlayerList = new List<PPlayer>();
        for (PPlayer _Player = null; _Player != null || PlayerList.Count == 0; ) {
            _Player = AskForTargetPlayer(Chooser, (PGame Game, PPlayer Player) => {
                return Condition(Game, Player) && !PlayerList.Contains(Player);
            }, Title, PlayerList.Count > 0);
            if (_Player != null) {
                PlayerList.Add(_Player);
            }
        }
        return PlayerList;
    }

    public PCard AskToChooseOwnCard(PPlayer Player, string Title, bool AllowEquipment = true, bool AllowJudge = false) {
        PGame Game = PNetworkManager.NetworkServer.Game;
        Game.TagManager.CreateTag(new PChooseCardTag(Player, null, AllowEquipment, AllowJudge));
        PNetworkManager.NetworkServer.TellClient(Player, new PShowInformationOrder(Title));
        PThread.WaitUntil(() => Game.TagManager.FindPeekTag<PChooseCardTag>(PChooseCardTag.TagName).Card != null);
        return Game.TagManager.PopTag<PChooseCardTag>(PChooseCardTag.TagName).Card;
    }

    public PBlock AskToChooseBlock(PPlayer Player, string Title, Predicate<PBlock> Condition = null) {
        PGame Game = PNetworkManager.NetworkServer.Game;
        PBlock Answer = null;
        Game.TagManager.CreateTag(new PChooseBlockTag(Player, null));
        PNetworkManager.NetworkServer.TellClient(Player, new PShowInformationOrder(Title));
        while (Answer == null) {
            PThread.WaitUntil(() => Game.TagManager.FindPeekTag<PChooseBlockTag>(PChooseBlockTag.TagName).Block != null);
            Answer = Game.TagManager.FindPeekTag<PChooseBlockTag>(PChooseBlockTag.TagName).Block;
            if (Condition != null && !Condition(Answer)) {
                Answer = null;
            }
        }
        return Game.TagManager.PopTag<PChooseBlockTag>(PChooseBlockTag.TagName).Block;
    }

    public PCard AskToChooseOthersCard(PPlayer Player, PPlayer TargetPlayer, string Title, bool AllowJudge = false) {
        PGame Game = PNetworkManager.NetworkServer.Game;
        List<string> Names = new List<string>();
        List<PCard> CardList = new List<PCard>();
        if (TargetPlayer.Area.HandCardArea.CardNumber > 0) {
            Names.Add("手牌");
            CardList.Add(null);
        }
        TargetPlayer.Area.EquipmentCardArea.CardList.ForEach((PCard Card) => {
            Names.Add(Card.Name);
            CardList.Add(Card);
        });
        if (Names.Count < 1) {
            return null;
        }
        int ChosenResult = Ask(Player, Title, Names.ToArray());
        if (CardList[ChosenResult] == null) {
            return TargetPlayer.Area.HandCardArea.RandomCard();
        } else {
            return CardList[ChosenResult];
        }
    }
}