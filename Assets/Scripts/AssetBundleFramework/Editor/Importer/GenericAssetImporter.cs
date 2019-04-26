using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using UnityEngine.U2D;
using UnityEditor.U2D;

namespace AssetBundleFramework
{
    class GenericAssetImporter : AssetPostprocessor
    {
        private static HashSet<string> tempIgnoreAssets = new HashSet<string>(); // 用于储存刚刚已经import的资源
        static Dictionary<string, string> needCheckSpriteAtlasDic = new Dictionary<string, string>();

        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (var importedAsset in importedAssets)
            {
                MarkAssetBundlePath(importedAsset);     //设置ab包标记
                ProcessTextureType(importedAsset);      //设置贴图类型
                //ProcessPackingTag(importedAsset);     //设置图集tag（旧版图集）

                AddNeedCheckSpriteAtlasList(importedAsset);
            }

            foreach (var movedAsset in movedAssets)
            {
                MarkAssetBundlePath(movedAsset);
                ProcessTextureType(movedAsset);
                //ProcessPackingTag(movedAsset);
                
                AddNeedCheckSpriteAtlasList(movedAsset);
            }

            foreach (var movedFromAsset in movedFromAssetPaths)
            {
                AddNeedCheckSpriteAtlasList(movedFromAsset);
            }

            foreach (var deleteddAsset in deletedAssets)
            {
                AddNeedCheckSpriteAtlasList(deleteddAsset);
            }
            CheckSpriteAtlasList();
        }

        static void AddNeedCheckSpriteAtlasList(string importedAsset)
        {
            int lastIndex = importedAsset.LastIndexOf(".");
            if (lastIndex != -1 && importedAsset.Substring(lastIndex, importedAsset.Length - lastIndex) == ".png")
            {
                int nameIndex = importedAsset.LastIndexOf("/");
                string path = importedAsset.Substring(0, nameIndex);

                if (path.Contains("/nopack") || path.Contains("/NoPack"))
                    return;

                if (!needCheckSpriteAtlasDic.ContainsKey(path))
                {
                    needCheckSpriteAtlasDic.Add(path, importedAsset);
                }
            }
        }

        static void CheckSpriteAtlasList()
        {
            foreach (var item in needCheckSpriteAtlasDic)
            {
                CheckSpriteAtlas(item.Value);
            }
            needCheckSpriteAtlasDic.Clear();
        }


