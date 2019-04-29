﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFramework;

/// <summary>
/// 名称：登录面板脚本
/// 作用：
/// </summary>
public class LogonView : BaseView
{
    void Start()
    {
    }

    public void OpenLogon2()
    {
        UIFramework.UIManager.Instance.Open("ui/prefabs/logon.u3dassetbundle", "LogonPanel2");
    }
    public void OpenItemDisplay()
    {
        UIFramework.UIManager.Instance.Open("ui/prefabs/itemdisplay.u3dassetbundle", "ItemDisplay");
    }
    public void OpenItemDisplay2()
    {
        UIFramework.UIManager.Instance.Open("ui/prefabs/logon.u3dassetbundle", "ItemDisplay2");
    }
}
