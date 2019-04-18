/*
*Title:""项目开发
*
*Description:
*	[描述]
*
*Date:2017
*
*Version:0.1
*
*Modify Recoder:
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetBundleFramework
{
    public static class PathTool
    {

        private static string outPutPath = string.Empty;
        public static string assetBundelResourcesRoot = Application.dataPath + "/" + "AssetBundleResources";

        public static string ImagesDir = "Assets/AssetBundleResources/UI/Images";           //sprite图片存放路径

        public static string AssetConfigDir = "Assets/AssetBundleResources/AssetConfigs";   //unity资源配置存放路径

        public static string OutPutPath
        {
            get
            {
                return GetPlatformPath() + "/" + GetPlatformName();
            }
        }

        public static string GetPlatformPath()
        {
            string platformPath = string.Empty;
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsEditor:
                    platformPath = Application.streamingAssetsPath;
                    break;
                case RuntimePlatform.IPhonePlayer:
                case RuntimePlatform.Android:
                    platformPath = Application.persistentDataPath;
                    break;
            }
            return platformPath;
        }

        public static string GetPlatformName()
        {
            string platformName = string.Empty;
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsEditor:
                    platformName = "Windows";
                    break;
                case RuntimePlatform.IPhonePlayer:
                    platformName = "IPhone";
                    break;
                case RuntimePlatform.Android:
                    platformName = "Android";
                    break;
            }
            return platformName;
        }

        /// <summary>
        /// 获取下载AB包的路径
        /// </summary>
        /// <returns></returns>
        public static string GetWWWPath()
        {
            string strReturnWWWPath = string.Empty;
            switch (Application.platform)
            {
                // windows平台
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsEditor:
                    strReturnWWWPath = "file://" + OutPutPath;
                    break;
                //安卓平台
                case RuntimePlatform.Android:
                    strReturnWWWPath = "jar:file://" + OutPutPath;
                    break;
                default:
                    break;
            }
            return strReturnWWWPath;
        }
    }
}
