using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// MonoBehaviour单例
/// </summary>
/// <typeparam name="T"></typeparam>
public class MonoSingleton<T> : MonoBehaviour where T : UnityEngine.Component{
    private static T _Instance;

    public static T Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = new GameObject("_Script:" + typeof(T).Name).AddComponent<T>();
            }
            return _Instance;
        }
    }
}
