using System;
using System.Linq;

/// <summary>
/// 选择命令+选择标题+选择项个数+选择项*N
/// </summary>
/// CR：在MUI中打开选择框
public class PAskOrder : POrder {
    public PAskOrder() : base("ask",
        null,
        (string[] args) => {
            string Title = args[1];
            int OptionNumber = Convert.ToInt32(args[2]);
            string[] Options = new string[OptionNumber];
            string[] ToolTips = new string[OptionNumber];
            for (int i = 0; i < OptionNumber; ++i) {
                Options[i] = args[i + 3];
            }
            bool ToolTipEnabled = true;
            for (int i = 0; i < OptionNumber; ++ i) {
                if (i + 3 + OptionNumber < args.Length) {
                    ToolTips[i] = args[i + 3 + OptionNumber];
                } else {
                    ToolTipEnabled = false;
                    break;
                }
                
            }
            #region 点将卡和手气卡
            if (Title.Contains("点将卡")) {
                // 选将卡特殊判定
                // 必须要有选将卡才启用
                // 否则返回1
                if (PSystem.UserManager.ChooseGeneral == 0) {
                    PNetworkManager.NetworkClient.Send(new PChooseResultOrder("1"));
                    return;
                } 
            }
            if (Title.Contains("手气卡")) {
                if (PSystem.UserManager.Lucky == 0) {
                    PNetworkManager.NetworkClient.Send(new PChooseResultOrder("1"));
                    return;
                }
            }
            if (Title.Equals("点将")) {
                for (int i = 0; i < Options.Length; ++ i) {
                    if (!PSystem.UserManager.GeneralList.Contains(Options[i])) {
                        Options[i] += "[未获得]";
                    }
                }
            }
            #endregion


            PAnimation.AddAnimation("Ask-打开选择框", () => {
                PUIManager.GetUI<PMapUI>().Ask(Title, Options, ToolTipEnabled ? ToolTips : null);
            });
        }) {
    }
    public PAskOrder(string Title, int OptionNumber, string[] Options, string[] ToolTips = null) : this() {
        string[] ToolTipActual = ToolTips == null ? new string[] { } : ToolTips;
        args = new string[] { Title, OptionNumber.ToString() }.Concat(Options).Concat(ToolTipActual).ToArray();
    }
}
