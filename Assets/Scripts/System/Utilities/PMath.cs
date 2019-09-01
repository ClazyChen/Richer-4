using System;
using System.Collections.Generic;
using UnityEngine;

public class PMath {
    static System.Random random = new System.Random();
    public static int RandInt(int l, int r) {
        if (l == r) {
            return l;
        }
        return random.Next(l, r + 1);
    }
    public static int Max(List<int> Samples) {
        int Max = int.MinValue;
        Samples.ForEach((int Sample) => Max = Math.Max(Max, Sample));
        return Max;
    }
    public static KeyValuePair<T, int> Max<T>(List<T> Samples, Converter<T, int> Measure, bool MustPositive = false) where T: PObject {
        int Max = int.MinValue;
        T MaxSample = null;
        Samples.ForEach((T Sample) => {
            int Test = Measure(Sample);
            if (Test >= Max) {
                Max = Test;
                MaxSample = Sample;
            }
        });
        if (MustPositive && Max <= 0) {
            return new KeyValuePair<T, int> (null, Max);
        }
        return new KeyValuePair<T, int>(MaxSample, Max);
    }
    public static int Min(List<int> Samples) {
        int Min = int.MaxValue;
        Samples.ForEach((int Sample) => Min = Math.Min(Min, Sample));
        return Min;
    }
    public static KeyValuePair<T,int> Min<T>(List<T> Samples, Converter<T, int> Measure) where T : PObject {
        int Min = int.MaxValue;
        T MinSample = null;
        Samples.ForEach((T Sample) => {
            int Test = Measure(Sample);
            if (Test <= Min) {
                Min = Test;
                MinSample = Sample;
            }
        });
        return new KeyValuePair<T, int>(MinSample, Min);
    }
    public static double Sum(List<double> Samples) {
        double Sum = 0;
        Samples.ForEach((double Sample) =>Sum += Sample);
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

    public static int RandomIndex(List<double> Weight) {
        for (int i = 0; i < Weight.Count - 1; ++i) {
            if (RandTest(Weight[i] / Sum(Weight.GetRange(i, Weight.Count - i)))) {
                return i;
            }
        }
        return Weight.Count - 1;
    }
}