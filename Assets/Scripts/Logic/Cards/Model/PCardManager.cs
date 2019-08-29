using System.Collections.Generic;

/// <summary>
/// PCardManager类：牌的管理类
/// </summary>
public class PCardManager {
    private readonly PGame Game;

    public List<PPlayerCardArea> PlayerAreaList;
    public PCardArea CardHeap;
    public PCardArea ThrownCardHeap;
    public PCardArea SettlingArea;

    public PCardManager(PGame _Game) {
        Game = _Game;
        PlayerAreaList = new List<PPlayerCardArea>();
        Game.PlayerList.ForEach((PPlayer Player) => {
            PlayerAreaList.Add(new PPlayerCardArea(Player));
        });
        CardHeap = new PCardArea("牌堆");
        ThrownCardHeap = new PCardArea("弃牌堆");
        SettlingArea = new PCardArea("结算区");
    }

    public void Clear() {
        PlayerAreaList.ForEach((PPlayerCardArea Area) => Area.Clear());
        CardHeap.CardList.Clear();
        ThrownCardHeap.CardList.Clear();
        SettlingArea.CardList.Clear();
    }

    /// <summary>
    /// 初始化牌堆，将一副新的牌加入到牌堆里并洗牌
    /// </summary>
    public void InitializeCardHeap() {
        (new List<PCard>() {

        }).ForEach((PCard Card) => {
            CardHeap.CardList.Add(Card);
        });
        CardHeap.Wash();
    }

}