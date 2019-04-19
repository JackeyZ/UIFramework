using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssetBundleFramework;
using Kernal;
using System;

namespace UIFramework
{
    /// <summary>
    /// 名称：UI管理器
    /// 作用：UI框架核心，用户通过本脚本，来使用框架的绝大部分功能
    /// </summary>
    public class UIManager : MonoSingleton<UIManager>
    {
        private Dictionary<string, ABAsset> _DicViewPrefabPath = new Dictionary<string, ABAsset>();         // 用于储存窗体预设，key为窗体名称、value是AB资源
        private Dictionary<string, BaseView> _DicViews = new Dictionary<string, BaseView>();                // 用于缓存所有UI窗体（同时只能打开一个的窗体）
        private Dictionary<string, BaseView> _DicShowViews = new Dictionary<string, BaseView>();            // 用于储存当前显示的UI窗体（同时只能打开一个的窗体）
        private Queue<string> _QueNeedOpen = new Queue<string>();                                           // 需要打开的面板队列


        private GameObject _UICanvas = null;            // UI画布
        private NameTable _NameTable = null;

        private Transform _TraUIRoot = null;            // UI根节点
        private Transform _TraNormalRoot = null;        // 普通全屏窗口根节点
        private Transform _TraFixedRoot = null;         // 固定显示的UI根节点
        private Transform _TraPopUpRoot = null;         // 弹出窗体的UI根节点

        private bool isLoaded = false;
        private bool isLoading = false;

        void Awake()
        {
            if (!isLoaded && !isLoading)
            {
                isLoading = true;
                PrefabLoader.LoadPrefab("ui/prefabs.u3dassetbundle", "UICanvas", LoadCallBack, false);
            }
        }

        void LoadCallBack(UnityEngine.Object obj)
        {
            isLoading = false;
            // 实例化UI根画布
            _UICanvas = Instantiate<GameObject>(obj as GameObject);
            DontDestroyOnLoad(_UICanvas);

            // 找到不同类型面板的根节点
            _NameTable = _UICanvas.GetComponent<NameTable>();
            _TraUIRoot = _NameTable.Find("UIRoot").transform;
            _TraNormalRoot = _NameTable.Find("NormalRoot").transform;
            _TraFixedRoot = _NameTable.Find("FixedRoot").transform;
            _TraPopUpRoot = _NameTable.Find("PopUpRoot").transform;

            // 把面板预制体信息写入（后期改用读取配置的方式）
            _DicViewPrefabPath.Add("LogonView", new ABAsset("ui/prefabs/logon.u3dassetbundle", "LogonPanel"));

            isLoaded = true;
        }

        /// <summary>
        /// 打开面板
        /// </summary>
        /// <param name="viewName">面板名称</param>
        public void Open(string viewName)
        {
            if (string.IsNullOrEmpty(viewName))
            {
                return;
            }
            _QueNeedOpen.Enqueue(viewName);
        }

        /// <summary>
        /// 关闭面板
        /// </summary>
        /// <param name="viewName"></param>
        public void Close(string viewName)
        {
            if (string.IsNullOrEmpty(viewName))
            {
                return;
            }
            if (_DicShowViews.ContainsKey(viewName))
            {
                _DicShowViews[viewName].Hiding();
                _DicShowViews.Remove(viewName);
            }
        }

        void Update()
        {
            if (!isLoaded)
            {
                return;
            }
            LoadView();
        }

        // 根据需加载队列，加载面板
        void LoadView()
        {
            if (_QueNeedOpen.Count > 0)
            {
                string viewName = _QueNeedOpen.Dequeue();                        // 从需加载队列中读取界面

                // 判断面板是否已经打开
                if (_DicShowViews.ContainsKey(viewName))
                {
                    return;
                }

                // 尝试从缓存中读取面板实例
                BaseView baseView = null;
                _DicViews.TryGetValue(viewName, out baseView);                 
                if (baseView != null)
                {
                    SetViewParent(baseView);
                    baseView.Open();
                    _DicShowViews.Add(viewName, baseView);
                    return;
                }

                // 如果没有从缓存中找到，则加载预制体，进行实例化
                if (!_DicViewPrefabPath.ContainsKey(viewName))
                {
                    Debug.LogError(GetType() + "没有找到面板：" + viewName + "的预制体");
                    return;
                }
                PrefabLoader.LoadPrefab(_DicViewPrefabPath[viewName], (prefab) => {
                    GameObject go = Instantiate(prefab as GameObject);
                    baseView = go.GetComponent<BaseView>();
                    if (baseView != null)
                    {
                        _DicViews.Add(viewName, baseView);
                        _DicShowViews.Add(viewName, baseView);
                        baseView.viewName = viewName;
                        SetViewParent(baseView);
                        baseView.Open();
                    }
                    else
                    {
                        Debug.LogError("预制体：" + prefab.name + "上没有挂在baseview脚本");
                    }
                });
            }
        }// function_end

        void SetViewParent(BaseView baseView)
        {
            UIViewType viewType = baseView.uiType.uiViewType;
            switch (viewType)
            {
                case UIViewType.Normal:
                    baseView.gameObject.transform.SetParent(_TraNormalRoot, false);
                    break;
                case UIViewType.Fixed:
                    baseView.gameObject.transform.SetParent(_TraFixedRoot, false);
                    break;
                case UIViewType.PopUp:
                    baseView.gameObject.transform.SetParent(_TraPopUpRoot, false);
                    break;
                default:
                    baseView.gameObject.transform.SetParent(_TraNormalRoot, false);
                    break;
            }
        }
    }//class_end
}