        #region AB包标记
        /// <summary>
        /// 标记ab包路径
        /// </summary>
        /// <param name="assetPath"></param>
        private static void MarkAssetBundlePath(string assetPath)
        {
            FileInfo fileInfo = new FileInfo(assetPath);
            //参数检查
            if (string.IsNullOrEmpty(fileInfo.Extension) || fileInfo.Extension == ".meta")
                return;

            string fullPath = fileInfo.FullName.Replace('\\', '/');
            if (!fullPath.StartsWith(PathTool.assetBundelResourcesRoot))
            {
                return;
            }
            //得到AB包名称
            string assetBundleName = CalculationAssetBundleName(fileInfo);

            //设置AB包名和扩展名
            AssetImporter assetImporter = AssetImporter.GetAtPath(assetPath);
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
        /// 计算AB包路径名
        /// </summary>
        /// <param name="fileinfo"></param>
        /// <returns></returns>
        private static string CalculationAssetBundleName(FileInfo fileinfo)
        {
            string assetBundleName = string.Empty;
            //window下的路径
            string winPath = fileinfo.FullName;
            //unity下的操作的路径
            string unityPath = winPath.Replace('\\', '/'); //反斜杠替换成斜杠
                                                           //得到资源类型文件夹后的路径（二级目录相对路径 如：Textures/1.png）
            int subIndex = unityPath.IndexOf(PathTool.assetBundelResourcesRoot) + PathTool.assetBundelResourcesRoot.Length + 1;
            string typePath = unityPath.Substring(subIndex);
            //判断该文件是否不在一级目录
            if (typePath.Contains("/"))
            {
                assetBundleName = typePath.Substring(0, typePath.LastIndexOf("/"));
            }
            //如果类型相对路径不包含‘/’则表示该资源文件放在了场景资源文件夹下，与二级目录同级
            else
            {
                assetBundleName = fileinfo.Name;
            }
            return assetBundleName;
        }
        #endregion

        #region 贴图类型改为sprite
        /// <summary>
        /// 设置资源类型为sprite
        /// </summary>
        /// <param name="assetPath"></param>
        static private void ProcessTextureType(string assetPath)
        {
            AssetImporter assetImporter = AssetImporter.GetAtPath(assetPath);
            // 判断资源类型
            if (!(assetImporter is TextureImporter))
            {
                return;
            }
            TextureImporter textureImporter = assetImporter as TextureImporter;
            if (textureImporter.textureType != TextureImporterType.Sprite && textureImporter.assetPath.StartsWith(PathTool.ImagesDir))
            {
                textureImporter.textureType = TextureImporterType.Sprite;
                textureImporter.SaveAndReimport();
            }
        }
        #endregion

        #region 设置旧版图集的Tag（已弃用）
        /// <summary>
        /// 设置图集打包tag
        /// </summary>
        /// <param name="assetPath"></param>
        static private void ProcessPackingTag(string assetPath)
        {
            AssetImporter assetImporter = AssetImporter.GetAtPath(assetPath);
            // 判断资源类型
            if (!(assetImporter is TextureImporter))
            {
                return;
            }
            TextureImporter textureImporter = assetImporter as TextureImporter;
            if (textureImporter.assetPath.Contains("/nopack")
                || textureImporter.assetPath.Contains("/NoPack")
                || !textureImporter.assetPath.StartsWith(PathTool.ImagesDir))
            {
                textureImporter.spritePackingTag = string.Empty;
                return;
            }
            int subIndex = textureImporter.assetPath.IndexOf(PathTool.ImagesDir);
            string pack_tag = textureImporter.assetPath.Substring(subIndex + PathTool.ImagesDir.Length);
            pack_tag = "image" + pack_tag.Substring(0, pack_tag.LastIndexOf("/")).ToLower();
            if (textureImporter.spritePackingTag != pack_tag)
            {
                textureImporter.spritePackingTag = pack_tag;
                textureImporter.SaveAndReimport();
            }
        }
        #endregion

        #region 设置新版图集
        /// <summary>
        /// 重新设置对应目录下的SpriteAtlas
        /// </summary>
        /// <param name="assetPath"></param>
        static private void CheckSpriteAtlas(string assetPath)
        {
            if (!assetPath.StartsWith(PathTool.ImagesDir) || assetPath.Contains(AssetBundleDefined.NO_PACK_PATH_NAME))
            {
                return;
            }
            DirectoryInfo dirInfo = new DirectoryInfo(assetPath);   // 资源路径
            DirectoryInfo parentDirInfo = dirInfo.Parent;           // 资源父路径
            if(!parentDirInfo.Exists)
            {
                return;
            }
            // 检查当前目录图集是否已存在
            foreach (var item in parentDirInfo.GetFileSystemInfos())
            {
                if (item.Name == "SpriteAtlas.spriteatlas")
                {
                    //unity下的操作的路径
                    string unityPath = item.FullName.Replace('\\', '/');          //反斜杠替换成斜杠
                    AssetDatabase.DeleteAsset(unityPath.Substring(unityPath.IndexOf("Assets")));
                }
            }

            SpriteAtlas atlas = new SpriteAtlas();
            FileInfo fileInfo = new FileInfo(assetPath);
            // 这里我使用的是png图片，已经生成Sprite精灵了
            foreach (var item in parentDirInfo.GetFileSystemInfos())
            {
                FileInfo info = item as FileInfo;
                if (info is FileInfo && info.Extension == ".png")
                {
                    //unity下的操作的路径
                    string unityPath = item.FullName.Replace('\\', '/');          //反斜杠替换成斜杠
                    Object obj = AssetDatabase.LoadAssetAtPath<Sprite>(unityPath.Substring(unityPath.IndexOf("Assets")));
                    atlas.Add(new[] { obj });
                }
            }
            AssetDatabase.CreateAsset(atlas, assetPath.Substring(0, assetPath.Length - fileInfo.Name.Length) + AssetBundleDefined.SPRITE_ATLAS_NAME);
        }
        #endregion
    }
}


//// 设置参数 可根据项目具体情况进行设置
//SpriteAtlasPackingSettings packSetting = new SpriteAtlasPackingSettings()
//{
//    blockOffset = 1,
//    enableRotation = false,
//    enableTightPacking = false,
//    padding = 2,
//};
//atlas.SetPackingSettings(packSetting);

//SpriteAtlasTextureSettings textureSetting = new SpriteAtlasTextureSettings()
//{
//    readable = false,
//    generateMipMaps = false,
//    sRGB = true,
//    filterMode = FilterMode.Bilinear,
//};
//atlas.SetTextureSettings(textureSetting);

//TextureImporterPlatformSettings platformSetting = new TextureImporterPlatformSettings()
//{
//    maxTextureSize = 2048,
//    format = TextureImporterFormat.Automatic,
//    crunchedCompression = true,
//    textureCompression = TextureImporterCompression.Compressed,
//    compressionQuality = 50,
//};
//atlas.SetPlatformSettings(platformSetting); DirectoryInfo dir = new DirectoryInfo(_texturePath);