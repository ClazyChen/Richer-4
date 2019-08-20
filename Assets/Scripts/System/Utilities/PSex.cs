public class PSex: PObject {
    private PSex (string _Name) {
        Name = _Name;
    }

    public static PSex NoSex = new PSex("无性别");
    public static PSex Male = new PSex("男性");
    public static PSex Female = new PSex("女性");
    public static PSex Trans = new PSex("转性");
}