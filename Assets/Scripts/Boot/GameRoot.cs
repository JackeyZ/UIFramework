using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFramework;
using AssetBundleFramework;

using System.Text;
using System.IO;

/// <summary>
/// 名称：游戏引导
/// 作用：整个游戏的开始
/// </summary>
public class GameRoot : MonoBehaviour
{
    float tempTime = 0;
    void Start()
    {
        UIManager.Instance.Open("ui/prefabs/mainview.u3dassetbundle","MainView");
        Kernal.GameObjectPool.Instance.PreLoadGameObject();
        //StartCoroutine("TestLoad2");
    }

    IEnumerator TestLoad()
    {
        while (enabled)
        {
            if (tempTime % 3 < 1)
            {
                yield return new WaitForSeconds(0.5f);
            }
            else
            {
                yield return new WaitForSeconds(3f);
            }
            tempTime += Time.deltaTime;
            UIManager.Instance.Open("ui/prefabs/messagepopup.u3dassetbundle", "MessagePopUp", "公告：123123" as object);
        }
    }
    IEnumerator TestLoad2()
    {
        yield return new WaitForSeconds(5f);
        SceneLoader.LoadSceneSync("scenes/fightscenes.u3dscene", "FightScene01");
        ConfigManager.Instance.LoadConfig("excel1");
    }
}
