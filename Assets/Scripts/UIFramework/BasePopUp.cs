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
        /// <summary>
        /// PopUp类型的面板，关闭由自己控制，不交给UIManager管理，因为PopUp类型面板可以多开，Key（AB路径:资源名称）值对应很多个面板实例
        /// </summary>
        public override void Close()
        {
            GameObjectPool.Instance.DestroyGO(gameObject, true);
        }

        /// <summary>
        /// PopUp类型的面板，关闭由自己控制，不交给UIManager管理，因为PopUp类型面板可以多开，Key（AB路径:资源名称）值对应很多个面板实例
        /// </summary>
        public override void ClickClose()
        {
            Close();
        }
    }
}