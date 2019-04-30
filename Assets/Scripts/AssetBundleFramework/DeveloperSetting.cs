using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AssetBundleFramework
{
    /// <summary>
    /// 名称：
    /// 作用：
    /// </summary>
    public class DeveloperSetting
    {
#if UNITY_EDITOR
        [MenuItem("AssetBundleTools/Setting/UseAssetBundleAsset")]
        public static void SetUseAssetBundleAsset()
        {
            PlayerPrefs.SetInt("UseAssetBundleAsset", PlayerPrefs.GetInt("UseAssetBundleAsset") == 1 ? 0 : 1);
        }

        [MenuItem("AssetBundleTools/Setting/UseAssetBundleAsset", true)]
        public static bool MenuUseAssetBundleAssetCheck()
        {
            Menu.SetChecked("AssetBundleTools/Setting/UseAssetBundleAsset", GetUseAssetBundleAsset());
            return true;
        }

        /// <summary>
        /// 在editor下是否读取打包出来的assetbundle资源，如果不读取则根据ab路径读取本地资源
        /// </summary>
        /// <returns></returns>
        public static bool GetUseAssetBundleAsset()
        {
            return PlayerPrefs.GetInt("UseAssetBundleAsset") == 1;
        }
#endif
    }

}