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
