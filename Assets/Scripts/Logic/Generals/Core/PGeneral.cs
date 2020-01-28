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

    public int Cost = 0;
    public string Tips = string.Empty;


    /// <summary>
    /// 客户端专用
    /// </summary>
    public List<PSkillInfo> SkillInfoList;

    public PGeneral(string _Name) {
        Name = _Name;
        SkillList = new List<PSkill>();
        SkillInfoList = new List<PSkillInfo>();
    }

}