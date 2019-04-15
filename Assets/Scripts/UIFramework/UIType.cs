using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIFramework
{
    /// <summary>
    /// 名称：窗体类型
    /// 
    /// </summary>
    [Serializable]
    public class UIType
    {
        public UIViewType uiViewType = UIViewType.Normal;                       // UI窗体类型
        public UIViewShowMode uiViewShowMode = UIViewShowMode.Normal;           // UI窗体显示类型
        public UIViewLucenyType uiViewLucenyType = UIViewLucenyType.Lucency;    // UI窗体透明度类型
    }
}