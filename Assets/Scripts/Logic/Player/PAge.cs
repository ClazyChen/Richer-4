public class PAge: PObject {
    private PAge(string _Name) {
        Name = _Name;
    }

    public static PAge NoAge = new PAge("无时代");
    public static PAge Classic = new PAge("古典时代");
    public static PAge Medieval = new PAge("中古时代");
    public static PAge Renaissance = new PAge("文艺复兴");
    public static PAge Industrial = new PAge("工业时代");
}