/*
*Title:"AssetBundle框架"项目开发
*
*Description:
*	定义assetbundle框架内(非Editor)的委托、常量、枚举、静态变量
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
    /*委托定义区*/
    public delegate void DelLoadComplete(string abName);

    /*枚举定义区*/
    /// <summary>
    /// AB包分类--分类包（类包之间的AB包互不依赖）,如果互相依赖了会导致重复加载相同AB包。
    /// </summary>
    public enum BundleClassify
    {
        Normal,         //普通的默认分类
        ClassOne,
        ClassTwo,
    }

    public class AssetBundleDefined
    {
        /*常量定义区*/
        public const string ASSETBUNDLE_MANIFEST_STR = "AssetBundleManifest";   //读取清单的固定写法
        public const string SPRITE_ATLAS_NAME = "SpriteAtlas.spriteatlas";      //UI图片各文件夹内的图集文件名
        public const string NO_PACK_PATH_NAME = "/NoPack";                      //如果ui图片的文件路径包含/nopack则不打包进图集
    }
}
