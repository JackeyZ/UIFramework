using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssetBundleFramework;

namespace UIFramework
{
    /// <summary>
    /// 名称：UI管理器
    /// 作用：UI框架核心，用户通过本脚本，来使用框架的绝大部分功能
    /// </summary>
    public class UIManager : MonoSingleton<UIManager>
    {
        private Dictionary<string, ABAsset> _DicViewPrefabPath = new Dictionary<string, ABAsset>();         // 用于储存窗体预设，key为窗体名称、value是AB资源
        private Dictionary<string, BaseView> _DicViews = new Dictionary<string, BaseView>();                // 用于缓存所有UI窗体
        private Dictionary<string, BaseView> _DicShowViews = new Dictionary<string, BaseView>();            // 用于储存当前显示的UI窗体

        private Transform _TraUIRoot = null;           // UI根节点
        private Transform _TraNormalRoot = null;       // 普通全屏窗口根节点
        private Transform _TraFixedRoot = null;        // 固定显示的UI根节点
        private Transform _TraPopUpRoot = null;        // 弹出窗体的UI根节点

        void Awake()
        {

        }
    }
}
