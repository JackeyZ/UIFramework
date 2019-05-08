/*
*Title:"AssetBundle框架"项目开发
*
*Description:
*	用于存储给定ab包的依赖关系
*       储存指定AB包的所有依赖关系包
*       储存指定AB包的所有引用关系包
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

namespace AssetBundleFramework { 
    public class ABRelation {
        private string _ABName;                         //当前AB包名称
        private List<string> _LisAllDependenceAB;       //所有依赖包名称集合
        private List<string> _LisAllReferenceAB;        //所有引用包名称集合

        public ABRelation(string abName)
        {
            _ABName = abName;
            _LisAllDependenceAB = new List<string>();
            _LisAllReferenceAB = new List<string>();
        }

        /* 依赖关系处理 */
        /// <summary>
        /// 增加依赖关系
        /// </summary>
        /// <param name="abName">AB包名称</param>
        /// <returns>true：增加依赖关系成功，false：依赖关系已存在</returns>
        public bool AddDependence(string abName)
        {
            if (!_LisAllDependenceAB.Contains(abName))
            {
                _LisAllDependenceAB.Add(abName);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 移除依赖关系
        /// </summary>
        /// <param name="abName">AB包名称</param>
        /// <returns>true：已经没有依赖关系，false：还存在依赖关系</returns>
        public bool RemoveDependence(string abName)
        {
            if (_LisAllDependenceAB.Contains(abName))
            {
                _LisAllDependenceAB.Remove(abName);
            }
            if(_LisAllDependenceAB.Count > 0)
            {
                return false;
            }
            return true;
        }
    
        /// <summary>
        /// 获取所有依赖关系
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllDependence()
        {
            return _LisAllDependenceAB;
        }

        /* 引用关系处理 */
        /// <summary>
        /// 增加被依赖(引用)关系
        /// </summary>
        /// <param name="abName">AB包名称</param>
        /// <returns>true：增加被依赖关系成功，false：被依赖关系已存在</returns>
        public bool AddReference(string abName)
        {
            if (!_LisAllReferenceAB.Contains(abName))
            {
                _LisAllReferenceAB.Add(abName);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 移除被依赖（引用）关系
        /// </summary>
        /// <param name="abName">AB包名称</param>
        /// <returns>true：已经没有被依赖（引用）关系，false：还存在被依赖（引用）关系</returns>
        public bool RemoveReference(string abName)
        {
            if (_LisAllReferenceAB.Contains(abName))
            {
                _LisAllReferenceAB.Remove(abName);
            }
            if (_LisAllReferenceAB.Count > 0)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 获取被依赖（引用）关系
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllReference()
        {
            return _LisAllReferenceAB;
        }
    }
}