using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;

public abstract class PAbstractUI : PObject {
    /// <summary>
    /// 用于标志原型的组件
    /// </summary>
    protected static readonly string Prototype = "Prototype";

    /// <summary>
    /// 是否正在被激活
    /// </summary>
    public bool IsActive = false;
    /// <summary>
    /// UI的背景图-Unity相关
    /// </summary>
    public Transform UIBackgroundImage;
    /// <summary>
    /// 打开UI的时候的操作（初始化控件）
    /// </summary>
    public virtual void Open() {
        IsActive = true;
        UIBackgroundImage.gameObject.SetActive(true);
    }
    /// <summary>
    /// 关闭UI的时候的操作（删除控件和监听器）
    /// </summary>
    public virtual void Close() {
        RemoveAllListeners();
        RemoveAllOptions();
        IsActive = false;
        UIBackgroundImage.gameObject.SetActive(false);
    }

    /// <summary>
    /// 创建指定背景的UI
    /// </summary>
    /// <param name="_Background">背景</param>
    public PAbstractUI(Transform _Background) {
        UIBackgroundImage = _Background;
    }

    /// <summary>
    /// 获取指定类型的所有控件字段
    /// </summary>
    /// <typeparam name="T">控件（字段）类型</typeparam>
    /// <returns>所有该类型控件字段的列表</returns>
    protected List<T> GetControls<T>() {
        return new List<FieldInfo>(GetType().GetFields())
            .FindAll((FieldInfo Field) => Field.FieldType.Equals(typeof(T)))
            .ConvertAll((FieldInfo Field) => (T)Field.GetValue(this));
    }

    /// <summary>
    /// 初始化控件，获取本类的所有指定类型的变量，用反射实现，要求变量名和控件在Unity编辑器里的名字相同
    /// </summary>
    /// <param name="ControlType">控件的类型</param>
    protected void InitializeControls<T>() {
        new List<FieldInfo>(GetType().GetFields())
            .FindAll((FieldInfo Field) => Field.FieldType.Equals(typeof(T)))
            .ForEach((FieldInfo Field) => Field.SetValue(this, UIBackgroundImage.Find(Field.Name).GetComponent<T>()));
    }

    /// <summary>
    /// 清除所有Button字段上挂载的监听器
    /// </summary>
    protected void RemoveAllListeners() {
        GetControls<Button>().ForEach((Button Control) => Control.onClick.RemoveAllListeners());
    }

    /// <summary>
    /// 清除所有Dropdown字段的选项
    /// </summary>
    protected void RemoveAllOptions() {
        GetControls<Dropdown>().ForEach((Dropdown dropdown) => dropdown.options.Clear());
    }

    /// <summary>
    /// 重置所有Dropdown，如果有选项，显示第一个，否则显示空白
    /// </summary>
    protected void ResetDropdowns() {
        GetControls<Dropdown>().ForEach((Dropdown dropdown) => {
            if (dropdown.options.Count > 0) {
                dropdown.captionText.text = dropdown.options[0].text;
            } else {
                dropdown.captionText.text = string.Empty;
            }
        });
    }

    /// <summary>
    /// 设置颜色
    /// </summary>
    /// <param name="TargetColor">目标颜色</param>
    public void SetColor(Color TargetColor) {
        if (UIBackgroundImage.gameObject.GetComponent<MeshRenderer>() != null) {
            UIBackgroundImage.gameObject.GetComponent<MeshRenderer>().material.color = TargetColor;
        } else if (UIBackgroundImage.gameObject.GetComponent<Image>() != null) {
            UIBackgroundImage.gameObject.GetComponent<Image>().material.color = TargetColor;
        }
    }
}