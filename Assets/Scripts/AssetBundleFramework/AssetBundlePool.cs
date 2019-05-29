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
            if (_AssetBundleDic.ContainsKey(bundleName) && _AssetBundleDic[bundleName].bundleLoadStatus == BundleLoadStatus.LOADED)
            {
                return true;
            }
            return false;
        }

        public void DisposeAssetBundle(string abName)
        {
            AssetBundleItem bundleItem = null;
            _AssetBundleDic.TryGetValue(abName, out bundleItem);
            // 加载完的AB包才能卸载
            if (bundleItem != null)
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

                        // 后续要加一个当前AB包是否常驻内存的判断

                        // 如果目标包所依赖的包已经没有被其他包依赖了，则把目标包所依赖的包也卸载掉（递归）
                        if (isClear)
                        {
                            DisposeAssetBundle(DependAbName);
                        }
                    }
                }
                else
                {
                    Debug.LogError(abName + "包还被其他包所依赖，不能卸载"); // 暂时为Error，其实不是重要报错
                }

            }
        }// function_end

        public void DisposeAllAssetBundle()
        {
            foreach (var item in _AssetBundleDic)
            {
                item.Value.Dispose();                                                   // 卸载ab包
                _AssetBundleDic.Remove(item.Key);
            }
        }
        #endregion
    }
}
