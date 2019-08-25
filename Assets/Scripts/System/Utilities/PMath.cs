using System;
using System.Collections.Generic;

public class PMath {
    static Random random = new Random();
    public static int RandInt(int l, int r) {
        return random.Next(l, r + 1);
    }
    public static int Max(List<int> Samples) {
        int Max = int.MinValue;
        Samples.ForEach((int Sample) => Max = Math.Max(Max, Sample));
        return Max;
    }
    public static int Min(List<int> Samples) {
        int Min = int.MaxValue;
        Samples.ForEach((int Sample) => Min = Math.Min(Min, Sample));
        return Min;
    }
    public static int Sum(List<int> Samples) {
        int Sum = 0;
        Samples.ForEach((int Sample) =>Sum += Sample);
        return Sum;
    }

    /// <summary>
    /// 以一定概率返回true
    /// </summary>
    /// <param name="p">指定的概率</param>
    /// <returns></returns>
    public static bool RandTest(double p) {
        return random.NextDouble() <= p;
    }

    public static int Percent(int Base, int Percentage) {
        float temp = (float)Base / 1000;
        int TenCents = (int)(temp + 0.99) * 100;
        return TenCents * (Percentage / 10);
    }
}