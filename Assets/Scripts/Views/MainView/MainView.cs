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
    public void OpenTest()
    {
        //var obj = AssetBundleFramework.AssetBundleMgr.Instance.LoadBundleAssetSync("ui/prefabs/itemdisplay.u3dassetbundle", "ItemDisplay");
        //GameObject go = Instantiate(obj as GameObject);
        //go.transform.SetParent(GameObject.Find("NormalRoot").transform);
        //go.transform.localPosition = new Vector3(0, 0, 0);
        //go.transform.localScale = new Vector3(1, 1, 1);
    }
}
