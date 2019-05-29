using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetBundleFramework
{
    /// <summary>
    /// 名称：AssetBundle管理器
    /// 作用：管理整个项目的AB包
    /// </summary>
    public class AssetBundleMgr : MonoSingleton<AssetBundleMgr>
    {
        AssetBundlePool _AssetBundlePool = new AssetBundlePool();
        //用于储存刚利用IEnumerator加载完的object
        private Hashtable _Ht;

        //AssetBundle （清单文件） 系统类
        private AssetBundleManifest _ManifestObj = null;

        void Awake()
        {
            _Ht = new Hashtable();
            //加载Manifest清单文件（主清单，记录着所有AB包之间的依赖关系）
            StartCoroutine(ABManifestLoader.Instance.ManifestLoad((succeed) => {
                if (succeed)
                {
                    _ManifestObj = ABManifestLoader.Instance.GetManifest();
                }
            }));
        }

        #region 同步加载

        #region 加载Bundle
        /// <summary>
        /// 同步加载AB包，安卓下可能会有问题（路径问题）
        /// </summary>
        /// <param name="bundleName">包名</param>
        /// <returns></returns>
        public AssetBundleItem LoadBundleSync(string bundleName)
        {
            if (_AssetBundlePool.BundleIsLoaded(bundleName))
            {
                return _AssetBundlePool.GetBundleItem(bundleName);
            }
            // 检查主Manifest清单文件是否加载完成
            if (!ABManifestLoader.Instance.IsLoadFinish)
            {
                Debug.LogError("主清单尚未加载完毕");
                return null;
            }
            // 检查目标包及其依赖包是否在异步加载中
            if (CheckBundleCanLoadSync(bundleName) == false)
            {
                Debug.LogError(bundleName + "或其依赖包在异步加载中，不能进行同步加载");
                return null;
            }

            AssetBundleItem bundleItem = _AssetBundlePool.GetBundleItem(bundleName);
            if(bundleItem == null)
            {
                bundleItem = _AssetBundlePool.AddBundleItem(bundleName);
            }

            bundleItem.bundleLoadStatus = BundleLoadStatus.LOADING;
            LoadBundleDependeceSync(bundleItem);

            return bundleItem;
        }

        /// <summary>
        /// 加载ab包并添加依赖项
        /// </summary>
        /// <param name="bundleItem">加载目标包</param>
        private void LoadBundleDependeceSync(AssetBundleItem bundleItem)
        {
            string[] strDependeceArray = ABManifestLoader.Instance.GetAssetBundleDependce(bundleItem.BundleName);
            foreach (var depend in strDependeceArray)
            {
                // 添加依赖项
                bundleItem.abRelation.AddDependence(depend);
                AssetBundleItem dependBundleItem = _AssetBundlePool.GetBundleItem(depend);
                if (dependBundleItem != null && _AssetBundlePool.BundleIsLoaded(depend))
                {
                    // 添加被依赖项
                    dependBundleItem.abRelation.AddReference(bundleItem.BundleName);
                    continue;
                }

                if (dependBundleItem == null)
                {
                    dependBundleItem = _AssetBundlePool.AddBundleItem(depend);
                }
                dependBundleItem.bundleLoadStatus = BundleLoadStatus.LOADING;
                // 添加被依赖项
                dependBundleItem.abRelation.AddReference(bundleItem.BundleName);
                LoadBundleDependeceSync(dependBundleItem);
            }

            bundleItem.LoadAssetBundleSync();
        }

        /// <summary>
        /// 检查ab包能否在进行异步加载，如果处于异步加载中，则不允许进行同步加载
        /// </summary>
        /// <returns></returns>
        public bool CheckBundleCanLoadSync(string bundleName)
        {
            AssetBundleItem bundleItem = _AssetBundlePool.GetBundleItem(bundleName);
            if(bundleItem != null && bundleItem.bundleLoadStatus == BundleLoadStatus.LOADING)
            {
                return false;
            }

            string[] strDependeceArray = ABManifestLoader.Instance.GetAssetBundleDependce(bundleName);
            foreach (var depend in strDependeceArray)
            {
                if (CheckBundleCanLoadSync(depend) == false)
                {
                    return false;
                }
            }
            return true;
        }

        #endregion

        #region 加载Asset
        /// <summary>
        /// 同步加载ab包资源
        /// </summary>
        /// <param name="bundleName">包名</param>
        /// <param name="assetName">资源名</param>
        /// <returns></returns>
        public UnityEngine.Object LoadBundleAssetSync(string bundleName, string assetName, bool cache = false)
        {
            if (_AssetBundlePool.BundleIsLoaded(bundleName))
            {
                return _AssetBundlePool.GetBundleItem(bundleName).LoadAsset(assetName, cache);
            }
            else
            {
                AssetBundleItem bundleItem = LoadBundleSync(bundleName);
                if(bundleItem != null)
                {
                    return bundleItem.LoadAsset(assetName, cache);
                }
                else
                {
                    return null;
                }
            }
        }
        #endregion

        #endregion


        #region 异步加载

        #region 加载asset
        public void LoadBundleAsset(string bundleName, string assetName, Action<UnityEngine.Object> loadcallback, bool isCache = false)
        {
            // 判断AB包是否已加载
            if(_AssetBundlePool.BundleIsLoaded(bundleName))
            {
                AssetBundleItem bundleItem = _AssetBundlePool.GetBundleItem(bundleName);
                UnityEngine.Object obj = bundleItem.LoadAsset(assetName, isCache);
                loadcallback(obj);
            }
            else
            {
                // 创建AB包加载完毕回调
                Action<bool, string> bundleLoadCallback = delegate (bool succeed, string assetbundleName)
                {
                    // 如果加载成功
                    if (succeed)
                    {
                        AssetBundleItem abItem = _AssetBundlePool.GetBundleItem(assetbundleName);
                        UnityEngine.Object obj = abItem.LoadAsset(assetName, isCache);
                        loadcallback(obj);
                    }
                    else
                    {
                        loadcallback(null);
                    }
                };
                // 先加载AB包
                LoadBundle(bundleName, bundleLoadCallback);
            }
        }
        public void LoadBundleAsset(ABAsset asset, Action<UnityEngine.Object> loadcallback, bool isCache = false)
        {
            LoadBundleAsset(asset.ABPath, asset.AssetName, loadcallback, isCache);
        }

        /// <summary>
        /// 外部调用，以IEnumerator方式加载资源，加载完的资源用GetYieldBundleAsset（）方法获取
        /// </summary>
        /// <param name="abName">AB包名称</param>
        /// <param name="assetName">包内资源名称</param>
        /// <param name="isCache">是否缓存</param>
        /// <param name="classify">分类包</param>
        public IEnumerator LoadBundleAsset(string abName, string assetName, bool isCache = true)
        {
            bool loadDone = false;              // 是否加载完成
            float loadTime = 20f;               // 最长加载时间，超过该加载时间还未完成则直接返回

            Action<UnityEngine.Object> assetLoadComplete = delegate (UnityEngine.Object prefab)
            {
                _Ht.Add(abName + ":" + assetName, prefab);
                loadDone = true;
            };

            LoadBundleAsset(abName, assetName, assetLoadComplete, isCache);

            while (!loadDone && loadTime > 0)
            {
                yield return null;
                loadTime -= Time.deltaTime;
            }
        }

        /// <summary>
        /// 外部调用，获取以IEnumerator方式加载的预制体，配合IEnumerator LoadBundleAsset（）方法使用
        /// </summary>
        /// <param name="abName">ab包名称</param>
        /// <param name="assetName">保内资源名称</param>
        /// <returns></returns>
        public UnityEngine.Object GetYieldBundleAsset(string abName, string assetName)
        {
            if (_Ht[abName + ":" + assetName] != null)
            {
                UnityEngine.Object obj = _Ht[abName + ":" + assetName] as UnityEngine.Object;
                _Ht.Remove(obj);
                return obj;
            }
            return null;
        }
        #endregion

        #region 加载bundle
        public void LoadBundle(string bundleName, Action<bool, string> loadCallback)
        {
            StartCoroutine(LoadBundleIEn(bundleName, loadCallback));
        }

        private IEnumerator LoadBundleIEn(string bundleName, Action<bool, string> loadCallback)
        {
            //等待主Manifest清单文件加载完成
            while (!ABManifestLoader.Instance.IsLoadFinish)
            {
                yield return null;
            }

            AssetBundleItem bundleItem = _AssetBundlePool.GetBundleItem(bundleName);
            // 检查是否已加载
            if (_AssetBundlePool.BundleIsLoaded(bundleName))
            {
                loadCallback(true, bundleName);
            }
            // 检查是否加载中
            else if (bundleItem != null && bundleItem.bundleLoadStatus == BundleLoadStatus.LOADING)
            {
                bundleItem.LoadCallback += loadCallback;
            }
            else
            {
                if(bundleItem == null)
                {
                    bundleItem = _AssetBundlePool.AddBundleItem(bundleName, loadCallback);
                }
                else
                {
                    bundleItem.LoadCallback += loadCallback;
                }
                bundleItem.bundleLoadStatus = BundleLoadStatus.LOADING;

                string[] strDependeceArray = ABManifestLoader.Instance.GetAssetBundleDependce(bundleItem.BundleName);
                foreach (var depend in strDependeceArray)
                {
                    // 添加依赖项
                    bundleItem.abRelation.AddDependence(depend);
                    // 先加载所有依赖的AB包并设置被依赖关系
                    yield return LoadReference(depend, bundleItem);
                }

                // 真正加载AB包
                yield return bundleItem.LoadAssetBundle();
            }
        }

        /// <summary>
        /// 加载依赖的AB包并设置被依赖关系
        /// </summary>
        /// <param name="dependBundleName">当前包依赖的AB包名称</param>
        /// <param name="bundleItem">当前包Item</param>
        /// <returns></returns>
        private IEnumerator LoadReference(string dependBundleName, AssetBundleItem bundleItem)
        {
            Action<bool, string> loadCompleteCallback = delegate (bool succeed, string assetBundleName)                 //协程加载完成的回调
            {
                if (succeed)
                {
                    //添加AB包被依赖关系（引用）
                    _AssetBundlePool.GetBundleItem(assetBundleName).abRelation.AddReference(bundleItem.BundleName);
                }
            };

            AssetBundleItem dependBundleItem = _AssetBundlePool.GetBundleItem(dependBundleName);
            //如果AB包已经加载
            if (dependBundleItem != null)
            {
                //添加AB包被依赖关系（引用）
                dependBundleItem.abRelation.AddReference(bundleItem.BundleName);
            }
            else {
                //开始加载依赖的包(这是一个递归调用)
                yield return LoadBundleIEn(dependBundleName, loadCompleteCallback);
            }
        }
        #endregion

        #endregion


        #region 释放
        /// <summary>
        /// 卸载ab包，顺带会把其所依赖的包也卸载掉
        /// </summary>
        /// <param name="abName"></param>
        public void DisposeAssetBundle(string abName)
        {
#if UNITY_EDITOR
            // 是否设置了使用assetbundle资源
            if (AssetBundleFramework.DeveloperSetting.GetUseAssetBundleAsset())
            {
                _AssetBundlePool.DisposeAssetBundle(abName);
            }
#else
            _AssetBundlePool.DisposeAssetBundle(abName);
#endif
        }

        /// <summary>
        /// 释放所有资源。（慎用）
        /// </summary>
        public void DisposeAllAssets()
        {
#if UNITY_EDITOR
            // 是否设置了使用assetbundle资源
            if (AssetBundleFramework.DeveloperSetting.GetUseAssetBundleAsset())
            {
                _AssetBundlePool.DisposeAllAssetBundle();
            }
#else
            _AssetBundlePool.DisposeAllAssetBundle();
#endif
        }
        #endregion
    }
}