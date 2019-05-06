using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFramework;

/// <summary>
/// 名称：
/// 作用：
/// </summary>
public class MainView : BaseView
{
    public void OpenMountView()
    {
        UIManager.Instance.Open("ui/prefabs/logon.u3dassetbundle", "LogonPanel");
    }

    public void OpenWingView()
    {
        UIFramework.UIManager.Instance.Open("ui/prefabs/itemdisplay.u3dassetbundle", "ItemDisplay");
    }
}
