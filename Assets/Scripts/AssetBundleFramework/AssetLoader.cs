﻿/*
*Title:"Assetbundle框架"项目开发
*
*Description:
*   名称：AB包内资源的加载类
*   作用：控制给定AB包内资源的加载、释放、缓存、查询
*	        1、加载与管理AB资源
*	        2、加载具有“缓存功能”的资源，带选用参数
*           3、卸载、释放AB资源
*           4、查看当前AB资源
*   用法：利用其他类加载完AB包之后赋值给该类对象，利用该类对资源进行管理
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
    public class AssetLoader
    {
        private AssetBundle _CurrentAssetBundle;    //当前AB包
        private Hashtable _Ht;                      //缓存容器集合

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ab">给定用WWW加载的AssetBundle的实例</param>
        public AssetLoader(AssetBundle ab)
        {
            if (ab != null)
            {
                _CurrentAssetBundle = ab;
                _Ht = new Hashtable();
            }
            else
            {
                Debug.LogError(GetType() + "AssetLoader构造函数参数为null");
            }
        }

        /// <summary>
        /// 加载当前AB包中指定的资源
        /// </summary>
        /// <param name="assetName">资源名</param>
        /// <param name="isCache">是否开启缓存</param>
        /// <returns></returns>
        public UnityEngine.Object LoadAsset(string assetName, bool isCache = true)
        {
            return LoadResource<UnityEngine.Object>(assetName, isCache);
        }

        /// <summary>
        /// 加载当前AB包的资源，
        /// 关于该资源的释放：返回的是一个Asset源对象，该Asset可能会克隆出其他对象，以及被其他对象引用，
        /// 所以释放的时候调用Resources.UnloadUnusedAssets比较方便管理，如果确定该资源没有被引用，则可以调用下面的UnLoadAsset
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="assetName">资源名称</param>
        /// <param name="isCache">是否需要缓存</param>
        /// <returns>加载的资源</returns>
        private T LoadResource<T>(string assetName, bool isCache = true) where T : UnityEngine.Object
        {
            //缓存集合是否已经存在
            if (_Ht.Contains(assetName))
            {
                return _Ht[assetName] as T;
            }

            //加载资源
            T tmpTResource = _CurrentAssetBundle.LoadAsset<T>(assetName);
            //判断是否加入缓存集合
            if (tmpTResource != null && isCache)
            {
                _Ht.Add(assetName, tmpTResource);
            }
            else if (tmpTResource == null)
            {
                Debug.LogError(GetType() + "资源：" + _CurrentAssetBundle.name + ":" + assetName + "加载失败请检查");
            }
            return tmpTResource;
        }

        public void ClearCache()
        {
            _Ht.Clear();
        }
        
        /// <summary>
        /// 卸载指定的资源,一般情况不会调用该接口
        /// 用Resources.UnloadUnusedAssets来释放资源
        /// </summary>
        /// <param name="asset">资源名称</param>
        /// <returns></returns>
        public bool UnLoadAsset(UnityEngine.Object asset)
        {
            if (asset != null)
            {
                if (_Ht.ContainsKey(asset.name))
                {
                    _Ht.Remove(asset.name);
                }
                Resources.UnloadAsset(asset);
                return true;
            }
            Debug.LogError(GetType() + "/UnLoadAsset()/参数 asset==null ,请检查！");
            return false;
        }
        
        /// <summary>
        /// 查询当前AssetBundle中包含的所有资源名称。
        /// </summary>
        /// <returns></returns>
        public string[] RetriveAllAssetName()
        {
            return _CurrentAssetBundle.GetAllAssetNames();
        }
    }
}
