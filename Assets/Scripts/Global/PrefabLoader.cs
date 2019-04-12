using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabLoader
{
    public static void LoadPrefab(string abName, string assetName, Action<UnityEngine.Object> LoadCompete)
    {
#if UNITY_EDITOR
        LoadCompete(AssetBundleFramework.AssetLoadInEditor.LoadObject<UnityEngine.Object>(abName, assetName));
#else
        AssetBundleFramework.AssetBundleMgr.Instance.LoadBundleAsset(abName, assetName, LoadCompete);
#endif
    }
}
