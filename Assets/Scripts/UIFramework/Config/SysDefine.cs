using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIFramework
{
    /// <summary>
    /// 名称：UI框架核心参数
    /// 作用：
    ///     定义UI框架的系统常量、委托、枚举等
    /// </summary>
    public class SysDefine
    {
        /* 路径 */

        /* 标签Tag */

        /* 委托 */

    }
    #region 枚举
    /// <summary>
    /// 窗体位置类型
    /// </summary>
    public enum UIViewType
    {
        Normal,         //普通
        Fixed,          //固定窗体
        PopUp           //弹出窗体
    }

    /// <summary>
    /// 窗体显示类型
    /// </summary>
    public enum UIViewShowMode
    {
        Normal,         //普通
        ReverseChange,  //反向切换
        HideOther       //隐藏其他
    }

    /// <summary>
    /// 透明度类型
    /// </summary>
    public enum UIViewLucenyType
    {
        Lucency,            //完全透明，不可穿透
        Translucence,       //半透明，不可穿透
        Impenetrable,       //低透明，不可穿透
        Pentrate,           //可穿透
    }

    public enum ViewOpenStatus
    {
        Close,          // 关闭状态（默认）
        Open,           // 打开状态（显示和隐藏状态也属于打开状态）
    }
    public enum ViewShowStatus
    {
        Show,           // 显示状态
        Hide,           // 隐藏状态
    }

    #endregion


}
