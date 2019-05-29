using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader
{
    /// <summary>
    /// 异步加载场景
    /// </summary>
    /// <param name="abName"></param>
    /// <param name="sceneName"></param>
    /// <param name="loadCompleted"></param>
    /// <param name="loadSceneMode"></param>
    public static void LoadSceneAsync(string abName, string sceneName, Action<AsyncOperation> loadCompleted = null, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
    {
#if UNITY_EDITOR
        // 是否设置了使用assetbundle资源
        if (AssetBundleFramework.DeveloperSetting.GetUseAssetBundleAsset())
        {
            AssetBundleFramework.AssetBundleMgr.Instance.LoadBundle(abName, (succeed, bundleName) =>
            {
                AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
                if (loadCompleted != null)
                {
                    loadCompleted(operation);
                }
            });
        }
        else
        {
            // 根据包名获取资源路径，因为后缀名可能不一样所以获取的是数组
            string[] paths = AssetDatabase.GetAssetPathsFromAssetBundle(abName);
            if (paths.Length == 0)
            {
                Debug.LogError("AB包路径有误：" + abName);
                if (loadCompleted != null)
                {
                    loadCompleted(null);
                }
                return;
            }
            LoadSceneParameters param = new LoadSceneParameters(loadSceneMode);
            AsyncOperation operation = EditorSceneManager.LoadSceneAsyncInPlayMode(paths[0], param);
            if (loadCompleted != null)
            {
                loadCompleted(operation);
            }
        }
#else
        AssetBundleFramework.AssetBundleMgr.Instance.LoadBundle(abName, (succeed, bundleName) => {
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
            if (loadCompleted != null)
            {
                loadCompleted(operation);
            }
        });
#endif
    }

    /// <summary>
    /// 同步加载场景
    /// </summary>
    /// <param name="abName"></param>
    /// <param name="sceneName"></param>
    /// <param name="loadSceneMode"></param>
    /// <returns></returns>
    public static bool LoadSceneSync(string abName, string sceneName, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
    {
#if UNITY_EDITOR
        // 是否设置了使用assetbundle资源
        if (AssetBundleFramework.DeveloperSetting.GetUseAssetBundleAsset())
        {
            return AssetBundleFramework.AssetBundleMgr.Instance.LoadBundleSync(abName) != null;
        }
        else
        {
            // 根据包名获取资源路径，因为后缀名可能不一样所以获取的是数组
            string[] paths = AssetDatabase.GetAssetPathsFromAssetBundle(abName);
            if (paths.Length == 0)
            {
                Debug.LogError("AB包路径有误：" + abName);
                return false;
            }
            LoadSceneParameters param = new LoadSceneParameters(loadSceneMode);
            EditorSceneManager.LoadSceneInPlayMode(paths[0], param);
            return true;
        }
#else
        return AssetBundleFramework.AssetBundleMgr.Instance.LoadBundleSync(abName) != null;
#endif
    }
}