using System;

public class PRandom {
    static Random random = new Random();
    public static int RandInt(int l, int r) {
        return random.Next(l, r + 1);
    }
}