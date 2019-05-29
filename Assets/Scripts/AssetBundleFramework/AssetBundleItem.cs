using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace AssetBundleFramework {
    /// <summary>
    /// AB包加载状态
    /// </summary>
    public enum BundleLoadStatus
    {
        WAIT_LOAD,              // 未加载
        LOADING,                // 加载中
        LOADED,                 // 加载成功
        LOAD_FAIL,              // 加载失败
        DISPOSED,               // 已释放
    }

    /// <summary>
    /// 名称：单个AB包管理类
    /// 作用：用于记录单个AB包的状态以及管理单个AB包
    /// </summary>
    public class AssetBundleItem : System.IDisposable
    {
        public string BundleName;                              // ab包名
        public ABRelation abRelation;                          // 依赖关系
        public BundleLoadStatus bundleLoadStatus;              // 加载状态
        public Action<bool, string> LoadCallback;              // 加载完成回调（参数1：加载是否成功, 参数2：加载成功的AB包名）
        public AssetBundle assetBundle;
        public string _ABDownLoadPath;

        private AssetLoader _AssetLoader;

        public AssetBundleItem(string bundleName, Action<bool, string> loadCallbcak)
        {
            BundleName = bundleName;
            abRelation = new ABRelation(bundleName);
            LoadCallback = loadCallbcak;
            bundleLoadStatus = BundleLoadStatus.WAIT_LOAD;
            _ABDownLoadPath = PathTool.GetWWWPath() + "/" + bundleName;
        }

        public void LoadAssetBundleSync()
        {
            assetBundle = AssetBundle.LoadFromFile(_ABDownLoadPath);
            if (assetBundle == null)
            {
                Debug.LogError(GetType() + "LoadAssetBundle失败：" + _ABDownLoadPath);
                LoadCallback(false, BundleName);
            }
            else
            {
                _AssetLoader = new AssetLoader(assetBundle);
                bundleLoadStatus = BundleLoadStatus.LOADED;
                if (LoadCallback != null)
                {
                    LoadCallback(true, BundleName);
                }
            }
        }

        public IEnumerator LoadAssetBundle()
        {
            using (UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(_ABDownLoadPath))
            {
                request.SendWebRequest();
                while (!request.isDone)
                {
                    //Debug.Log(_ABDownLoadPath + "下载中（" + request.downloadProgress * 100 + "%）");
                    yield return null;
                }

                //取得ab的方式1
                //AssetBundle ab_prefab = DownloadHandlerAssetBundle.GetContent(request);
                //取得ab的方式2
                assetBundle = (request.downloadHandler as DownloadHandlerAssetBundle).assetBundle;
                if (bundleLoadStatus != BundleLoadStatus.DISPOSED)
                {
                    if (assetBundle == null)
                    {
                        Debug.LogError(GetType() + "LoadAssetBundle失败：" + _ABDownLoadPath);
                        bundleLoadStatus = BundleLoadStatus.LOAD_FAIL;
                        LoadCallback(false, BundleName);
                    }
                    else
                    {
                        _AssetLoader = new AssetLoader(assetBundle);
                        bundleLoadStatus = BundleLoadStatus.LOADED;
                        if (LoadCallback != null)
                        {
                            LoadCallback(true, BundleName);
                        }
                    }
                }
                else
                {
                    if (assetBundle != null)
                        assetBundle.Unload(false);
                }
            }
        }

        /// <summary>
        /// 外部调用加载AB包内的资源
        /// </summary>
        /// <param name="assetName">包内资源名称</param>
        /// <param name="isCache">是否缓存包内资源</param>
        /// <returns></returns>
        public UnityEngine.Object LoadAsset(string assetName, bool isCache = true)
        {
            if (_AssetLoader != null)
            {
                return _AssetLoader.LoadAsset(assetName, isCache);
            }
            Debug.LogError(GetType() + "/LoadAsset()/ 参数_AssetLoader == null  ,请检查！" + ":" + assetName);
            return null;
        }

        /// <summary>
        /// 卸载ab资源
        /// </summary>
        /// <param name="asset"></param>
        public void UnLoadAsset(UnityEngine.Object asset)
        {
            if (_AssetLoader != null)
            {
                _AssetLoader.UnLoadAsset(asset);
            }
            else
            {
                Debug.LogError(GetType() + "：_AssetLoader is null");
            }
        }

        private void ClearLoadCallBack()
        {
            Delegate[] delArray = LoadCallback.GetInvocationList();
            for (int i = 0; i < delArray.Length; i++)
            {
                LoadCallback -= delArray[i] as Action<bool, string>;
            }
        }

        public void Dispose()
        {
            bundleLoadStatus = BundleLoadStatus.DISPOSED;
            if (assetBundle != null)
                assetBundle.Unload(false);
        }

        public void DisposeAll()
        {
            bundleLoadStatus = BundleLoadStatus.DISPOSED;
            if (assetBundle != null)
                assetBundle.Unload(true);
        }
    }
}