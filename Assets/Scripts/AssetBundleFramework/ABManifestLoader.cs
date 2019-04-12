/*
*Title:"Assetbundle框架"项目开发
*
*Description:
*   用于AB包依赖清单的加载（加载主清单，记录着每个AB包之间的依赖关系）
*
*Date:2019
*
*Version:0.1
*
*Modify Recoder:
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace AssetBundleFramework
{
    public class ABManifestLoader : Singleton<ABManifestLoader>, System.IDisposable
    {
        private AssetBundleManifest _manifest;      //主清单
        private AssetBundle _manifestBundle;
        private string _loadPath;                   //主清单所在路径
        private bool _isLoadFinish;                 //是否加载完成

        public bool IsLoadFinish{ get { return _isLoadFinish; } }

        public ABManifestLoader()
        {
            _manifest = null;
            _loadPath = PathTool.GetWWWPath() + "/" + PathTool.GetPlatformName();
            _isLoadFinish = false;
        }

        /// <summary>
        /// 加载依赖清单
        /// </summary>
        /// <returns></returns>
        public IEnumerator ManifestLoad()
        {
            using (UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(_loadPath))
            {
                yield return request.SendWebRequest();
                _manifestBundle = (request.downloadHandler as DownloadHandlerAssetBundle).assetBundle;
                if (_manifestBundle == null)
                {
                    Debug.LogError(GetType() + "读取manifest失败：" + _loadPath);
                    yield return null;
                }
                _manifest = _manifestBundle.LoadAsset(AssetBundleDefined.ASSETBUNDLE_MANIFEST_STR) as AssetBundleManifest;
                _isLoadFinish = true;
            }
        }

        public AssetBundleManifest GetManifest()
        {
            if (_isLoadFinish)
            {
                if (_manifest != null)
                    return _manifest;
                else
                    Debug.LogError(GetType() + "_manifest is null 请检查");
            }
            else
            {
                Debug.LogError(GetType() + "manifest尚未加载完毕");
            }
            return null;
        }

        /// <summary>
        /// 获取assetbundle所有依赖包
        /// </summary>
        /// <returns></returns>
        public string[] GetAssetBundleDependce(string abName)
        {
            return GetManifest().GetDirectDependencies(abName);
        }

        /// <summary>
        /// 释放manifest对应的Assetbundle
        /// </summary>
        public void Dispose()
        {
            if(_manifestBundle != null)
            {
                _manifestBundle.Unload(true);  
            }
        }

    }
}