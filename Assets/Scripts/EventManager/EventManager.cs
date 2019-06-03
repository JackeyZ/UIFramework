using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace EventSystem
{
    public enum EventName
    {
        event1,
    }

    /// <summary>
    /// 名称：事件管理器
    /// 作用：管理整个游戏的事件消息
    /// </summary>
    public class EventManager : MonoSingleton<EventManager>
    {
        private Dictionary<EventName, Action<ArrayList>> eventDic1 = new Dictionary<EventName, Action<ArrayList>>();    // 事件字典
        private Dictionary<EventName, Action> eventDic0 = new Dictionary<EventName, Action>();                          // 无参数事件字典

        // 事件触发队列，一帧只触发一个事件
        private Queue<QueueItem> triggerQueue = new Queue<QueueItem>();

        /// <summary>
        /// 事件队列元素
        /// </summary>
        class QueueItem {
            public EventName name;
            public ArrayList dataList;

            public QueueItem (EventName eventName, ArrayList dataList)
            {
                name = eventName;
                this.dataList = dataList;
            }

            public override string ToString()
            {
                return name + dataList.GetHashCode().ToString();
            }
        }

        /// <summary>
        /// 重写比较函数，服务于于Queue.Contains()方法，只比较eventName和ArrayData里面的元素，不比较引用
        /// </summary>
        class QueueItemComparer : IEqualityComparer<QueueItem>
        {
            // 重写比较方法
            public bool Equals(QueueItem x, QueueItem y)
            {
                if (y.name == x.name && y.dataList.Count == x.dataList.Count)
                {
                    bool result = true;
                    for (int i = 0; i < y.dataList.Count; i++)
                    {
                        if (x.dataList[i].GetHashCode() != y.dataList[i].GetHashCode())
                        {
                            result = false;
                        }
                    }
                    return result;
                }
                return false;
            }
            
            public int GetHashCode(QueueItem obj)
            {
                return obj.GetHashCode();
            }
        }

        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="listener"></param>
        public void AddListening(EventName eventName, Action<ArrayList> listener)
        {
            Action<ArrayList> curAction = null;
            if (eventDic1.TryGetValue(eventName, out curAction))
            {
                curAction += listener;
            }
            else
            {
                eventDic1.Add(eventName, listener);
            }
        }

        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="listener"></param>
        public void AddListening(EventName eventName, Action listener)
        {
            Action curAction = null;
            if (eventDic0.TryGetValue(eventName, out curAction))
            {
                curAction += listener;
            }
            else
            {
                eventDic0.Add(eventName, listener);
            }
        }

        /// <summary>
        /// 注销事件
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="listener"></param>
        public void RemoveListening(EventName eventName, Action<ArrayList> listener)
        {
            Action<ArrayList> curAction = null;
            if (eventDic1.TryGetValue(eventName, out curAction))
            {
                curAction -= listener;

                if (curAction != null)
                {
                    Delegate[] delArray = curAction.GetInvocationList();
                    if (delArray.Length == 0)
                    {
                        eventDic1.Remove(eventName);
                    }
                }
            }
        }

        /// <summary>
        /// 注销事件
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="listener"></param>
        public void RemoveListening(EventName eventName, Action listener)
        {
            Action curAction = null;
            if (eventDic0.TryGetValue(eventName, out curAction))
            {
                curAction -= listener;

                if (curAction != null)
                {
                    Delegate[] delArray = curAction.GetInvocationList();
                    if (delArray.Length == 0)
                    {
                        eventDic0.Remove(eventName);
                    }
                }
            }
        }

        /// <summary>
        /// 每帧只触发一个事件
        /// </summary>
        void Update()
        {
            if(triggerQueue.Count > 0)
            {
                QueueItem qi = triggerQueue.Dequeue();
                Action<ArrayList> curAction1 = null;
                if (eventDic1.TryGetValue(qi.name, out curAction1))
                {
                    curAction1(qi.dataList);
                }

                Action curAction0 = null;
                if (eventDic0.TryGetValue(qi.name, out curAction0))
                {
                    curAction0();
                }
            }
        }

        /// <summary>
        /// 触发事件
        /// </summary>
        /// <param name="eventName">事件名</param>
        /// <param name="data">数据</param>
        public void TriggerEvent(EventName eventName, ArrayList data = null)
        {
            QueueItem qi = new QueueItem(eventName, data);
            QueueItemComparer comparer = new QueueItemComparer();
            // 判断触发的事件是否重复（事件名和data都相等才算重复）
            if (!triggerQueue.Contains(qi, comparer))
            {
                // 进入队列，每帧触发一个
                triggerQueue.Enqueue(qi);
            }
        }

        public void TriggerEvent<T>(EventName eventName, T data) where T : class
        {
            TriggerEvent(eventName, new ArrayList { data });
        }

        public void TriggerEvent<T1, T2>(EventName eventName, T1 data1, T2 data2 = null)
            where T1 : class where T2 : class
        {
            TriggerEvent(eventName, new ArrayList { data1, data2 });
        }
        public void TriggerEvent<T1, T2, T3>(EventName eventName, T1 data1, T2 data2 = null, T3 data3 = null)
            where T1 : class where T2 : class where T3 : class
        {
            TriggerEvent(eventName, new ArrayList { data1, data2, data3 });
        }

        public void TriggerEvent<T1, T2, T3, T4>(EventName eventName, T1 data1, T2 data2 = null, T3 data3 = null, T4 data4 = null)
            where T1 : class where T2 : class where T3 : class where T4 : class
        {
            TriggerEvent(eventName, new ArrayList { data1, data2, data3, data4 });
        }

        public void TriggerEvent<T1, T2, T3, T4, T5>(EventName eventName, T1 data1, T2 data2 = null, T3 data3 = null, T4 data4 = null, T5 data5 = null)
            where T1 : class where T2 : class where T3 : class where T4 : class where T5 : class
        {
            TriggerEvent(eventName, new ArrayList { data1, data2, data3, data4, data5 });
        }
    }

}
