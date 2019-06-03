using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// MonoBehaviour单例
/// </summary>
/// <typeparam name="T"></typeparam>
public class MonoSingleton<T> : MonoBehaviour where T : UnityEngine.Component{
    private static T _Instance;
    public static bool isDestory = false;                       // 记录该单例是否已经销毁，用于防止退出游戏的时候，其他脚本在OnDisable中调用该单例，但该单例已被销毁而导致的报错

    public static T Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = new GameObject("_Script:" + typeof(T).Name).AddComponent<T>();
                DontDestroyOnLoad(_Instance);
            }
            return _Instance;
        }
    }

    public void OnDestroy()
    {
        isDestory = true;
    }
}
