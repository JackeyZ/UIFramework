using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFramework;
using AssetBundleFramework;

/// <summary>
/// 名称：游戏引导
/// 作用：整个游戏的开始
/// </summary>
public class GameRoot : MonoBehaviour
{
    void Start()
    {
        UIManager.Instance.Open("ui/prefabs/mainview.u3dassetbundle","MainView");
        Kernal.GameObjectPool.Instance.PreLoadGameObject();
        StartCoroutine("TestLoad");
    }

    IEnumerator TestLoad()
    {
        while (enabled)
        {
            yield return new WaitForSeconds(3f);
            UIManager.Instance.Open("ui/prefabs/messagepopup.u3dassetbundle", "MessagePopUp", "公告：123123" as object);
        }
    }
}
