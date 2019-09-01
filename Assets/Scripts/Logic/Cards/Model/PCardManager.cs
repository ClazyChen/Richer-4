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

    public void ThrowAll(PPlayerCardArea Area) {
        for (int i = Area.HandCardArea.CardNumber - 1; i >= 0; --i) {
            MoveCard(Area.HandCardArea.CardList[i], Area.HandCardArea, ThrownCardHeap);
        }
        for (int i = Area.EquipmentCardArea.CardNumber - 1; i >= 0; --i) {
            MoveCard(Area.EquipmentCardArea.CardList[i], Area.EquipmentCardArea, ThrownCardHeap);
        }
        for (int i = Area.JudgeCardArea.CardNumber - 1; i >= 0; --i) {
            MoveCard(Area.JudgeCardArea.CardList[i], Area.JudgeCardArea, ThrownCardHeap);
        }
    }

    public void MoveCard(PCard Card, PCardArea Source, PCardArea Destination) {
        PLogger.Log("["+Card.Name + "]将要从[" + Source.Name + "]移动到[" + Destination.Name + "]");
        if (Source.CardList.Contains(Card)) {
            PMoveCardTag MoveCardTag = Game.Monitor.CallTime(PTime.Card.LeaveAreaTime, new PMoveCardTag(Card, Source, Destination));
            MoveCardTag.Source.CardList.Remove(Card);
            if (MoveCardTag.Source.Owner != null) {
                if (MoveCardTag.Source.IsHandCardArea()) {
                    PNetworkManager.NetworkServer.TellClient(MoveCardTag.Source.Owner, new PRefreshHandCardsOrder(MoveCardTag.Source.ToStringArray()));
                    PNetworkManager.NetworkServer.TellClients(new PRefreshHandCardNumberOrder(MoveCardTag.Source.Owner.Index.ToString(), MoveCardTag.Source.CardNumber.ToString()));
                }
            }
            Game.Monitor.CallTime(PTime.Card.EnterAreaTime, MoveCardTag);
            MoveCardTag.Destination.CardList.Add(Card);
            if (MoveCardTag.Destination.Owner != null) {
                if (MoveCardTag.Destination.IsHandCardArea()) {
                    PNetworkManager.NetworkServer.TellClient(MoveCardTag.Destination.Owner, new PRefreshHandCardsOrder(MoveCardTag.Destination.ToStringArray()));
                    PNetworkManager.NetworkServer.TellClients(new PRefreshHandCardNumberOrder(MoveCardTag.Destination.Owner.Index.ToString(), MoveCardTag.Destination.CardNumber.ToString()));
                }
            }
        }
    }

    /// <summary>
    /// 初始化牌堆，将一副新的牌加入到牌堆里并洗牌
    /// </summary>
    public void InitializeCardHeap() {
        Clear();
        PlayerAreaList = new List<PPlayerCardArea>();
        Game.PlayerList.ForEach((PPlayer Player) => {
            PlayerAreaList.Add(new PPlayerCardArea(Player));
        });
        (new List<PCard>() {
            new P_ManTiienKuoHai().Instantiate(),
            new P_ManTiienKuoHai().Instantiate(),
            new P_ManTiienKuoHai().Instantiate(),
            new P_ManTiienKuoHai().Instantiate(),
            new P_ManTiienKuoHai().Instantiate(),
            new P_WeiWeiChiuChao().Instantiate(),
            new P_WeiWeiChiuChao().Instantiate(),
            new P_WeiWeiChiuChao().Instantiate(),
            new P_WeiWeiChiuChao().Instantiate(),
            new P_WeiWeiChiuChao().Instantiate(),
            new P_IITaiLao().Instantiate(),
            new P_IITaiLao().Instantiate(),
            new P_IITaiLao().Instantiate(),
            new P_IITaiLao().Instantiate(),
            new P_IITaiLao().Instantiate(),
            new P_CheevnHuoTaChieh().Instantiate(),
            new P_CheevnHuoTaChieh().Instantiate(),
            new P_CheevnHuoTaChieh().Instantiate(),
            new P_CheevnHuoTaChieh().Instantiate(),
            new P_CheevnHuoTaChieh().Instantiate(),
            new P_ShevngTungChiHsi().Instantiate(),
            new P_ShevngTungChiHsi().Instantiate(),
            new P_ShevngTungChiHsi().Instantiate(),
            new P_ShevngTungChiHsi().Instantiate(),
            new P_ShevngTungChiHsi().Instantiate(),
            new P_WuChungShevngYou().Instantiate(),
            new P_WuChungShevngYou().Instantiate(),
            new P_WuChungShevngYou().Instantiate(),
            new P_WuChungShevngYou().Instantiate(),
            new P_WuChungShevngYou().Instantiate(),
            new P_AnTuCheevnTsaang().Instantiate(),
            new P_AnTuCheevnTsaang().Instantiate(),
            new P_AnTuCheevnTsaang().Instantiate(),
            new P_AnTuCheevnTsaang().Instantiate(),
            new P_AnTuCheevnTsaang().Instantiate(),
            new P_KevAnKuanHuo().Instantiate(),
            new P_KevAnKuanHuo().Instantiate(),
            new P_KevAnKuanHuo().Instantiate(),
            new P_KevAnKuanHuo().Instantiate(),
            new P_KevAnKuanHuo().Instantiate(),
            new P_LiTaiTaaoChiang().Instantiate(),
            new P_LiTaiTaaoChiang().Instantiate(),
            new P_LiTaiTaaoChiang().Instantiate(),
            new P_LiTaiTaaoChiang().Instantiate(),
            new P_LiTaiTaaoChiang().Instantiate()

        }).ForEach((PCard Card) => {
            CardHeap.CardList.Add(Card);
        });
        CardHeap.Wash();
    }

}