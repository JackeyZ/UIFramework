using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// 名称：AB包常驻内存配置工具
/// 作用：用于创建AB包常驻内存配置
/// </summary>
public class AssetBundlePermanentAssetConfigTools
{
    // 创建AB包常驻内存配置
    [MenuItem("AssetConfigTools/CreatAssetBundlePermanentAsset")]
    static void CreatePreLoadAsset()
    {
        // 实例化类  Bullet
        ScriptableObject bullet = ScriptableObject.CreateInstance<AssetBundlePermanentAsset>();

        // 如果实例化 Bullet 类为空，返回
        if (!bullet)
        {
            Debug.LogWarning("Bullet not found");
            return;
        }
        // 配置保存的路径
        string path = AssetBundleFramework.PathTool.AssetConfigDir;
        // 如果项目不包含该路径，创建一个
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);  
        }


        //将类名 Bullet 转换为字符串
        //拼接保存自定义资源（.asset） 路径
        path = string.Format((string)(AssetBundleFramework.PathTool.AssetConfigDir + "/{0}.asset"), (typeof(AssetBundlePermanentAsset).ToString()));

        // 生成自定义资源到指定路径
        AssetDatabase.CreateAsset(bullet, path);
    }
}
