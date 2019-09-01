using System;
/// <summary>
/// PTrigger类：
/// 用来实现一个挂在时机上的触发器
/// </summary>
public class PTrigger : PObject{
    public static Action<PGame> DefaultAction = (PGame Game) => { };
    public static Predicate<PGame> DefaultCondition = (PGame Game) => true;

    /// <summary>
    /// 是否必须发动
    /// </summary>
    public bool IsLocked = false;
    /// <summary>
    /// 触发该操作的玩家（用于规定结算次序）
    /// null表示系统操作，优先级最高
    /// </summary>
    public PPlayer Player = null;
    public PTime Time = null; // 发动时机
    public Action<PGame> Effect = DefaultAction;
    public Predicate<PGame> Condition = DefaultCondition;
    public Predicate<PGame> AICondition = DefaultCondition;
    public int AIPriority = 1; // 权重大的效果AI优先发动
    public bool CanRepeat = false; // 是否可以被重复发动

    public delegate bool PlayerCondition(PGame Game, PPlayer Player);

    public PTrigger(string _Name) {
        Name = _Name;
    }

    public static int ComparePriority(PGame Game, PTrigger x, PTrigger y) {
        int SettleIndex(PPlayer Player) {
            if (Player == null) {
                return -1;
            }
            return Player.Index + (Player.Index < Game.NowPlayerIndex ? Game.PlayerNumber : 0);
        }
        int XPlayerIndex = SettleIndex(x.Player);
        int YPlayerIndex = SettleIndex(y.Player);
        if (XPlayerIndex < YPlayerIndex) {
            return -1;
        } else if (XPlayerIndex > YPlayerIndex) {
            return 1;
        } else if (x.Player != null) {
            if (x.Player.IsAI) {
                if (x.AIPriority > y.AIPriority) {
                    return -1;
                } else if (x.AIPriority < y.AIPriority) {
                    return 1;
                } else {
                    return 0;
                }
            } else {
                return 0;
            }
        } else {
            return 0;
        }
    }


    public static readonly Converter<PPlayer, PlayerCondition> Except = (PPlayer Player) => {
        return (PGame _Game, PPlayer _Player) => !Player.Equals(_Player);
    };

    public static readonly PlayerCondition NoCondition = (PGame _Game, PPlayer _Player) => true;
}