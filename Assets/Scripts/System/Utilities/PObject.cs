using System.Collections.Generic;
using System.Reflection;
using System;
using System.Linq;

public class PObject {
    public string Name;
    public override string ToString() {
        return Name;
    }

    public static List<T> ListInstance<T>() where T : PObject {
        return new List<FieldInfo>(typeof(T).GetFields())
            .FindAll((FieldInfo Field) => Field.FieldType.Equals(typeof(T)))
            .ConvertAll((FieldInfo Field) => (T)Field.GetValue(null));
    }

    // 在继承于PObject的某个类型T的所有静态对象中寻找指定对象
    public static T FindInstance<T>(string InstanceName) where T : PObject {
        return ListInstance<T>().Find((T Instance) => Instance.Name.Equals(InstanceName));
    }

    public static IEnumerable<Type> ListSubTypes<T>() {
        return typeof(T).Assembly.GetTypes().Where((Type TempType) => TempType.IsSubclassOf(typeof(T)));
    }

    public static List<T> ListSubTypeInstances<T>() {
        List<T> TempList = new List<T>();
        foreach (Type SubType in ListSubTypes<T>()) {
            try {
                T Instance = (T)Activator.CreateInstance(SubType);
                TempList.Add(Instance);
            } catch {
                continue;
            }
        }
        return TempList;
    }

    public static List<T> CloneList<T>(List<T> Source) where T:PObject {
        List<T> ans = new List<T>();
        Source.ForEach((T x) => ans.Add(x));
        return ans;
    }
}