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
        UIManager.Instance.Open("ui/prefabs/logon.u3dassetbundle","LogonPanel");
        Kernal.GameObjectPool.Instance.PreLoadGameObject();
    }

    IEnumerator TestLoad()
    {
        while (enabled)
        {
            yield return new WaitForSeconds(0.5f);
        }
    }
}
