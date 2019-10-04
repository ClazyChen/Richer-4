public class PCardArch : PArch {
    public class PHuszTag : PTag {
        public static string TagName = "护花使者记录标记";
        public static string UserFieldName = "调虎离山的使用者";
        public PHuszTag(PPlayer User) : base(TagName) {
            AppendField(UserFieldName, User);
            Visible = false;
        }
        public PPlayer User {
            get {
                return GetField<PPlayer>(UserFieldName, null);
            }
        }
    }
    public class PZhuangfmsTag : PTag {
        public static string TagName = "装疯卖傻记录标记";
        public static string UserFieldName = "假痴不癫的使用者";
        public PZhuangfmsTag(PPlayer User) : base(TagName) {
            AppendField(UserFieldName, User);
            Visible = false;
        }
        public PPlayer User {
            get {
                return GetField<PPlayer>(UserFieldName, null);
            }
        }
    }
    public class PCardInjureTag : PTag {
        public static string TagName = "卡牌造成伤害记录标记";
        public static string CardFieldName = "造成伤害的卡牌";
        public static string TotalInjureFieldName = "累计造成的伤害量";
        public PCardInjureTag(PCard Card, int TotalInjure) : base(TagName + Card.Name) {
            AppendField(CardFieldName, Card);
            AppendField(TotalInjureFieldName, TotalInjure);
            Visible = false;
        }
        public PCard Card {
            get {
                return GetField<PCard>(CardFieldName, null);
            }
        }
        public int TotalInjure {
            get {
                return GetField(TotalInjureFieldName, 0);
            }
            set {
                SetField(TotalInjureFieldName, value);
            }
        }
    }


    public PCardArch() : base("卡牌类成就") {
        TriggerList.Add(new PTrigger("吃掉电脑屏幕") {
            IsLocked = true,
            Time = PTime.EndGameTime,
            Effect = (PGame Game) => {
                Game.GetWinner().ForEach((PPlayer Player) => {
                    if (Player.Area.HandCardArea.CardNumber >= 17) {
                        Announce(Game, Player, "吃掉电脑屏幕");
                    }
                });
            }
        });
        TriggerList.Add(new PTrigger("海天一色") {
            IsLocked = true,
            Time = PTime.DieTime,
            Effect = (PGame Game) => {
                PDyingTag DyingTag = Game.TagManager.FindPeekTag<PDyingTag>(PDyingTag.TagName);
                PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                if (InjureTag != null && DyingTag != null && DyingTag.Player.Equals(InjureTag.ToPlayer) && InjureTag.InjureSource is PCard && ((PCard)InjureTag.InjureSource).Model is P_ManTiienKuoHai) {
                    PUseCardTag UseCardTag = Game.TagManager.FindPeekTag<PUseCardTag>(PUseCardTag.TagName);
                    if (UseCardTag != null && UseCardTag.User != null && UseCardTag.Card.Equals(InjureTag.InjureSource)) {
                        Announce(Game, UseCardTag.User, "海天一色");
                    }
                }
            }
        });
        TriggerList.Add(new PTrigger("口蜜腹剑") {
            IsLocked = true,
            Time = PTime.DieTime,
            Effect = (PGame Game) => {
                PDyingTag DyingTag = Game.TagManager.FindPeekTag<PDyingTag>(PDyingTag.TagName);
                PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                if (InjureTag != null && DyingTag != null && DyingTag.Player.Equals(InjureTag.ToPlayer) && InjureTag.InjureSource is PCard && ((PCard)InjureTag.InjureSource).Model is P_HsiaoLiTsaangTao) {
                    PUseCardTag UseCardTag = Game.TagManager.FindPeekTag<PUseCardTag>(PUseCardTag.TagName);
                    if (UseCardTag != null && UseCardTag.User != null && UseCardTag.Card.Equals(InjureTag.InjureSource)) {
                        Announce(Game, UseCardTag.User, "口蜜腹剑");
                    }
                }
            }
        });
        #region 出其不意
        string Chuqby = "出其不意";
        TriggerList.Add(new PTrigger("出其不意[初始化]") {
            IsLocked = true,
            Time = PPeriod.StartTurn.During,
            Effect = (PGame Game) => {
                Game.NowPlayer.Tags.CreateTag(new PUsedTag(Chuqby, 1));
            }
        });
        TriggerList.Add(new PTrigger("出其不意[使用暗度陈仓]") {
            IsLocked = true,
            Time = PTime.Card.EndSettleTime,
            Condition = (PGame Game) => {
                PUseCardTag UseCardTag = Game.TagManager.FindPeekTag<PUseCardTag>(PUseCardTag.TagName);
                return UseCardTag.TargetList.Contains(Game.NowPlayer) && UseCardTag.Card.Model is P_AnTuCheevnTsaang;
            },
            Effect = (PGame Game) => {
                Game.NowPlayer.Tags.FindPeekTag<PUsedTag>(PUsedTag.TagNamePrefix + Chuqby).Count = 1;
            }
        });
        TriggerList.Add(new PTrigger("出其不意") {
            IsLocked = true,
            Time = PTime.PurchaseLandTime,
            Condition = (PGame Game) => {
                PPurchaseLandTag PurchaseLandTag = Game.TagManager.FindPeekTag<PPurchaseLandTag>(PPurchaseLandTag.TagName);
                return PurchaseLandTag.Player.Equals(Game.NowPlayer) && PurchaseLandTag.Block.IsBusinessLand && Game.NowPlayer.Tags.FindPeekTag<PUsedTag>(PUsedTag.TagNamePrefix + Chuqby).Count > 0;
            },
            Effect = (PGame Game) => {
                Announce(Game, Game.NowPlayer, "出其不意");
            }
        });
        #endregion
        #region 百足之虫
        string Baizzc = "百足之虫";
        MultiPlayerTriggerList.Add((PPlayer Player) =>
        new PTrigger("百足之虫[初始化]") {
            IsLocked = true,
            Time = PTime.StartGameTime,
            Effect = (PGame Game) => {
                Player.Tags.CreateTag(new PUsedTag(Baizzc, 2));
            }
        });
        MultiPlayerTriggerList.Add((PPlayer Player) => 
        new PTrigger("百足之虫[使用借尸还魂]") {
            IsLocked = true,
            Time = PTime.Card.EndSettleTime,
            Condition = (PGame Game) => {
                PUseCardTag UseCardTag = Game.TagManager.FindPeekTag<PUseCardTag>(PUseCardTag.TagName);
                return UseCardTag.TargetList.Contains(Player) && UseCardTag.Card.Model is P_ChiehShihHuanHun;
            },
            Effect = (PGame Game) => {
                PUsedTag UsedTag = Player.Tags.FindPeekTag<PUsedTag>(PUsedTag.TagNamePrefix + Baizzc);
                UsedTag.Count++;
                if (UsedTag.Count >= UsedTag.Limit) {
                    Announce(Game, Player, Baizzc);
                }
            }
        });
        #endregion
        #region 护花使者
        string Huhsz = "护花使者";
        MultiPlayerTriggerList.Add((PPlayer Player) =>
        new PTrigger("护花使者[初始化]") {
            IsLocked = true,
            Time = PPeriod.StartTurn.Start,
            Effect = (PGame Game) => {
                Player.Tags.PopTag<PHuszTag>(PHuszTag.TagName);
            }
        });
        TriggerList.Add(new PTrigger("护花使者[使用调虎离山]") {
            IsLocked = true,
            Time = PTime.Card.EndSettleTime,
            Condition = (PGame Game) => {
                PUseCardTag UseCardTag = Game.TagManager.FindPeekTag<PUseCardTag>(PUseCardTag.TagName);
                return UseCardTag.Card.Model is P_TiaoHuLiShan;
            },
            Effect = (PGame Game) => {
                PUseCardTag UseCardTag = Game.TagManager.FindPeekTag<PUseCardTag>(PUseCardTag.TagName);
                UseCardTag.TargetList.ForEach((PPlayer Player) => {
                    if (!Player.Tags.ExistTag(PHuszTag.TagName) && Player.TeamIndex == UseCardTag.User.TeamIndex) {
                        Player.Tags.CreateTag(new PHuszTag(UseCardTag.User));
                    }
                });
            }
        });
        TriggerList.Add(new PTrigger("护花使者") {
            IsLocked = true,
            Time = PTime.Injure.StartSettle,
            Condition = (PGame Game) => {
                PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                return InjureTag.ToPlayer != null && InjureTag.ToPlayer.Tags.ExistTag(PHuszTag.TagName);
            },
            Effect= (PGame Game) => {
                PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                Announce(Game, InjureTag.ToPlayer.Tags.FindPeekTag<PHuszTag>(PHuszTag.TagName).User, Huhsz);
            }
        });
        #endregion
        TriggerList.Add(new PTrigger("搬石砸脚") {
            IsLocked = true,
            Time = PTime.Card.EndSettleTime,
            Effect = (PGame Game) => {
                PUseCardTag UseCardTag = Game.TagManager.FindPeekTag<PUseCardTag>(PUseCardTag.TagName);
                if (UseCardTag.Card.Model is P_PaaoChuanYinYoo &&
                UseCardTag.TargetList.TrueForAll((PPlayer Player) => Player.TeamIndex == UseCardTag.User.TeamIndex)) {
                    Announce(Game, UseCardTag.User, "搬石砸脚");
                }
            }
        });
        #region 水至清则无鱼
        string Shuizqzwy = "水至清则无鱼";
        TriggerList.Add(new PTrigger("水至清则无鱼[初始化]") {
            IsLocked = true,
            Time = PTime.Card.AfterEmitTargetTime,
            Effect = (PGame Game) => {
                PUseCardTag UseCardTag = Game.TagManager.FindPeekTag<PUseCardTag>(PUseCardTag.TagName);
                if (UseCardTag.Card.Model is P_HunShuiMoYoo) {
                    UseCardTag.User.Tags.CreateTag(new PCardInjureTag(UseCardTag.Card, 0));
                }
            }
        });
        TriggerList.Add(new PTrigger("水至清则无鱼[增量计算]") {
            IsLocked = true,
            Time = PTime.Injure.EndSettle,
            Effect = (PGame Game) => {
                PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                if (InjureTag.Injure > 0 && InjureTag.FromPlayer != null && 
                InjureTag.InjureSource is PCard && ((PCard)InjureTag.InjureSource).Model is P_HunShuiMoYoo &&
                InjureTag.FromPlayer.Tags.ExistTag(PCardInjureTag.TagName + P_HunShuiMoYoo.CardName)) {
                    InjureTag.FromPlayer.Tags.FindPeekTag<PCardInjureTag>(PCardInjureTag.TagName + P_HunShuiMoYoo.CardName).TotalInjure += InjureTag.Injure;
                }
            }
        });
        TriggerList.Add(new PTrigger("水至清则无鱼") {
            IsLocked = true,
            Time = PTime.Card.EndSettleTime,
            Effect = (PGame Game) => {
                PUseCardTag UseCardTag = Game.TagManager.FindPeekTag<PUseCardTag>(PUseCardTag.TagName);
                if (UseCardTag.Card.Model is P_HunShuiMoYoo && UseCardTag.User.Tags.ExistTag(PCardInjureTag.TagName + P_HunShuiMoYoo.CardName)) {
                    PCardInjureTag CardInjureTag = UseCardTag.User.Tags.PopTag<PCardInjureTag>(PCardInjureTag.TagName + P_HunShuiMoYoo.CardName);
                    if (CardInjureTag.TotalInjure >= 3500) {
                        Announce(Game, UseCardTag.User, Shuizqzwy);
                    }
                }
            }
        });
        #endregion
        #region 逃出生天
        string Taocst = "逃出生天";
        TriggerList.Add(new PTrigger("逃出生天[初始化]") {
            IsLocked = true,
            Time = PPeriod.StartTurn.During,
            Effect = (PGame Game) => {
                Game.NowPlayer.Tags.CreateTag(new PUsedTag(Taocst, 1));
            }
        });
        TriggerList.Add(new PTrigger("逃出生天[使用金蝉脱壳]") {
            IsLocked = true,
            Time = PTime.Card.EndSettleTime,
            Condition = (PGame Game) => {
                PUseCardTag UseCardTag = Game.TagManager.FindPeekTag<PUseCardTag>(PUseCardTag.TagName);
                return Game.NowPlayer.Equals(UseCardTag.User) && UseCardTag.Card.Model is P_ChinChaanToowChiiao;
            },
            Effect = (PGame Game) => {
                Game.NowPlayer.Tags.FindPeekTag<PUsedTag>(PUsedTag.TagNamePrefix + Taocst).Count = 1;
            }
        });
        TriggerList.Add(new PTrigger("逃出生天") {
            IsLocked = true,
            Time = PPeriod.SettleStage.Before,
            Condition = (PGame Game) => {
                return Game.NowPlayer.Tags.FindPeekTag<PUsedTag>(PUsedTag.TagNamePrefix + Taocst).Count > 0 &&
                (Game.NowPlayer.Position.GetMoneyStopPercent > 0 || Game.NowPlayer.Position.GetMoneyStopSolid > 0);
            },
            Effect = (PGame Game) => {
                Announce(Game, Game.NowPlayer, Taocst);
            }
        });
        #endregion
        #region 无处可逃
        string Wuckt = "无处可逃";
        TriggerList.Add(new PTrigger("无处可逃[初始化]") {
            IsLocked = true,
            Time = PTime.Card.AfterEmitTargetTime,
            Effect = (PGame Game) => {
                PUseCardTag UseCardTag = Game.TagManager.FindPeekTag<PUseCardTag>(PUseCardTag.TagName);
                if (UseCardTag.Card.Model is P_KuanMevnChoTsev) {
                    UseCardTag.User.Tags.CreateTag(new PCardInjureTag(UseCardTag.Card, 0));
                }
            }
        });
        TriggerList.Add(new PTrigger("无处可逃[增量计算]") {
            IsLocked = true,
            Time = PTime.Injure.EndSettle,
            Effect = (PGame Game) => {
                PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                if (InjureTag.Injure > 0 && InjureTag.FromPlayer != null &&
                InjureTag.InjureSource is PCard && ((PCard)InjureTag.InjureSource).Model is P_KuanMevnChoTsev &&
                InjureTag.FromPlayer.Tags.ExistTag(PCardInjureTag.TagName + P_KuanMevnChoTsev.CardName)) {
                    InjureTag.FromPlayer.Tags.FindPeekTag<PCardInjureTag>(PCardInjureTag.TagName + P_KuanMevnChoTsev.CardName).TotalInjure += InjureTag.Injure;
                }
            }
        });
        TriggerList.Add(new PTrigger("无处可逃") {
            IsLocked = true,
            Time = PTime.Card.EndSettleTime,
            Effect = (PGame Game) => {
                PUseCardTag UseCardTag = Game.TagManager.FindPeekTag<PUseCardTag>(PUseCardTag.TagName);
                if (UseCardTag.Card.Model is P_KuanMevnChoTsev && UseCardTag.User.Tags.ExistTag(PCardInjureTag.TagName + P_KuanMevnChoTsev.CardName)) {
                    PCardInjureTag CardInjureTag = UseCardTag.User.Tags.PopTag<PCardInjureTag>(PCardInjureTag.TagName + P_KuanMevnChoTsev.CardName);
                    if (CardInjureTag.TotalInjure >= 7000) {
                        Announce(Game, UseCardTag.User, Wuckt);
                    }
                }
            }
        });
        #endregion
        TriggerList.Add(new PTrigger("不安好心") {
            IsLocked = true,
            Time = PTime.DieTime,
            Effect = (PGame Game) => {
                PDyingTag DyingTag = Game.TagManager.FindPeekTag<PDyingTag>(PDyingTag.TagName);
                PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                if (InjureTag != null && DyingTag != null && DyingTag.Player.Equals(InjureTag.ToPlayer) && InjureTag.InjureSource is PCard && ((PCard)InjureTag.InjureSource).Model is P_ChiaTaoFaKuo) {
                    PUseCardTag UseCardTag = Game.TagManager.FindPeekTag<PUseCardTag>(PUseCardTag.TagName);
                    if (UseCardTag != null && UseCardTag.User != null && UseCardTag.Card.Equals(InjureTag.InjureSource)) {
                        Announce(Game, UseCardTag.User, "不安好心");
                    }
                }
            }
        });
        #region 装疯卖傻
        string Zhuangfms = "装疯卖傻";
        MultiPlayerTriggerList.Add((PPlayer Player) =>
        new PTrigger("装疯卖傻[初始化]") {
            IsLocked = true,
            Time = PPeriod.StartTurn.Start,
            Effect = (PGame Game) => {
                Player.Tags.PopTag<PZhuangfmsTag>(PZhuangfmsTag.TagName);
            }
        });
        TriggerList.Add(new PTrigger("装疯卖傻[使用假痴不癫]") {
            IsLocked = true,
            Time = PTime.Card.EndSettleTime,
            Condition = (PGame Game) => {
                PUseCardTag UseCardTag = Game.TagManager.FindPeekTag<PUseCardTag>(PUseCardTag.TagName);
                return UseCardTag.Card.Model is P_ChiaChiihPuTien;
            },
            Effect = (PGame Game) => {
                PUseCardTag UseCardTag = Game.TagManager.FindPeekTag<PUseCardTag>(PUseCardTag.TagName);
                UseCardTag.TargetList.ForEach((PPlayer Player) => {
                    if (!Player.Tags.ExistTag(PZhuangfmsTag.TagName) && Player.TeamIndex == UseCardTag.User.TeamIndex) {
                        Player.Tags.CreateTag(new PZhuangfmsTag(UseCardTag.User));
                    }
                });
            }
        });
        TriggerList.Add(new PTrigger("装疯卖傻") {
            IsLocked = true,
            Time = PTime.Injure.StartSettle,
            Condition = (PGame Game) => {
                PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                return InjureTag.ToPlayer != null && InjureTag.ToPlayer.Tags.ExistTag(PZhuangfmsTag.TagName);
            },
            Effect = (PGame Game) => {
                PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                Announce(Game, InjureTag.ToPlayer.Tags.FindPeekTag<PZhuangfmsTag>(PZhuangfmsTag.TagName).User, Zhuangfms);
            }
        });
        #endregion
    }
}