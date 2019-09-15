public class PLandArch : PArch { 
    public PLandArch() : base("土地类成就") {
        TriggerList.Add(new PTrigger("地产大亨") {
            IsLocked = true,
            Time = PTime.EndGameTime,
            Effect = (PGame Game) => {
                Game.GetWinner().ForEach((PPlayer Player) => {
                    if (Game.Map.BlockList.FindAll((PBlock Block) => Player.Equals(Block.Lord)).Count >= 15) {
                        Announce(Game, Player, "地产大亨");
                    }
                });
            }
        });
        TriggerList.Add(new PTrigger("大包工头") {
            IsLocked = true,
            Time = PTime.EndGameTime,
            Effect = (PGame Game) => {
                Game.GetWinner().ForEach((PPlayer Player) => {
                    if (PMath.Sum(Game.Map.BlockList.FindAll((PBlock Block) => Player.Equals(Block.Lord)).ConvertAll((PBlock Block) =>(double) Block.HouseNumber)) >= 30) {
                        Announce(Game, Player, "大包工头");
                    }
                });
            }
        });
        TriggerList.Add(new PTrigger("双11剁手") {
            IsLocked = true,
            Time = PTime.DieTime,
            Effect = (PGame Game) => {
                PDyingTag DyingTag = Game.TagManager.FindPeekTag<PDyingTag>(PDyingTag.TagName);
                PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                if (InjureTag != null && DyingTag != null && DyingTag.Player.Equals( InjureTag.ToPlayer) && InjureTag.InjureSource is PBlock && ((PBlock)InjureTag.InjureSource).BusinessType.Equals(PBusinessType.ShoppingCenter)) {
                    Announce(Game, DyingTag.Player, "双11剁手");
                }
            }
        });
        TriggerList.Add(new PTrigger("化学爆炸") {
            IsLocked = true,
            Time = PTime.DieTime,
            Effect = (PGame Game) => {
                PDyingTag DyingTag = Game.TagManager.FindPeekTag<PDyingTag>(PDyingTag.TagName);
                PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                if (InjureTag != null && DyingTag != null && DyingTag.Player.Equals(InjureTag.ToPlayer) && InjureTag.InjureSource is PBlock && ((PBlock)InjureTag.InjureSource).BusinessType.Equals(PBusinessType.Institute)) {
                    Announce(Game, DyingTag.Player, "化学爆炸");
                }
            }
        });
        TriggerList.Add(new PTrigger("舍身饲虎") {
            IsLocked = true,
            Time = PTime.DieTime,
            Effect = (PGame Game) => {
                PDyingTag DyingTag = Game.TagManager.FindPeekTag<PDyingTag>(PDyingTag.TagName);
                PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                if (InjureTag != null && DyingTag != null && DyingTag.Player.Equals(InjureTag.ToPlayer) && InjureTag.InjureSource is PBlock && ((PBlock)InjureTag.InjureSource).BusinessType.Equals(PBusinessType.Park)) {
                    Announce(Game, DyingTag.Player, "舍身饲虎");
                }
            }
        });
        TriggerList.Add(new PTrigger("撞墙") {
            IsLocked = true,
            Time = PTime.DieTime,
            Effect = (PGame Game) => {
                PDyingTag DyingTag = Game.TagManager.FindPeekTag<PDyingTag>(PDyingTag.TagName);
                PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                if (InjureTag != null && DyingTag != null && DyingTag.Player.Equals(InjureTag.ToPlayer) && InjureTag.InjureSource is PBlock && ((PBlock)InjureTag.InjureSource).BusinessType.Equals(PBusinessType.Castle)) {
                    Announce(Game, DyingTag.Player, "撞墙");
                }
            }
        });
        TriggerList.Add(new PTrigger("卖身") {
            IsLocked = true,
            Time = PTime.DieTime,
            Effect = (PGame Game) => {
                PDyingTag DyingTag = Game.TagManager.FindPeekTag<PDyingTag>(PDyingTag.TagName);
                PInjureTag InjureTag = Game.TagManager.FindPeekTag<PInjureTag>(PInjureTag.TagName);
                if (InjureTag != null && DyingTag != null && DyingTag.Player.Equals(InjureTag.ToPlayer) && InjureTag.InjureSource is PBlock && ((PBlock)InjureTag.InjureSource).BusinessType.Equals(PBusinessType.Pawnshop)) {
                    Announce(Game, DyingTag.Player, "卖身");
                }
            }
        });
        TriggerList.Add(new PTrigger("连锁商城") {
            IsLocked = true,
            Time = PTime.EndGameTime,
            Effect = (PGame Game) => {
                Game.GetWinner().ForEach((PPlayer Player) => {
                    if (Game.Map.BlockList.FindAll((PBlock Block) => Player.Equals(Block.Lord) && Block.BusinessType.Equals(PBusinessType.ShoppingCenter)).Count >= 3) {
                        Announce(Game, Player, "连锁商城");
                    }
                });
            }
        });
        TriggerList.Add(new PTrigger("5A级景区") {
            IsLocked = true,
            Time = PTime.EndGameTime,
            Effect = (PGame Game) => {
                Game.GetWinner().ForEach((PPlayer Player) => {
                    if (Game.Map.BlockList.Exists((PBlock Block) => Player.Equals(Block.Lord) && Block.BusinessType.Equals(PBusinessType.Park) && Block.HouseNumber >= 5)) {
                        Announce(Game, Player, "5A级景区");
                    }
                });
            }
        });
        TriggerList.Add(new PTrigger("十三太堡") {
            IsLocked = true,
            Time = PTime.EndGameTime,
            Effect = (PGame Game) => {
                Game.GetWinner().ForEach((PPlayer Player) => {
                    if (Game.Map.BlockList.Exists((PBlock Block) => Player.Equals(Block.Lord) && Block.BusinessType.Equals(PBusinessType.Castle) && Block.HouseNumber >= 13)) {
                        Announce(Game, Player, "十三太堡");
                    }
                });
            }
        });
    }
}