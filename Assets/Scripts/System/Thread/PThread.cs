using System;
using UnityEngine;
using System.Threading;

public class PThread {
    private class Config {
        public static float ThreadWaitTime = 0.01f;
    }

    public static void Delay(float DelayTime) {
        if (DelayTime < 0.0f) {
            return;
        }
        try {
            Thread.Sleep(Mathf.FloorToInt(DelayTime * 1000));
        } catch (ThreadInterruptedException) {
            // 不需要做任何处理
        }
    }
    public static void WaitUntil(Func<bool> judger) {
        while (!judger()) {
            Delay(Config.ThreadWaitTime);
        }
    }
    public static void Async(Action action) {
        (new Thread(() => { action(); }) {
            IsBackground = true
        }).Start();
    }
    /// <summary>
    /// 【阻塞】重复执行某个函数若干次
    /// </summary>
    /// <param name="SingleAction">每次执行的命令函数</param>
    /// <param name="RepeatTimes">重复次数</param>
    /// <param name="TotalDelayTime">每次执行命令间延时之和</param>
    public static void Repeat(Action SingleAction, int RepeatTimes, float TotalDelayTime) {
        if (RepeatTimes <= 0) {
            return;
        }
        float DelayTime = TotalDelayTime / RepeatTimes;
        for (int i = 0; i < RepeatTimes; ++i) {
            SingleAction();
            Delay(DelayTime);
        }
    }
}