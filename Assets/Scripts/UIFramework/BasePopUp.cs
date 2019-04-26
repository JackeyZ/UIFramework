using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kernal;
namespace UIFramework
{
    /// <summary>
    /// 名称：弹出窗口基类
    /// 作用：
    /// </summary>
    public class BasePopUp : BaseView
    {
        public override void Close()
        {
            GameObjectPool.Instance.DestroyGO(gameObject, true);
        }
    }
}