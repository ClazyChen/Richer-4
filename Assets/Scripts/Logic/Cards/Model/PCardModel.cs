using System;
using System.Collections.Generic;
/// <summary>
/// PCardModel类：卡牌的模型
/// </summary>
public abstract class PCardModel: PObject {
    /*
     * PCardModel 的设计逻辑：
     * 每个PCard内含一个Model的实例
     * 
     * 对于计策牌（继承自PSchemeCardModel），拥有一个Trigger List域MoveInHandTriggerList
     *      在这张牌进入手牌时，安装相应的Trigger
     * 对于伏击牌也有（部分公共的），以及判定阶段的一个Trigger（实际上在判定阶段的During的规则Trigger内触发）
     * 对于装备牌，拥有一个绑定的技能
     * 
     * 此外对于计策牌，有一个InHandExpectation函数，计算这张牌被这名角色拿着的收益
     * 对于伏击牌，还有InJudgeAreaExpectation函数，计算这张牌在这名角色判定区内的收益
     * 对于装备牌，其在装备区内的收益InEquipmentAreaExpectation函数，包括技能价值和【枭姬等技能带来的额外收益】两部分
     */

    public PCardType Type;
    public int Point;
    public int Index;

    public List<Func<PPlayer, PCard, PTrigger>> MoveInHandTriggerList;
    public List<Func<PPlayer, PCard, PTrigger>> MoveInEquipTriggerList;

    protected delegate List<PPlayer> TargetChooser(PGame Game, PPlayer Player);
    protected delegate void EffectFunc(PGame Game, PPlayer User, PPlayer Target);

    public virtual int AIInEquipExpectation(PGame Game, PPlayer Player) {
        return 0;
    }

    public virtual int AIInHandExpectation(PGame Game, PPlayer Player) {
        return 0;
    }

    public PCardModel(string _Name) {
        Name = _Name;
        MoveInHandTriggerList = new List<Func<PPlayer, PCard, PTrigger>>();
        MoveInEquipTriggerList = new List<Func<PPlayer, PCard, PTrigger>>();
    }
    public PCard Instantiate() {
        return new PCard(this);
    }
}