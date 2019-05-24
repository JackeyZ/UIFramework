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
    public static void LoadSceneAsync(string abName, string sceneName, Action<AsyncOperation> loadCompleted = null, LoadSceneMode loadSceneMode = LoadSceneMode.Single, bool isCache = true)
    {
#if UNITY_EDITOR
        // 是否设置了使用assetbundle资源
        if (AssetBundleFramework.DeveloperSetting.GetUseAssetBundleAsset())
        {
            AssetBundleFramework.AssetBundleMgr.Instance.LoadBundleAsset(abName, null, (obj) => {
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
        AssetBundleFramework.AssetBundleMgr.Instance.LoadBundleAsset(abName, null, (obj) => {
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
            if (loadCompleted != null)
            {
                loadCompleted(operation);
            }
        });
#endif
    }
}
