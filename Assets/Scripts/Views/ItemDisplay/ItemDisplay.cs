using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFramework;

/// <summary>
/// 名称：
/// 作用：
/// </summary>
public class ItemDisplay : BaseView
{

    public void OpenItemDisplay2()
    {
        UIFramework.UIManager.Instance.Open("ui/prefabs/itemdisplay.u3dassetbundle", "ItemDisplay2");
    }
}
