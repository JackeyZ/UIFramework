using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIFramework
{
    /// <summary>
    /// 名称：窗体基类
    /// 作用：
    ///     1、定义四个生命周期：
    ///         Display（显示状态）、Hiding（隐藏状态）、ReDisplay（再显示状态）、Freeze（冻结状态）
    ///     2、
    ///
    /// </summary>
    public class BaseView : MonoBehaviour
    {
        /*字段*/
        private UIType uiType = new UIType();


        /*属性*/
        public UIType UiType
        {
            get
            { return uiType; }
            set
            { uiType = value; }
        }

        /// <summary>
        /// 显示状态
        /// </summary>
        public virtual void Display()
        {
            this.gameObject.SetActive(true);
        }

        /// <summary>
        /// 隐藏状态
        /// </summary>
        public virtual void Hiding()
        {

        }

        /// <summary>
        /// 再显示状态
        /// </summary>
        public virtual void ReDisplay()
        {

        }

        /// <summary>
        /// 冻结状态
        /// </summary>
        public virtual void Freeze()
        {

        }
    }
}
