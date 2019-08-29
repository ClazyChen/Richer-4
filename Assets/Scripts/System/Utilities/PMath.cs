using System;
using System.Collections.Generic;
using UnityEngine;

public class PMath {
    static System.Random random = new System.Random();
    public static int RandInt(int l, int r) {
        return random.Next(l, r + 1);
    }
    public static int Max(List<int> Samples) {
        int Max = int.MinValue;
        Samples.ForEach((int Sample) => Max = Math.Max(Max, Sample));
        return Max;
    }
    public static T Max<T>(List<T> Samples, Converter<T, int> Measure) where T: PObject {
        int Max = int.MinValue;
        T MaxSample = null;
        Samples.ForEach((T Sample) => {
            int Test = Measure(Sample);
            if (Test >= Max) {
                Max = Test;
                MaxSample = Sample;
            }
        });
        return MaxSample;
    }
    public static int Min(List<int> Samples) {
        int Min = int.MaxValue;
        Samples.ForEach((int Sample) => Min = Math.Min(Min, Sample));
        return Min;
    }
    public static T Min<T>(List<T> Samples, Converter<T, int> Measure) where T : PObject {
        int Min = int.MaxValue;
        T MinSample = null;
        Samples.ForEach((T Sample) => {
            int Test = Measure(Sample);
            if (Test >= Min) {
                Min = Test;
                MinSample = Sample;
            }
        });
        return MinSample;
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

    public static bool InRect(Vector3 Point, RectTransform Rect) {
        return (Math.Abs(Point.x - Rect.position.x) < Rect.rect.width * Rect.lossyScale.x / 2) && (Math.Abs(Point.y - Rect.position.y) < Rect.rect.height * Rect.lossyScale.y / 2);
    }
}