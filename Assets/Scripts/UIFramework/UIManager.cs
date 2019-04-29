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
        private Dictionary<string, BaseView> _DicNormalViews = new Dictionary<string, BaseView>();                  // 用于缓存所有普通UI窗体
        private Dictionary<string, BaseView> _DicShowNormalViews = new Dictionary<string, BaseView>();              // 用于储存当前显示的普通UI窗体(show窗体)
        private Dictionary<string, BaseView> _DicFixedViews = new Dictionary<string, BaseView>();                   // 用于缓存所有固定UI窗体
        private Dictionary<string, BaseView> _DicShowFixedViews = new Dictionary<string, BaseView>();               // 用于储存当前显示的固定UI窗体(show窗体)

        private List<BaseView> _ListOpenView = new List<BaseView>();                                                // 当前Open的窗体有序列表（不包括popup的面板）

        private Queue<ViewOpenStruct> _QueNeedOpen = new Queue<ViewOpenStruct>();                                   // 需要打开的面板队列

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

            isLoaded = true;
        }

        /// <summary>
        /// 打开面板
        /// </summary>
        public void Open(string abName, string assetName, object data = null)
        {
            if (string.IsNullOrEmpty(abName) || string.IsNullOrEmpty(assetName))
            {
                return;
            }
            ViewOpenStruct viewOpenStruct = new ViewOpenStruct();
            viewOpenStruct.asset = new ABAsset(abName, assetName);
            viewOpenStruct.data = data;
            _QueNeedOpen.Enqueue(viewOpenStruct);
        }
        public void Open(ABAsset asset, object data = null)
        {
            Open(asset.ABPath, asset.AssetName, data);
        }

        /// <summary>
        /// 外部调用，跳转到其他面板
        /// </summary>
        /// <param name="fromAsset">哪个面板资源调用的</param>
        /// <param name="asset">ab资源</param>
        /// <param name="data">数据</param>
        public void Jump(ABAsset fromAsset, ABAsset asset, object data = null)
        {
            Close(fromAsset.ToString());
            Open(asset, data);
        }
        public void Jump(string fromAbName, string fromAssetName, string abName, string assetName, object data = null)
        {
            Jump(new ABAsset(fromAbName, fromAssetName), new ABAsset(abName, assetName));
        }

        /// <summary>
        /// 外部调用,关闭面板，只对normal和fixed有效
        /// </summary>
        /// <param name="viewKey"></param>
        public void Close(string viewKey)
        {
            if (string.IsNullOrEmpty(viewKey))
            {
                return;
            }
            if (_DicShowNormalViews.ContainsKey(viewKey))
            {
                CloseView(_DicShowNormalViews[viewKey]);
            }
            else if (_DicShowFixedViews.ContainsKey(viewKey))
            {
                CloseView(_DicShowFixedViews[viewKey]);
            }
        }
        public void Close(ABAsset asset)
        {
            Close(asset.ToString());
        }
        public void Close(string abName, string assetName)
        {
            if (string.IsNullOrEmpty(abName) || string.IsNullOrEmpty(assetName))
            {
                return;
            }
            Close(new ABAsset(abName, assetName));
        }

        /// <summary>
        /// 外部调用，根据UI类型，关闭所有对应的UI
        /// </summary>
        /// <param name="viewType"></param>
        public void CloseViewByUIType(UIViewType viewType = UIViewType.Normal)
        {
            switch (viewType)
            {
                case UIViewType.Normal:
                    List<BaseView> needCloseNormalViewList = new List<BaseView>();

                    foreach (var keyValue in _DicShowNormalViews)
                    {
                        needCloseNormalViewList.Add(keyValue.Value);
                    }

                    foreach (var baseView in needCloseNormalViewList)
                    {
                        CloseView(baseView);
                    }

                    needCloseNormalViewList.Clear();
                    break;
                case UIViewType.Fixed:
                    List<BaseView> needCloseFixedViewList = new List<BaseView>();

                    foreach (var keyValue in _DicShowFixedViews)
                    {
                        needCloseFixedViewList.Add(keyValue.Value);
                    }

                    foreach (var baseView in needCloseFixedViewList)
                    {
                        CloseView(baseView);
                    }

                    needCloseFixedViewList.Clear();
                    break;
                case UIViewType.PopUp:
                    foreach (var PopUpViewTransform in _TraPopUpRoot)
                    {
                        BaseView baseView = (PopUpViewTransform as Transform).gameObject.GetComponent<BaseView>();
                        CloseView(baseView);
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 外部调用，关闭所有UI
        /// </summary>
        public void CloseAllView()
        {
            CloseViewByUIType(UIViewType.PopUp);
            CloseViewByUIType(UIViewType.Normal);
            CloseViewByUIType(UIViewType.Fixed);
        }

        /// <summary>
        /// 外部调用， 根据类型隐藏面板
        /// </summary>
        /// <param name="viewType"></param>
        public void HideViewByUIType(UIViewType viewType = UIViewType.Normal)
        {
            switch (viewType)
            {
                case UIViewType.Normal:
                    List<BaseView> needHideNormalViewList = new List<BaseView>();

                    foreach (var keyValue in _DicShowNormalViews)
                    {
                        needHideNormalViewList.Add(keyValue.Value);
                    }

                    foreach (var baseView in needHideNormalViewList)
                    {
                        HideView(baseView);
                    }
                    needHideNormalViewList.Clear();
                    break;
                case UIViewType.Fixed:
                    List<BaseView> needHideFixedViewList = new List<BaseView>();

                    foreach (var keyValue in _DicShowFixedViews)
                    {
                        needHideFixedViewList.Add(keyValue.Value);
                    }

                    foreach (var baseView in needHideFixedViewList)
                    {
                        HideView(baseView);
                    }

                    needHideFixedViewList.Clear();
                    break;
                case UIViewType.PopUp:
                    foreach (var PopUpViewTransform in _TraPopUpRoot)
                    {
                        BaseView baseView = (PopUpViewTransform as Transform).gameObject.GetComponent<BaseView>();
                        CloseView(baseView);
                    }
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 隐藏所有面板，外部调用
        /// </summary>
        public void HideAllView()
        {
            HideViewByUIType(UIViewType.PopUp);
            HideViewByUIType(UIViewType.Normal);
            HideViewByUIType(UIViewType.Fixed);
        }

        /// <summary>
        /// 面板是否显示出来
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public bool IsDisplay(ABAsset asset)
        {
            return _DicShowFixedViews.ContainsKey(asset.ToString()) || _DicShowNormalViews.ContainsKey(asset.ToString());
        }
        /// <summary>
        /// 面板是否显示出来
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public bool IsDisplay(string abName, string assetName)
        {
            ABAsset asset = new ABAsset(abName, assetName);
            return IsDisplay(asset);
        }

        /// <summary>
        /// 面板是否打开，打开不一定显示，也可能是隐藏状态
        /// </summary>
        /// <param name="baseView"></param>
        /// <returns></returns>
        public bool IsOpen(BaseView baseView)
        {
            return _ListOpenView.Contains(baseView);
        }
        public bool IsOpen(ABAsset asset)
        {
            foreach (var baseView in _ListOpenView)
            {
                if(baseView.DataStruct.asset == asset) 
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsOpen(string abName, string assetName)
        {
            return IsOpen(new ABAsset(abName, assetName));
        }

        #region 私有方法
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
                ViewOpenStruct viewOpenStruct = _QueNeedOpen.Dequeue();                        // 从需加载队列中读取界面

                // 判断面板是否已经显示出来
                if (IsDisplay(viewOpenStruct.asset))
                {
                    // 由于已经打开所以缓存中一定会有面板实例
                    BaseView tempBaseView = GetBaseViewFromCache(viewOpenStruct.asset);
                    DisplayView(tempBaseView, viewOpenStruct);
                    return;
                }

                // 尝试从缓存中读取面板实例
                BaseView baseView = GetBaseViewFromCache(viewOpenStruct.asset);
                
                if (baseView != null)
                {
                    SetViewParent(baseView);
                    OpenView(baseView, viewOpenStruct);
                    return;
                }
                
                PrefabLoader.LoadPrefab(viewOpenStruct.asset, (obj) => {
                    GameObject prefab = obj as GameObject;
                    baseView = prefab.GetComponent<BaseView>();
                    GameObject go = null;
                    if (baseView != null)
                    {
                        switch (baseView.uiType.uiViewType)
                        {
                            case UIViewType.Normal:
                                go = Instantiate(prefab);
                                baseView = go.GetComponent<BaseView>();
                                _DicNormalViews.Add(viewOpenStruct.asset.ToString(), baseView);
                                break;
                            case UIViewType.Fixed:
                                go = Instantiate(prefab);
                                baseView = go.GetComponent<BaseView>();
                                _DicFixedViews.Add(viewOpenStruct.asset.ToString(), baseView);
                                break;
                            case UIViewType.PopUp:
                                go = GameObjectPool.Instance.InstantiateGO(prefab);
                                baseView = go.GetComponent<BaseView>();
                                break;
                            default:
                                break;
                        }
                        SetViewParent(baseView);
                        OpenView(baseView, viewOpenStruct);
                    }
                    else
                    {
                        Debug.LogError("预制体：" + prefab.name + "上没有挂载baseview脚本");
                    }
                });
            }
        }// function_end

        // 关闭面板
        void CloseView(BaseView baseView)
        {
            switch (baseView.uiType.uiViewType)
            {
                case UIViewType.Normal:
                    if (_DicShowNormalViews.ContainsKey(baseView.DataStruct.asset.ToString()))
                        _DicShowNormalViews.Remove(baseView.DataStruct.asset.ToString());
                    break;
                case UIViewType.Fixed:
                    if (_DicShowFixedViews.ContainsKey(baseView.DataStruct.asset.ToString()))
                        _DicShowFixedViews.Remove(baseView.DataStruct.asset.ToString());
                    break;
            }
            _ListOpenView.Remove(baseView);
            baseView.Close();

            // 当隐藏其他面板关闭的时候，按顺序逐个显示所有隐藏的面板
            if (baseView.uiType.uiViewType == UIViewType.Normal && baseView.uiType.uiViewShowMode == UIViewShowMode.HideOther)
            {
                List<BaseView> tempList = new List<BaseView>();
                foreach (var item in _ListOpenView)
                {
                    tempList.Add(item);
                }
                
                foreach (BaseView item in tempList)
                {
                    DisplayView(item, item.DataStruct);
                }
            }
        }

        // 隐藏面板
        void HideView(BaseView baseView)
        {
            switch (baseView.uiType.uiViewType)
            {
                case UIViewType.Normal:
                    if (_DicShowNormalViews.ContainsKey(baseView.DataStruct.asset.ToString()))
                        _DicShowNormalViews.Remove(baseView.DataStruct.asset.ToString());
                    break;
                case UIViewType.Fixed:
                    if (_DicShowFixedViews.ContainsKey(baseView.DataStruct.asset.ToString()))
                        _DicShowFixedViews.Remove(baseView.DataStruct.asset.ToString());
                    break;
            }
            baseView.Hide();
        }

        // 打开面板，面板未打开才允许调用该函数
        void OpenView(BaseView baseView, ViewOpenStruct dataStruct)
        {
            if (!IsOpen(baseView))
            {
                if(baseView.uiType.uiViewType != UIViewType.PopUp)
                {
                    _ListOpenView.Add(baseView);
                }
                baseView.Open();
            }
            DisplayView(baseView, dataStruct);
        }

        // 显示面板
        void DisplayView(BaseView baseView, ViewOpenStruct dataStruct)
        {
            // 如果面板的展示类型是隐藏其他类型，则隐藏其他所有显示的面板
            if (baseView.uiType.uiViewType == UIViewType.Normal && baseView.uiType.uiViewShowMode == UIViewShowMode.HideOther)
            {
                HideAllView();
            }

            baseView.DataStruct = dataStruct;
            AddViewToShowDic(dataStruct.asset, baseView);

            // 把面板放到列表最后
            if (_ListOpenView.Contains(baseView))
            {
                _ListOpenView.Remove(baseView);
                _ListOpenView.Add(baseView);
            }
            baseView.transform.SetSiblingIndex(baseView.transform.parent.childCount);

            baseView.Display();
        }

        #region 私有工具函数
        /// <summary>
        /// 设置面板父节点
        /// </summary>
        /// <param name="baseView"></param>
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

        /// <summary>
        /// 从缓存获取面板
        /// </summary>
        /// <returns></returns>
        BaseView GetBaseViewFromCache(ABAsset asset)
        {
            BaseView baseView = null;
            _DicNormalViews.TryGetValue(asset.ToString(), out baseView);
            if (baseView != null)
            {
                return baseView;
            }
            _DicFixedViews.TryGetValue(asset.ToString(), out baseView);
            if (baseView != null)
            {
                return baseView;
            }
            return baseView;
        }

        /// <summary>
        /// 把面板加到显示字典中
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="view"></param>
        void AddViewToShowDic(ABAsset asset, BaseView view)
        {
            switch (view.uiType.uiViewType)
            {
                case UIViewType.Normal:
                    if (!_DicShowNormalViews.ContainsKey(asset.ToString()))
                        _DicShowNormalViews.Add(asset.ToString(), view);
                    break;
                case UIViewType.Fixed:
                    if (!_DicShowFixedViews.ContainsKey(asset.ToString()))
                        _DicShowFixedViews.Add(asset.ToString(), view);
                    break;
            }
        }
        #endregion

        #endregion
    }//class_end

    public struct ViewOpenStruct{
        public ABAsset asset;
        public object data;
    }
}
