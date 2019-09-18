using System;
using System.Collections.Generic;
/// <summary>
/// PGeneral类：武将的模型
/// </summary>
public abstract class PGeneral: PObject {

    public PSex Sex = PSex.NoSex;
    public PAge Age = PAge.NoAge;
    public int Index = 0;
    public List<PSkill> SkillList;

    public PGeneral(string _Name) {
        Name = _Name;
        SkillList = new List<PSkill>();
    }

}