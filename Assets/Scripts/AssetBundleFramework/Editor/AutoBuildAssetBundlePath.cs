/*
*Title:"AssetBundle框架"项目开发
*
*Description:
*	自动给资源文件添加ab包路径
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
    /*******************该类已弃用，导入资源自动标记AB包*********************/
    public class AutoBuildAssetBundlePath
    {
        //[MenuItem("AssetBundleTools/BuildAllAssetBundlePath")]
        public static void BuildAllAssetBundlePath()
        {
            /*
             DirectoryInfo 是文件夹信息
             FileInfo 是文件信息
             FileSystemInfo 是文件夹和文件信息

             FileSystemInfo可转化成DirectoryInfo
             FileSystemInfo可转化成FileInfo
             */
            string assetBundleResRoot = string.Empty;   //需要打包的资源根目录
            DirectoryInfo[] sceneDirArray = null;       //用于储存二级目录信息

            //清理无用的AB包名
            AssetDatabase.RemoveUnusedAssetBundleNames();
            //资源根目录
            assetBundleResRoot = PathTool.assetBundelResourcesRoot;
            //获取二级目录信息
            DirectoryInfo tempDirInfo = new DirectoryInfo(assetBundleResRoot);
            sceneDirArray = tempDirInfo.GetDirectories();
            //遍历二级目录，设置ab包名
            foreach (DirectoryInfo curDirInfo in sceneDirArray)
            {
                JudgeDirOrFileByRecursive(curDirInfo, curDirInfo.Name); //二级目录里文件夹名称就是场景名称，各自场景里的资源都打到同一个ab包里
            }

            //刷新资源
            AssetDatabase.Refresh();
            Debug.Log("设置AB包名称完成");
        }

        /// <summary>
        /// 判断传入的目录信息下的所有东西是文件还是文件夹，如果是文件则设置AB包，
        /// 如果是文件夹则递归调用该函数，直到给所有文件设置好AB包
        /// </summary>
        /// <param name="dirInfo">目录信息</param>
        /// <param name="sceneName">场景名称</param>
        private static void JudgeDirOrFileByRecursive(DirectoryInfo dirInfo, string sceneName)
        {
            FileSystemInfo fileSysInfo = dirInfo as FileSystemInfo;
            //判断文件是否存在
            if (!fileSysInfo.Exists)
            {
                Debug.LogError("文件或目录不存在：" + dirInfo + "请检查");
                return;
            }
            //遍历下级目录所有文件和文件夹
            FileSystemInfo[] fileSysInfoArray = dirInfo.GetFileSystemInfos();
            foreach (FileSystemInfo curFileSysInfo in fileSysInfoArray)
            {
                // 如果是文件
                if (curFileSysInfo is FileInfo)
                {
                    //设置AB包路径
                    BuildAssetBundlePath(curFileSysInfo as FileInfo, sceneName);
                }
                // 如果是文件夹
                else if (curFileSysInfo is DirectoryInfo)
                {
                    JudgeDirOrFileByRecursive(curFileSysInfo as DirectoryInfo, sceneName);
                }
            }
        }

        private static void BuildAssetBundlePath(FileInfo fileInfo, string sceneName)
        {
            //参数检查
            if (fileInfo.Extension == ".meta")
                return;

            //得到AB包名称
            string assetBundleName = CalculationAssetBundleName(fileInfo, sceneName);
            //得到资源文件的相对路径
            int tmpIndex = fileInfo.FullName.IndexOf("Assets");
            string relativePath = fileInfo.FullName.Substring(tmpIndex);
            //设置AB包名和扩展名
            AssetImporter assetImporter = AssetImporter.GetAtPath(relativePath);
            assetImporter.assetBundleName = assetBundleName;
            if (fileInfo.Extension == ".unity")
            {
                assetImporter.assetBundleVariant = "u3dScene";
            }
            else
            {
                assetImporter.assetBundleVariant = "u3dAssetBundle";
            }
        }

        /// <summary>
        /// 根据文件名和场景名计算ab包名
        /// 规则：场景名 + 资源类型（场景资源文件夹的二级目录，textures、prefabs、meterials等）
        /// </summary>
        /// <param name="fileinfo">文件信息</param>
        /// <param name="sceneName">场景名称</param>
        /// <returns>ab包名</returns>
        private static string CalculationAssetBundleName(FileInfo fileinfo, string sceneName)
        {
            string assetBundleName = string.Empty;
            //window下的路径
            string winPath = fileinfo.FullName;
            //unity下的操作的路径
            string unityPath = winPath.Replace('\\', '/'); //反斜杠替换成斜杠
                                                           //得到资源类型文件夹后的路径（二级目录相对路径 如：Textures/1.png）
            int subIndex = unityPath.IndexOf(sceneName) + sceneName.Length + 1;
            string typePath = unityPath.Substring(subIndex);
            //判断该文件是否在二级目录文件夹下
            if (typePath.Contains("/"))
            {
                string[] tmpStrArray = typePath.Split('/');
                //得到最后的AB包名
                assetBundleName = sceneName + "/" + tmpStrArray[0];
            }
            //如果类型相对路径不包含‘/’则表示该资源文件放在了场景资源文件夹下，与二级目录同级
            else
            {
                //得到最后的AB包名
                assetBundleName = sceneName + "/" + fileinfo.Name;
            }

            return assetBundleName;
        }
    }
}