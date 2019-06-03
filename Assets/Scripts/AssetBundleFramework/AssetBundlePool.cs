using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetBundleFramework
{
    /// <summary>
    /// 名称：Bundle池
    /// 作用：管理所有已加载的AB包
    /// </summary>
    public class AssetBundlePool
    {
        Dictionary<string, AssetBundleItem> _AssetBundleDic = new Dictionary<string, AssetBundleItem>();

        public const float MAX_UNLOAD_TIME = 10;            // bundle包多久不使用则释放

        // 常驻内存AB包配置
        private AssetBundlePermanentAsset abPermanentAsset = null;

        #region 私有方法

        #endregion

        #region 公共方法
        public AssetBundleItem GetBundleItem(string bundleName)
        {
            if (_AssetBundleDic.ContainsKey(bundleName))
                return _AssetBundleDic[bundleName];
            else
                return null;
        }

        public AssetBundleItem AddBundleItem(string bundleName, Action<bool, string> loadCallbcak = null)
        {
            AssetBundleItem assetBundleItem = new AssetBundleItem(bundleName, loadCallbcak);
            _AssetBundleDic.Add(bundleName, assetBundleItem);
            return assetBundleItem;
        }

        public bool BundleIsLoaded(string bundleName)
        {
            if (_AssetBundleDic.ContainsKey(bundleName) && _AssetBundleDic[bundleName].BundleLoadStatus == BundleLoadStatus.LOADED)
            {
                return true;
            }
            return false;
        }

        #region 释放相关
        /// <summary>
        /// 释放ab包的接口，所以AB包释放都调用这个方法
        /// </summary>
        /// <param name="abName"></param>
        public void DisposeAssetBundle(string abName)
        {
            AssetBundleItem bundleItem = null;
            _AssetBundleDic.TryGetValue(abName, out bundleItem);
            // 加载完的AB包才能卸载
            if (bundleItem != null && bundleItem.BundleLoadStatus == BundleLoadStatus.LOADED)
            {
                //不在常驻内存列表的AB包才能卸载
                if (abPermanentAsset == null || !abPermanentAsset.BundleNameHS.Contains(abName))
                {
                    ABRelation abRelation = bundleItem.abRelation;
                    // 没有被别的包依赖才可以进行卸载
                    if (abRelation.GetAllReference().Count == 0)
                    {
                        bundleItem.Dispose();                                                   // 卸载目标ab包
                        _AssetBundleDic.Remove(abName);
                        List<string> dependenceList = abRelation.GetAllDependence();            // 获取目标包所有依赖的包列表
                        foreach (string DependAbName in dependenceList)
                        {
                            bool isClear = _AssetBundleDic[DependAbName].abRelation.RemoveReference(abName); // 去掉目标包所依赖的包的被依赖关系

                            // 如果目标包所依赖的包已经没有被其他包依赖了，则把目标包所依赖的包也卸载掉（递归）
                            if (isClear)
                            {
                                DisposeAssetBundle(DependAbName);
                            }
                        }
                    }
                }
            }
        }// function_end

        /// <summary>
        /// 卸载所有AB包内存镜像,但不卸载对象
        /// </summary>
        public void DisposeAllAssetBundle()
        {
            List<string> needUnLoadBundleList = new List<string>();
            foreach (var item in _AssetBundleDic)
            {
                needUnLoadBundleList.Add(item.Value.BundleName);
            }

            foreach (string needUnLoadBundleName in needUnLoadBundleList)
            {
                DisposeAssetBundle(needUnLoadBundleName);
            }

            Resources.UnloadUnusedAssets();
        }

        /// <summary>
        /// 清空所有AB包的资源加载缓存
        /// </summary>
        public void ClearAllBundleItemCache()
        {
            foreach (var item in _AssetBundleDic)
            {
                item.Value.ClearLoaderCache();                                            // 清空ab包资源loader的缓存
            }
            Resources.UnloadUnusedAssets();
        }

        /// <summary>
        /// 检查AssetBundleItem上次的使用时间，判断是否卸载内存镜像
        /// </summary>
        public void CheckBundleLastUseTime()
        {
            List<string> needUnLoadBundleList = new List<string>();
            float nowTime = Time.unscaledTime;
            foreach (var item in _AssetBundleDic) {
                // 检查ab包上次使用的时间
                if (nowTime > item.Value.lastUseTimeStamp + MAX_UNLOAD_TIME)
                {
                    needUnLoadBundleList.Add(item.Value.BundleName);
                }
            }

            foreach (string needUnLoadBundleName in needUnLoadBundleList)
            {
                DisposeAssetBundle(needUnLoadBundleName);                              // 卸载AB包
            }
        }
        #endregion

        #endregion

        #region 常驻内存AssetConfig加载
        /// <summary>
        /// 常驻内存AssetConfig加载(AssetBundleMgr初始化之后调用)
        /// </summary>
        public void LoadAssetBundlePermanentAsset()
        {
            string abName = "assetconfigs.u3dassetbundle";
            string assetName = "AssetBundlePermanentAsset";

            AssetBundleMgr.Instance.LoadBundleAsset(abName, assetName, (obj) =>
            {
                if (obj != null)
                {
                    abPermanentAsset = obj as AssetBundlePermanentAsset;
                }
                else
                {
                    Debug.LogError("常驻内存ab包配置加载失败");
                }
            }, false);

        }
        #endregion
    }
}
