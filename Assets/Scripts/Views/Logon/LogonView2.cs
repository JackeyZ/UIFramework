using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFramework;

/// <summary>
/// 名称：登录面板脚本
/// 作用：
/// </summary>
public class LogonView2 : BaseView
{
    void Start()
    {
    }

    public void OpenLogon()
    {
        UIFramework.UIManager.Instance.Open("ui/prefabs/logon.u3dassetbundle", "LogonPanel");
    }
    public void OpenItemDisplay()
    {
        UIFramework.UIManager.Instance.Open("ui/prefabs/itemdisplay.u3dassetbundle", "ItemDisplay");
    }
}
