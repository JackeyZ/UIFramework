/*
*Title:"AssetBundle框架"项目开发
*
*Description:
*	打包assetbundle
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
using UnityEditor;
using System.IO;
namespace AssetBundleFramework { 
    public class BuildAssetBundle {
        [MenuItem("AssetBundleTools/BuildAllAssetBundle")]
        /// <summary>
        /// 打包assetbundle
        /// </summary>
        public static void BuildAllAssetBundle()
        {
            //AB包输出路径
            string ABOutPath = string.Empty;

            //获取“StreamAssets”路径
            ABOutPath = PathTool.OutPutPath;

            //判断输出目录文件夹是否存在
            if (Directory.Exists(ABOutPath))
            {
                DeleteAssetBundle.DelAssetBundle();
            }
            Directory.CreateDirectory(ABOutPath);

            //打包生成
            BuildPipeline.BuildAssetBundles(ABOutPath, BuildAssetBundleOptions.None, AssetBundleConst.buildTarget); //BuildAssetBundleOptions.UncompressedAssetBundle
            //刷新
            AssetDatabase.Refresh();
        }

    }
}