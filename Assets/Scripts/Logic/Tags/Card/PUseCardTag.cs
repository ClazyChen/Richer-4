using System.Collections.Generic;

public class PUseCardTag : PTag {
    public static string TagName = "使用卡牌";
    public static string CardFieldName = "使用的卡牌";
    public static string UserFieldName = "卡牌的使用者";
    public static string TargetListFieldName = "卡牌的目标";
    public PUseCardTag(PCard _Card, PPlayer _User, List<PPlayer> _TargetList): base(TagName) {
        AppendField(CardFieldName, _Card);
        AppendField(UserFieldName, _User);
        AppendField(TargetListFieldName, _TargetList);
    }
    public PCard Card {
        get {
            return GetField<PCard>(CardFieldName, null);
        }
    }
    public PPlayer User {
        get {
            return GetField<PPlayer>(UserFieldName, null);
        }
    }
    public List<PPlayer> TargetList {
        get {
            return GetField<List<PPlayer>>(TargetListFieldName, null);
        }
    }

}