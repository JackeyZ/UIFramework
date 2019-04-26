using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetBundleFramework
{
    /// <summary>
    /// 名称：AB资源类
    /// 作用：存储资源名称与资源AB路径
    /// </summary>
    public class ABAsset
    {
        private string _ABPath;
        private string _AssetName;

        public string ABPath
        {
            get
            {
                return _ABPath;
            }
        }

        public string AssetName
        {
            get
            {
                return _AssetName;
            }
        }
        /// <summary>
        /// 资源构造函数
        /// </summary>
        /// <param name="abName">AB包名</param>
        /// <param name="assetName">资源名称</param>
        public ABAsset(string abName, string assetName)
        {
            _ABPath = abName;
            _AssetName = assetName;
        }

        public override string ToString()
        {
            return _ABPath + ":" + _AssetName;
        }
    }
}
