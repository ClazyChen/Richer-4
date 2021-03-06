﻿using System;
using System.Collections.Generic;

public class PCardTriggerInstaller : PSystemTriggerInstaller {
    public class Config {
        public static int StartGameCardCount = 3;
    }

    public PCardTriggerInstaller() : base("卡牌移入移出区域") {
        TriggerList.Add(new PTrigger("游戏开始时摸牌") {
            IsLocked = true,
            Time = PTime.StartGameTime,
            Effect = (PGame Game) => {
                Game.PlayerList.ForEach((PPlayer Player) => {
                    Game.GetCard(Player, Config.StartGameCardCount);
                    int LuckyCardCount = 0;
                    while (LuckyCardCount < 3 && Player.IsUser && PNetworkManager.NetworkServer.ChooseManager.AskYesOrNo(Player, "是否使用手气卡？剩余次数=" + (3 - LuckyCardCount))) {
                        Game.CardManager.ThrowAll(Player.Area);
                        LuckyCardCount++;
                        Game.GetCard(Player, Config.StartGameCardCount);
                    }
                });
            }
        });
        TriggerList.Add(new PTrigger("卡牌移入手牌装载触发器") {
            IsLocked = true,
            Time = PTime.Card.EnterAreaTime,
            Condition = (PGame Game) => {
                PMoveCardTag MoveTagFlag = Game.TagManager.FindPeekTag<PMoveCardTag>(PMoveCardTag.TagName);
                return MoveTagFlag.Destination.IsHandCardArea();
            },
            Effect = (PGame Game) => {
                PMoveCardTag MoveTagFlag = Game.TagManager.FindPeekTag<PMoveCardTag>(PMoveCardTag.TagName);
                PPlayer Accepter = MoveTagFlag.Destination.Owner;
                MoveTagFlag.Card.MoveInHandTriggerList = MoveTagFlag.Card.Model.MoveInHandTriggerList.ConvertAll((Func<PPlayer, PCard, PTrigger> Trigger) => Trigger(Accepter, MoveTagFlag.Card));
                MoveTagFlag.Card.MoveInHandTriggerList.ForEach((PTrigger Trigger) => {
                    Game.Monitor.AddTrigger(Trigger);
                });
            }
        });
        TriggerList.Add(new PTrigger("卡牌移出手牌摘下触发器") {
            IsLocked = true,
            Time = PTime.Card.LeaveAreaTime,
            Condition = (PGame Game) => {
                PMoveCardTag MoveTagFlag = Game.TagManager.FindPeekTag<PMoveCardTag>(PMoveCardTag.TagName);
                return MoveTagFlag.Source.IsHandCardArea();
            },
            Effect = (PGame Game) => {
                PMoveCardTag MoveTagFlag = Game.TagManager.FindPeekTag<PMoveCardTag>(PMoveCardTag.TagName);
                PPlayer Giver = MoveTagFlag.Source.Owner;
                MoveTagFlag.Card.MoveInHandTriggerList.ForEach((PTrigger Trigger) => {
                    Game.Monitor.RemoveTrigger(Trigger);
                });
            }
        });
        TriggerList.Add(new PTrigger("卡牌移入装备区装载触发器") {
            IsLocked = true,
            Time = PTime.Card.EnterAreaTime,
            Condition = (PGame Game) => {
                PMoveCardTag MoveTagFlag = Game.TagManager.FindPeekTag<PMoveCardTag>(PMoveCardTag.TagName);
                return MoveTagFlag.Destination.IsEquipmentArea();
            },
            Effect = (PGame Game) => {
                PMoveCardTag MoveTagFlag = Game.TagManager.FindPeekTag<PMoveCardTag>(PMoveCardTag.TagName);
                PPlayer Accepter = MoveTagFlag.Destination.Owner;
                MoveTagFlag.Card.MoveInEquipTriggerList = MoveTagFlag.Card.Model.MoveInEquipTriggerList.ConvertAll((Func<PPlayer, PCard, PTrigger> Trigger) => Trigger(Accepter, MoveTagFlag.Card));
                MoveTagFlag.Card.MoveInEquipTriggerList.ForEach((PTrigger Trigger) => {
                    Game.Monitor.AddTrigger(Trigger);
                });
            }
        });
        TriggerList.Add(new PTrigger("卡牌移出装备区摘下触发器") {
            IsLocked = true,
            Time = PTime.Card.LeaveAreaTime,
            Condition = (PGame Game) => {
                PMoveCardTag MoveTagFlag = Game.TagManager.FindPeekTag<PMoveCardTag>(PMoveCardTag.TagName);
                return MoveTagFlag.Source.IsEquipmentArea();
            },
            Effect = (PGame Game) => {
                PMoveCardTag MoveTagFlag = Game.TagManager.FindPeekTag<PMoveCardTag>(PMoveCardTag.TagName);
                PPlayer Giver = MoveTagFlag.Source.Owner;
                MoveTagFlag.Card.MoveInEquipTriggerList.ForEach((PTrigger Trigger) => {
                    Game.Monitor.RemoveTrigger(Trigger);
                });
            }
        });
        TriggerList.Add(new PTrigger("通知客户端卡牌指定目标") {
            IsLocked = true,
            Time = PTime.Card.AfterEmitTargetTime,
            Effect = (PGame Game) => {
                PUseCardTag UseCardTag = Game.TagManager.FindPeekTag<PUseCardTag>(PUseCardTag.TagName);
                PPlayer User = UseCardTag.User;
                PCard Card = UseCardTag.Card;
                List<PPlayer> TargetList = UseCardTag.TargetList;
                if (User != null && Card != null && TargetList != null && !TargetList.Contains(null)) {
                    PNetworkManager.NetworkServer.TellClients(new PPushTextOrder(User.Index.ToString(), Card.Model.Name, PPushType.Information.Name));
                    if (Card.Name.Equals(Card.Model.Name)) {
                        PNetworkManager.NetworkServer.TellClients(new PShowInformationOrder(User.Name + "使用了" + Card.Name));
                    } else {
                        PNetworkManager.NetworkServer.TellClients(new PShowInformationOrder(User.Name + "把" + Card.Name + "当做" + Card.Model.Name + "使用"));
                    }
                    PNetworkManager.NetworkServer.TellClients(new PShowInformationOrder("目标：" + string.Join(",", TargetList.ConvertAll((PPlayer Player) => Player.Name))));
                }
            }
        });
    }
}