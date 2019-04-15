using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFramework;

/// <summary>
/// 名称：游戏引导
/// 作用：整个游戏的开始
/// </summary>
public class GameRoot : MonoBehaviour
{
    void Start() {
        UIManager.Instance.Open("LogonView");
    }
}
