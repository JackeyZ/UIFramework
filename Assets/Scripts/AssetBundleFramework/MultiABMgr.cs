/*
*Title:"Assetbundle框架"项目开发
*
*Description:
*   多个AB包加载类--------加载所需包的时候自动加载其依赖的AB包
*           1、获取AB包之间的依赖和引用关系
*           2、管理Assetbundle之间的自动连锁（递归）加载机制
*
*Date:2019
*
*Version:0.1
*
*Modify Recoder:
*/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetBundleFramework
{
    public class MultiABMgr
    {
        // 单个AB包加载对象(Loader)
        private SingleABLoader _CurrentSingleABLoader;
        // 单个AB包Loader缓存集合
        private Dictionary<string, SingleABLoader> _DicSingleABLoaderCache;
        // 当前AB包名称
        private string _CurrentABName;
        // AB包对应的依赖-引用关系集合，同时可用于判断AB包是否已经加载
        private Dictionary<string, ABRelation> _DicABRelation;
        // 所有AB包是否加载完成
        private Dictionary<string, Action<string>> _LoadAllAssetBundleCompleteList;

        public MultiABMgr(string abName)
        {
            _CurrentABName = abName;
            _DicSingleABLoaderCache = new Dictionary<string, SingleABLoader>();
            _DicABRelation = new Dictionary<string, ABRelation>();
            _LoadAllAssetBundleCompleteList = new Dictionary<string, Action<string>>();
        }

        public void CompleteLoadAB(string abName)
        {
            if (_LoadAllAssetBundleCompleteList.ContainsKey(abName))    // 是否有需要回调的函数（目标AB包才会有，一般情况下依赖包不会有所需回调的函数,除非依赖包恰好是目标包）
            {
                Debug.Log(abName + "，加载完毕");
                _LoadAllAssetBundleCompleteList[abName](abName);
                ClearLoadCallBack(abName);
            }
        }

        /// <summary>
        /// 外部调用， 加载目标AB包
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="loadCallback"></param>
        /// <returns></returns>
        public IEnumerator LoadAssetBundle(string abName, Action<string> loadCallback)
        {
            AddLoadCallBack(abName, loadCallback);
            yield return LoadAssetBundle(abName);
        }

        /// <summary>
        /// 内部调用, 加载AB包
        /// </summary>
        /// <param name="abName"></param>
        /// <returns></returns>
        private IEnumerator LoadAssetBundle(string abName)
        {
            // AB包关系的建立
            if (!_DicABRelation.ContainsKey(abName))
            {
                //建立关系
                ABRelation abRelation = new ABRelation(abName);
                _DicABRelation.Add(abName ,abRelation);
            }
            ABRelation tempABRelation = _DicABRelation[abName];     // 获得当前AB包的依赖关系

            // 得到指定AB包所有的依赖关系（查询Manifest清单）
            string[] strDependeceArray = ABManifestLoader.Instance.GetAssetBundleDependce(abName);
            foreach (var depend in strDependeceArray)
            {
                // 添加依赖项
                tempABRelation.AddDependence(depend);
                // 先加载依赖的AB包并设置被依赖关系
                yield return LoadReference(depend, abName);
            }

            //真正加载AB包
            if (_DicSingleABLoaderCache.ContainsKey(abName))
            {
                yield return _DicSingleABLoaderCache[abName].LoadAssetBundle();
            }
            else
            {
                _CurrentSingleABLoader = new SingleABLoader(abName, CompleteLoadAB);
                _DicSingleABLoaderCache.Add(abName, _CurrentSingleABLoader);
                yield return _CurrentSingleABLoader.LoadAssetBundle();
            }
        }

        /// <summary>
        /// 加载依赖的AB包并设置被依赖关系
        /// </summary>
        /// <param name="abName">当前包依赖的AB包名称</param>
        /// <param name="refABName">当前的AB包名称</param>
        /// <returns></returns>
        private IEnumerator LoadReference(string abName, string refABName)
        {
            //如果AB包已经加载
            if (_DicABRelation.ContainsKey(abName))
            {
                ABRelation tmpABRelationObj = _DicABRelation[abName];
                //添加AB包引用关系（被依赖）
                tmpABRelationObj.AddReference(refABName);
            }
            else {
                ABRelation tmpABRelationObj = new ABRelation(abName);
                tmpABRelationObj.AddReference(refABName);
                _DicABRelation.Add(abName, tmpABRelationObj);

                //开始加载依赖的包(这是一个递归调用)
                yield return LoadAssetBundle(abName);
            }
        }

        /// <summary>
        /// 加载（AB包中）资源
        /// </summary>
        /// <param name="abName">AssetBunlde 名称</param>
        /// <param name="assetName">资源名称</param>
        /// <param name="isCache">是否使用（资源）缓存</param>
        /// <returns></returns>
        public UnityEngine.Object LoadAsset(string abName, string assetName, bool isCache = true)
        {
            foreach (string item_abName in _DicSingleABLoaderCache.Keys)
            {
                if (abName == item_abName)
                {
                    return _DicSingleABLoaderCache[item_abName].LoadAsset(assetName, isCache);
                }
            }
            Debug.LogError(GetType() + "/LoadAsset()/找不到AsetBunder包，无法加载资源，请检查！ abName=" + abName + " assetName=" + assetName);
            return null;
        }

        /// <summary>
        /// 指定AB包是否已经加载完毕
        /// </summary>
        /// <param name="abName"></param>
        /// <returns></returns>
        public bool AssetBundleIsLoaded(string abName)
        {
            return _DicSingleABLoaderCache.ContainsKey(abName) && _DicSingleABLoaderCache[abName].Loaded;
        }
        /// <summary>
        /// 指定AB包是否正在加载中
        /// </summary>
        /// <param name="abName"></param>
        /// <returns></returns>
        public bool AssetBundleIsLoading(string abName)
        {
            return _DicSingleABLoaderCache.ContainsKey(abName) && _DicSingleABLoaderCache[abName].Loading;
        }

        /// <summary>
        /// 设置目标AB包加载完毕回调
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="loadCallback"></param>
        public void AddLoadCallBack(string abName, Action<string> loadCallback)
        {
            if (loadCallback != null)
            {
                if (_LoadAllAssetBundleCompleteList.ContainsKey(abName))
                {
                    _LoadAllAssetBundleCompleteList[abName] += loadCallback;
                }
                else
                {
                    _LoadAllAssetBundleCompleteList.Add(abName, loadCallback);
                }
            }
        }

        /// <summary>
        /// 内部调用，清掉刚调用完毕的回调函数
        /// </summary>
        /// <param name="abName"></param>
        private void ClearLoadCallBack(string abName)
        {
            Delegate[] delArray = _LoadAllAssetBundleCompleteList[abName].GetInvocationList();
            for (int i = 0; i < delArray.Length; i++)
            {
                _LoadAllAssetBundleCompleteList[abName] -= delArray[i] as Action<string>;
            }
        }

        /// <summary>
        /// 释放所有的资源
        /// </summary>
        public void DisposeAllAsset()
        {
            try
            {
                //逐一释放所有加载过的AssetBundel 包中的资源
                foreach (SingleABLoader item_sABLoader in _DicSingleABLoaderCache.Values)
                {
                    item_sABLoader.DisposeAll();
                }
            }
            finally
            {
                _DicSingleABLoaderCache.Clear();
                _DicSingleABLoaderCache = null;

                //释放其他对象占用资源
                _DicABRelation.Clear();
                _DicABRelation = null;
                _CurrentABName = null;
                _LoadAllAssetBundleCompleteList.Clear();
                _LoadAllAssetBundleCompleteList = null;

                //卸载没有使用到的资源
                Resources.UnloadUnusedAssets();
                //强制垃圾收集
                System.GC.Collect();
            }
        }
    }
}
