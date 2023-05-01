using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformExts
{
    public static T Find<T>(this Transform transform,string path) where T: Component
    {
        var t = transform.Find(path);
        if (t == null)
        {
            return default(T);
        }
        return t.GetComponent<T>();
    }
    public static T GetComponent<T>(this GameObject gameObject, bool addIfNothing) where T : Component
    {
        T t = null;
        t = gameObject.GetComponent<T>();
        if (t == null && addIfNothing)
        {
            t = gameObject.AddComponent<T>();
        }
        return t;
    }
    public static Transform DeepFind(this Transform parent,string targetName)
    {
        Transform result = null;
        foreach (Transform item in parent)
        {
            if (item.name == targetName)
            {
                return item;
            }
            else
            {
                result=DeepFind(item, targetName);
                if (result != null)
                {
                    return result;
                }
            }
        }
        return null;
    }
}
