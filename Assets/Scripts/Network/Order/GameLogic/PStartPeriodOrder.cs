using System;
using UnityEngine.UI;
/// <summary>
/// 开始阶段命令+开始阶段的玩家编号+开始的阶段名
/// </summary>
/// CR：刷新阶段
public class PStartPeriodOrder : POrder {
    private class Config {
        public const float ChangePeriodTime = 0.2f;
    }

    public PStartPeriodOrder() : base("start_period",
        null,
        (string[] args) => {
            int NowPlayerIndex = Convert.ToInt32(args[1]);
            PPeriod Peroid = FindInstance<PPeriod>(args[2]);
            if (Peroid != null) {
                PNetworkManager.NetworkClient.GameStatus.NowPeriod = Peroid;
                PAnimation.AddAnimation("切换阶段", () => {
                    PUIManager.GetUI<PMapUI>().PlayerInformationGroup.GroupUIList.ForEach((PPlayerInformationBox Box) => {
                        Box.PeriodText.gameObject.SetActive(false);
                    });
                    Text CurrentPeriodText = PUIManager.GetUI<PMapUI>().PlayerInformationGroup.GroupUIList[NowPlayerIndex].PeriodText;
                    CurrentPeriodText.text = Peroid.IsFreeTime() ? "空闲时间点" : Peroid.Name;
                    CurrentPeriodText.gameObject.SetActive(true);
                    PUIManager.GetUI<PMapUI>().EndFreeTimeButton.interactable = Peroid.IsFreeTime() && NowPlayerIndex == PSystem.PlayerIndex;
                }, 2, Config.ChangePeriodTime);
            }
        }) {
    }

    public PStartPeriodOrder(string _NowPlayerIndex, string PeriodName) : this() {
        args = new string[] { _NowPlayerIndex , PeriodName};
    }
}
