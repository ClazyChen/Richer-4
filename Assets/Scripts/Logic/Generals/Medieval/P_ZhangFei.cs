using System;
using System.Collections.Generic;

public class P_ZhangFei : PGeneral {


    public P_ZhangFei() : base("张飞") {
        Sex = PSex.Male;
        Age = PAge.Medieval;
        Index = 14;
        Cost = 10;
        Tips = "定位：攻击\n" +
            "难度：简单\n" +
            "史实：三国时期蜀汉名将，“五虎上将”之一，曾在长坂坡负责断后，曹军无人敢逼近。\n" +
            "攻略：\n张飞是一个使用起来很简单的武将，为新手推荐。\n" +
            "张飞可以很容易制造出很高等级的房屋，如果人数较多而地图较小，高级房屋可以造成相当巨大的优势。但反之，如果人数少而地图大，滥用【咆哮】会导致张飞在前期现金数量迅速减少，而陷入发展不平衡的困境。由于张飞几乎可以完全控制地图上各片地的发展情况，所以集中力量发展还是分散建设到不同土地上，是张飞面临的首要问题。一般而言，张飞每片地应当保持至少2座房屋，以形成对【反客为主】的完全防御。";

        PSkill PaoXiao = new PSkill("咆哮") {
            Lock = true
        };
        SkillList.Add(PaoXiao
            .AddTrigger(
            (PPlayer Player, PSkill Skill) => {
                return new PTrigger(PaoXiao.Name) {
                    IsLocked = true,
                    Player = Player,
                    Time = PPeriod.SettleStage.Start,
                    Condition = (PGame Game) => {
                        return Player.Equals(Game.NowPlayer);
                    },
                    Effect = (PGame Game) => {
                        Game.TagManager.FindPeekTag<PPurchaseTag>(PPurchaseTag.TagName).Limit += 3;
                    }
                };
            }));
    }

}