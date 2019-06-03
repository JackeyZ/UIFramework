using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventSystem;

/// <summary>
/// 名称：测试事件系统用
/// 作用：
/// </summary>
public class TestEvent : MonoBehaviour
{
    void Start()
    {
        StartCoroutine("Cor");
    }

    void OnEnable()
    {
        //EventManager.Instance.AddListening(EventName.event1, CallBack2);
        EventManager.Instance.AddListening(EventName.event1, CallBack);

    }

    public void CallBack(ArrayList data)
    {
        if(data != null)
        {
            string str = "";
            foreach (var item in data)
            {
                str += item.ToString();
            }
            Debug.Log(str);
        }
        else
        {
            Debug.Log("CallBack null");
        }
    }
    public void CallBack2()
    {
        Debug.Log("CallBack2");
    }

    IEnumerator Cor()
    {
        yield return new WaitForSeconds(5f);
        EventManager.Instance.TriggerEvent(EventName.event1, "1");
        EventManager.Instance.TriggerEvent(EventName.event1, "2");
        EventManager.Instance.TriggerEvent(EventName.event1, "2");
        EventManager.Instance.TriggerEvent(EventName.event1, "2");
        EventManager.Instance.TriggerEvent(EventName.event1, "2");
        EventManager.Instance.TriggerEvent(EventName.event1, "2");
        EventManager.Instance.TriggerEvent(EventName.event1, "2");
        EventManager.Instance.TriggerEvent(EventName.event1, "2","3");
        EventManager.Instance.TriggerEvent(EventName.event1, "3");
        //EventManager.Instance.TriggerEvent(EventName.event1);
        //EventManager.Instance.TriggerEvent(EventName.event1, "sdfsdf1");
        //EventManager.Instance.TriggerEvent(EventName.event1, "sdfsdf1", "sdfsdf2");
        //EventManager.Instance.TriggerEvent(EventName.event1, "sdfsdf1", "sdfsdf2", "sdfsdf3");
        //EventManager.Instance.TriggerEvent(EventName.event1, "sdfsdf1", "sdfsdf2", "sdfsdf3", "sdfsdf4");
        //EventManager.Instance.TriggerEvent(EventName.event1, "sdfsdf1", "sdfsdf2", "sdfsdf3", "sdfsdf4", "sdfsdf5");
    }

    void OnDisable()
    {
        //EventManager.Instance.RemoveListening(EventName.event1, CallBack2);
        // 防止在关闭游戏的时候单例被销毁
        if(!EventManager.isDestory)
        {
            EventManager.Instance.RemoveListening(EventName.event1, CallBack);
        }
    }
}
