using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
namespace UIFramework
{
    /// <summary>
    /// 名称：窗体基类
    /// 作用：
    ///     1、定义四个生命周期：
    ///         Display（显示状态）、Hiding（隐藏状态）、ReDisplay（再显示状态）、Freeze（冻结状态）
    ///     2、
    ///
    /// </summary>
    public class BaseView : MonoBehaviour
    {
        /*字段*/
        [SerializeField]
        public UIType uiType = new UIType();

        public string viewName;                 // 面板名称，由UIManager赋值

        /// <summary>
        /// 显示状态
        /// </summary>
        public virtual void Open()
        {
            this.gameObject.SetActive(true);
        }

        /// <summary>
        /// 隐藏状态
        /// </summary>
        public virtual void Hiding()
        {

        }

        /// <summary>
        /// 再显示状态
        /// </summary>
        public virtual void ReDisplay()
        {

        }

        /// <summary>
        /// 冻结状态
        /// </summary>
        public virtual void Freeze()
        {

        }
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(BaseView), true)]
    public class BaseViewEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            //获取脚本对象
            BaseView script = target as BaseView;
            script.uiType.uiViewType = (UIViewType)EditorGUILayout.EnumPopup("面板类型：", script.uiType.uiViewType);
            script.uiType.uiViewShowMode = (UIViewShowMode)EditorGUILayout.EnumPopup("面板显示类型：", script.uiType.uiViewShowMode);
            script.uiType.uiViewLucenyType = (UIViewLucenyType)EditorGUILayout.EnumPopup("面板背景类型：", script.uiType.uiViewLucenyType);

            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        }
    }

#endif
}
