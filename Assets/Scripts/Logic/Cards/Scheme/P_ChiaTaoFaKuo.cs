using System;
using System.Collections.Generic;
/// <summary>
/// 假道伐虢
/// </summary>

public class PChiaTaoFaKuoTag : PTag {
    public static string TagName = "假道伐虢记录标记";
    public static string LordListFieldName = "沿途的其他领主";
    public PChiaTaoFaKuoTag(List<PPlayer> LordList) : base(TagName) {
        AppendField(LordListFieldName, LordList);
    }
    public List<PPlayer> LordList {
        get {
            return GetField<List<PPlayer>>(LordListFieldName, null);
        }
    }
}

public class PChiaTaoFaKuoTriggerInstaller : PSystemTriggerInstaller {

    public PChiaTaoFaKuoTriggerInstaller() : base("假道伐虢的记录") {
        TriggerList.Add(new PTrigger("假道伐虢[开始记录]") {
            IsLocked = true,
            Time = PPeriod.StartTurn.During,
            Effect = (PGame Game) => {
                Game.TagManager.PopTag<PChiaTaoFaKuoTag>(PChiaTaoFaKuoTag.TagName);
                Game.TagManager.CreateTag(new PChiaTaoFaKuoTag(new List<PPlayer>()));
            }
        });
        TriggerList.Add(new PTrigger("假道伐虢[经过土地]") {
            IsLocked = true,
            Time = PTime.MovePositionTime,
            Effect = (PGame Game) => {
                PTransportTag TransportTag = Game.TagManager.FindPeekTag<PTransportTag>(PTransportTag.TagName);
                PPlayer Lord = TransportTag.Destination.Lord;
                if (Lord != null && !Lord.Equals(Game.NowPlayer)) {
                    PChiaTaoFaKuoTag ChiaTaoFaKuoTag = Game.TagManager.FindPeekTag<PChiaTaoFaKuoTag>(PChiaTaoFaKuoTag.TagName);
                    if (!ChiaTaoFaKuoTag.LordList.Contains(Lord)) {
                        ChiaTaoFaKuoTag.LordList.Add(Lord);
                    }
                }
            }
        });
    }
}

public class P_ChiaTaoFaKuo : PSchemeCardModel {
    private List<PPlayer> AIEmitTargets(PGame Game, PPlayer Player) {
        return new List<PPlayer> { PAiTargetChooser.InjureTarget(Game, Player, (PGame _Game, PPlayer _Player) => {
            PChiaTaoFaKuoTag ChiaTaoFaKuoTag = _Game.TagManager.FindPeekTag<PChiaTaoFaKuoTag>(PChiaTaoFaKuoTag.TagName);
            return ChiaTaoFaKuoTag.LordList.Contains(_Player) && Player.TeamIndex != _Player.TeamIndex;
        }, 1000, true)};
    }

    public override int AIInHandExpectation(PGame Game, PPlayer Player) {
        int Basic = 1000;
        if (Game.PlayerList.FindAll((PPlayer _Player) => _Player.IsAlive).Count <= 2) {
            return 0;
        } 
        return Basic;
    }

    public readonly static string CardName = "假道伐虢";

    public P_ChiaTaoFaKuo():base(CardName) {
        Point = 4;
        Index = 24;
        foreach (PTime Time in new PTime[] {
            PPeriod.EndTurnStage.During
        }) {
            MoveInHandTriggerList.Add((PPlayer Player, PCard Card) => {
                return new PTrigger(CardName) {
                    IsLocked = false,
                    Player = Player,
                    Time = Time,
                    AIPriority = 100,
                    Condition = (PGame Game) => {
                        PChiaTaoFaKuoTag ChiaTaoFaKuoTag = Game.TagManager.FindPeekTag<PChiaTaoFaKuoTag>(PChiaTaoFaKuoTag.TagName);
                        return Player.Equals(Game.NowPlayer) && ChiaTaoFaKuoTag.LordList.Count >= 2;
                    },
                    AICondition = (PGame Game) => {
                        return AIEmitTargets(Game, Player)[0] != null && !Player.OutOfGame;
                    },
                    Effect = MakeNormalEffect(Player, Card, AIEmitTargets, (PGame _Game, PPlayer _Player) => {
                        PChiaTaoFaKuoTag ChiaTaoFaKuoTag = _Game.TagManager.FindPeekTag<PChiaTaoFaKuoTag>(PChiaTaoFaKuoTag.TagName);
                        return ChiaTaoFaKuoTag.LordList.Contains(_Player);
                    },
                        (PGame Game, PPlayer User, PPlayer Target) => {
                            Game.Injure(User, Target, 1000, Card);
                        })
                };
            });
        }
    }
}