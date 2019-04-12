using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Singleton<T> where T : class, new(){ 
    public Singleton()
    {
    }
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new T(); //Activator.CreateInstance<T>();
            }
            return _instance;
        }
    }
}
