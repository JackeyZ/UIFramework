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
using System.IO;
using UnityEngine;
using UnityEditor;
namespace AssetBundleFramework
{
    public class DeleteAssetBundle : MonoBehaviour
    {
        /// <summary>
        /// 删除打出来的AB包
        /// </summary>
        [MenuItem("AssetBundleTools/DeleteAssetBundle")]
        public static void DelAssetBundle()
        {
            string needDeletePath = PathTool.OutPutPath;
            if (!string.IsNullOrEmpty(needDeletePath))
            {
                Directory.Delete(needDeletePath, true); //true表示允许删除非空文件夹
                File.Delete(needDeletePath + ".meta");
                //刷新
                AssetDatabase.Refresh();
            }
        }
    }
}
