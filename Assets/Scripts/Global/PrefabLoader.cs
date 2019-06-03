using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabLoader
{
    public static void LoadPrefab(string abName, string assetName, Action<UnityEngine.Object> LoadCompete, bool isCache = true)
    {
#if UNITY_EDITOR
        // 是否设置了使用assetbundle资源
        if (AssetBundleFramework.DeveloperSetting.GetUseAssetBundleAsset())
        {
            AssetBundleFramework.AssetBundleMgr.Instance.LoadBundleAsset(abName, assetName, LoadCompete, isCache);
        }
        else
        {
            LoadCompete(AssetBundleFramework.AssetLoadInEditor.LoadObject<UnityEngine.Object>(abName, assetName));
        }
#else
        AssetBundleFramework.AssetBundleMgr.Instance.LoadBundleAsset(abName, assetName, LoadCompete, isCache);
#endif
    }
    public static void LoadPrefab(AssetBundleFramework.ABAsset abAsset, Action<UnityEngine.Object> LoadCompete, bool isCache = true)
    {
        LoadPrefab(abAsset.ABPath, abAsset.AssetName, LoadCompete, isCache);
    }

    public static UnityEngine.Object LoadPrefabSync(string abName, string assetName, bool isCache = false)
    {
#if UNITY_EDITOR
        if (AssetBundleFramework.DeveloperSetting.GetUseAssetBundleAsset())
        {
            return AssetBundleFramework.AssetBundleMgr.Instance.LoadBundleAssetSync(abName, assetName, isCache);
        }
        else
        {
            return AssetBundleFramework.AssetLoadInEditor.LoadObject<UnityEngine.Object>(abName, assetName);
        }
#else
        return AssetBundleFramework.AssetBundleMgr.Instance.LoadBundleAssetSync(abName, assetName, isCache);
#endif
    }

    public static UnityEngine.Object LoadPrefabSync(AssetBundleFramework.ABAsset abAsset, bool isCache = false)
    {
        return LoadPrefabSync(abAsset.ABPath, abAsset.AssetName, isCache);
    }
}
