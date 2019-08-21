using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

/// <summary>
/// PAbstractGroupUI类：
/// 用于实现UI界面内部的一组子UI的管理
/// </summary>
/// <typeparam name="T">必须实现T(Transform _Background)构造函数</typeparam>
public class PAbstractGroupUI<T> : PAbstractUI where T : PAbstractUI {

    /// <summary>
    /// 原型UI，在背景下只能有一个子对象拥有Prototype的词尾
    /// </summary>
    public readonly T PrototypeUI;
    public List<T> GroupUIList;

    /// <summary>
    /// 初始化UI组并找到原型
    /// </summary>
    /// <param name="_Background"></param>
    public PAbstractGroupUI(Transform _Background) : base(_Background) {
        PrototypeUI = null;
        for (int i = 0; i < UIBackgroundImage.childCount; ++i) {
            Transform TempTransform = UIBackgroundImage.GetChild(i);
            if (TempTransform.name.EndsWith(Prototype)) {
                PrototypeUI = (T)Activator.CreateInstance(typeof(T), BindingFlags.Default, null, new object[] { TempTransform }, null);
                break;
            }
        }
        GroupUIList = new List<T>();
        Close();
    }

    /// <summary>
    /// 新建子UI(复制原型)，将其附在此UI的背景下
    /// </summary>
    /// <returns>新建的UI</returns>
    protected T AddSubUI() {
        GameObject NewObject = UnityEngine.Object.Instantiate(PrototypeUI.UIBackgroundImage.gameObject);
        NewObject.transform.SetParent(UIBackgroundImage);
        T NewUI = (T)Activator.CreateInstance(typeof(T), BindingFlags.Default, null, new object[] { NewObject.transform }, null);
        NewUI.Open();
        GroupUIList.Add(NewUI);
        return NewUI;
    }

    /// <summary>
    /// 重构Close函数，先关闭子UI和原型再关闭本体
    /// </summary>
    public override void Close() {
        GroupUIList.ForEach((T SubUI) => SubUI.Close());
        GroupUIList.Clear();
        PrototypeUI.Close();
        base.Close();
    }

    //public override void Open() {
    //    base.Open();
    //    GroupUIList.ForEach((T SubUI) => SubUI.Open());
    //}
}
