using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UIFramework;

/// <summary>
/// 名称：
/// 作用：
/// </summary>
public class MessagePopUp : BasePopUp
{
    protected override void OpenCallback()
    {
        StartCoroutine("DelayClose");
    }

    IEnumerator DelayClose()
    {
        yield return new WaitForSeconds(2f);
        ClickClose();
    }

    protected override void SetDataCallback(object data)
    {
        string messageText = data as string;
        GameObjects["TxtMessage"].GetComponent<Text>().text = messageText;
    }
}
