/***
 *
 *   Title: "AssetBundle简单框架"项目
 *          框架主流程(第4层): 所有“场景”的AssetBundle 管理。
 *
 *   Description:
 *          功能： 
 *             1: 提取“Menifest 清单文件”，缓存本脚本。
 *             2：以“分类包”为单位，管理整个项目中所有的AssetBundle 包。
 *
 *   Author: Liuguozhu
 *
 *   Date: 2017.10
 *
 *   Modify：  
 *
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace AssetBundleFramework
{
	public class AssetBundleMgr : MonoSingleton<AssetBundleMgr>
    {
        //分类包集合
        private Dictionary<BundleClassify, MultiABMgr> _DicAllClassify = new Dictionary<BundleClassify, MultiABMgr>();
        //用于储存刚利用IEnumerator加载完的object
        private Hashtable _Ht;
        //AssetBundle （清单文件） 系统类
        private AssetBundleManifest _ManifestObj = null;

        void Awake()
        {
            _Ht = new Hashtable();
            //加载Manifest清单文件（主清单，记录着所有AB包之间的依赖关系）
            StartCoroutine(ABManifestLoader.Instance.ManifestLoad());
        }

        #region 私有函数
        /// <summary>
        /// 下载AssetBundle 指定包
        /// </summary>
        /// <param name="abName">AssetBundle 包名称</param>
        /// <param name="loadAllCompleteHandle">委托： 下载完成回调</param>
        /// <param name="classify">分类包（类包之间资源应当互不依赖）</param>
        /// <returns></returns>
        private IEnumerator LoadAssetBundlePack(string abName, Action<string> loadAllCompleteHandle, BundleClassify classify = BundleClassify.Normal)
        {
            //参数检查
            if (string.IsNullOrEmpty(abName))
            {
                Debug.LogError(GetType()+ "/LoadAssetBundlePack()/abName is null ,请检查！");
                yield return null;
            }

            //等待Manifest清单文件加载完成
            while (!ABManifestLoader.Instance.IsLoadFinish)
            {
                yield return null;
            }
            _ManifestObj = ABManifestLoader.Instance.GetManifest();
            if (_ManifestObj == null)
            {
                Debug.LogError(GetType() + "/LoadAssetBundlePack()/_ManifestObj is null ,请先确保加载Manifest清单文件！");
                yield return null;
            }

            //把当前AB分类包加入集合中。
            if (!_DicAllClassify.ContainsKey(classify))
            {
                MultiABMgr multiMgrObj = new MultiABMgr(abName);
                _DicAllClassify.Add(classify, multiMgrObj);
            }

            //调用下一层（“多包管理类”）
            MultiABMgr tmpMultiMgrObj = _DicAllClassify[classify];
            if (tmpMultiMgrObj==null)
            {
                Debug.LogError(GetType() + "/LoadAssetBundlePack()/tmpMultiMgrObj is null ,请检查！");
            }
            //调用“多包管理类”的加载指定AB包。
            yield return tmpMultiMgrObj.LoadAssetBundle(abName, loadAllCompleteHandle);
        }//Method_end
        
        /// <summary>
        /// 加载(AB 包中)资源，AB包加载完成之后才能调用
        /// </summary>
        /// <param name="scenesName">场景名称</param>
        /// <param name="abName">AssetBundle 包名称</param>
        /// <param name="assetName">资源名称</param>
        /// <param name="isCache">是否使用缓存</param>
        /// <returns></returns>
        private UnityEngine.Object LoadAsset(string abName, string assetName,bool isCache = true, BundleClassify classify = BundleClassify.Normal)
        {
            if (_DicAllClassify.ContainsKey(classify))
            {
                MultiABMgr multObj = _DicAllClassify[classify];
                return multObj.LoadAsset(abName, assetName, isCache);
            }
            Debug.LogError(GetType()+ "/LoadAsset()/找不到分类包，无法加载（AB包中）资源,请检查！  scenesName="+ classify);
            return null;
        }
        #endregion

        #region 公用函数
        /// <summary>
        /// 外部调用，加载资源用
        /// </summary>
        /// <param name="abName">AB包名称</param>
        /// <param name="assetName">包内资源名称</param>
        /// <param name="assetLoadComplete">回调函数</param>
        /// <param name="isCache">是否缓存</param>
        /// <param name="classify">分类包</param>
        public void LoadBundleAsset(string abName, string assetName, Action<UnityEngine.Object> assetLoadComplete, bool isCache = true, BundleClassify classify = BundleClassify.Normal)
        {
            Action<string> loadCompleteCallback = delegate (string assetBundleName)                 //AB包加载完成的回调
            {
                LoadBundleAsset(abName, assetName, assetLoadComplete, isCache, classify);
            };

            if (!_DicAllClassify.ContainsKey(classify))                                             //判断当前分类包是否已经创建
            {
                StartCoroutine(LoadAssetBundlePack(abName, loadCompleteCallback, classify));        //创建分类包加载器并加载给定AB包
                return;
            }

            MultiABMgr tmpMultiABMgr = _DicAllClassify[classify];
            if (!tmpMultiABMgr.AssetBundleIsLoaded(abName))                                         //判断AB包是否已经加载
            {
                if (tmpMultiABMgr.AssetBundleIsLoading(abName))                                     //判断AB包是否正在加载
                {
                    tmpMultiABMgr.AddLoadCallBack(abName, loadCompleteCallback);
                    return;
                }
                StartCoroutine(tmpMultiABMgr.LoadAssetBundle(abName, loadCompleteCallback));        //加载AB包
                return;
            }
            
            // 如果ab路径的扩展名是.u3dscene说明加载的是场景，则只加载ab包，不加载ab资源（场景（scene）不需要加载资源，只需要加载包）
            if (Path.GetExtension(abName) == ".u3dscene")
            {
                assetLoadComplete(null);                                                            //资源加载完成调用回调函数
            }
            else
            {
                assetLoadComplete(LoadAsset(abName, assetName, isCache, classify));                 //资源加载完成调用回调函数，参数为加载进来的资源（若AB包已经加载了，则这是一个同步加载）
            }
        }

        /// <summary>
        /// 外部调用，加载资源用（重载函数）
        /// </summary>
        /// <param name="asset">资源数据</param>
        /// <param name="assetLoadComplete">回调函数</param>
        /// <param name="isCache">是否缓存</param>
        /// <param name="classify">分类包</param>
        public void LoadBundleAsset(ABAsset asset, Action<UnityEngine.Object> assetLoadComplete, bool isCache = true, BundleClassify classify = BundleClassify.Normal)
        {
            LoadBundleAsset(asset.ABPath, asset.AssetName, assetLoadComplete, isCache, classify);
        }

        /// <summary>
        /// 外部调用，以IEnumerator方式加载资源，加载完的资源用GetYieldBundleAsset（）方法获取
        /// </summary>
        /// <param name="abName">AB包名称</param>
        /// <param name="assetName">包内资源名称</param>
        /// <param name="isCache">是否缓存</param>
        /// <param name="classify">分类包</param>
        public IEnumerator LoadBundleAsset(string abName, string assetName, bool isCache = true, BundleClassify classify = BundleClassify.Normal)
        {
            bool loadDone = false;              // 是否加载完成
            float loadTime = 20f;               // 最长加载时间，超过该加载时间还未完成则直接返回

            Action<UnityEngine.Object> assetLoadComplete = delegate (UnityEngine.Object prefab)
            {
                _Ht.Add(abName + ":" + assetName, prefab);
                loadDone = true;
            };

            LoadBundleAsset(abName, assetName, assetLoadComplete, isCache, classify);

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
            if(_Ht[abName + ":" + assetName] != null)
            {
                UnityEngine.Object obj = _Ht[abName + ":" + assetName] as UnityEngine.Object;
                _Ht.Remove(obj);
                return obj;
            }
            return null;
        }

        /// <summary>
        /// 卸载ab包，顺带会把其所依赖的包也卸载掉
        /// </summary>
        /// <param name="abName"></param>
        public void DisposeAssetBundle(string abName, BundleClassify classify = BundleClassify.Normal)
        {
#if UNITY_EDITOR
            // 是否设置了使用assetbundle资源
            if (AssetBundleFramework.DeveloperSetting.GetUseAssetBundleAsset())
            {
                if (_DicAllClassify.ContainsKey(classify))
                {
                    _DicAllClassify[classify].DisposeAssetBundle(abName);
                }
                else {
                    Debug.LogError(GetType() + "/DisposeAllAssets()/找不到分类包名称，无法释放资源，请检查！  classify=" + classify);
                }
            }
#else
            if (_DicAllClassify.ContainsKey(classify))
            {
                _DicAllClassify[classify].DisposeAssetBundle(abName);
            }
            else {
                Debug.LogError(GetType() + "/DisposeAllAssets()/找不到分类包名称，无法释放资源，请检查！  classify=" + classify);
            }
#endif
        }

        /// <summary>
        /// 释放分类包的所有资源。（慎用）
        /// </summary>
        /// <param name="classify">分类包名称</param>
        public void DisposeAllAssets(BundleClassify classify)
        {
            if (_DicAllClassify.ContainsKey(classify))
            {
                MultiABMgr multObj = _DicAllClassify[classify];
                multObj.DisposeAllAsset();
                _DicAllClassify.Remove(classify);
            }
            else {
                Debug.LogError(GetType() + "/DisposeAllAssets()/找不到分类包名称，无法释放资源，请检查！  classify=" + classify);
            }
        }
        #endregion

    }//Class_end
}


