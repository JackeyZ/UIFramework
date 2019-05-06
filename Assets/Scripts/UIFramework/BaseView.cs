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
    [RequireComponent(typeof(NameTable))]
    public class BaseView : MonoBehaviour
    {
        /*字段*/
        [SerializeField]
        public UIType uiType = new UIType();

        [SerializeField]
        private ViewOpenStatus _viewOpenStatus = ViewOpenStatus.Close;  // 打开状态
        [SerializeField]
        private ViewShowStatus _viewShowStatus = ViewShowStatus.Hide;   // 显示状态，只有打开面板之后这个状态才有用

        private NameTable _comNameTable;

        private Dictionary<string, GameObject> _GameObjects;

        private ViewOpenStruct dataStruct;

        public ViewOpenStruct DataStruct
        {
            get
            {
                return dataStruct;
            }

            set
            {
                dataStruct = value;
                if(value.data != null)
                {
                    SetDataCallback(value.data);
                }
            }
        }
        
        /// <summary>
        /// 获得NameTable绑定的Gameobject
        /// </summary>
        public Dictionary<string, GameObject> GameObjects
        {
            get
            {
                if (_comNameTable == null)
                {
                    _comNameTable = GetComponent<NameTable>();
                }

                return _comNameTable.DicGameObject;
            }
        }

        public ViewOpenStatus ViewOpenStatus{ get { return _viewOpenStatus; } }

        public ViewShowStatus ViewShowStatus { get { return _viewShowStatus; } }

        public virtual void Open()
        {
            _viewOpenStatus = ViewOpenStatus.Open;
            OpenCallback();
        }

        protected virtual void OpenCallback()
        {

        }

        /// <summary>
        /// 用于外部调用，关闭窗口
        /// </summary>
        public virtual void ClickClose()
        {
            UIManager.Instance.Close(dataStruct.asset.ToString());
        }

        /// <summary>
        /// 只允许UIManager调用，显示状态
        /// </summary>
        public virtual void Display()
        {
            this.gameObject.SetActive(true);
            _viewShowStatus = ViewShowStatus.Show;
            DisplayCallback();
        }

        protected virtual void DisplayCallback()
        {

        }

        /// <summary>
        /// 只允许UIManager调用，隐藏面板
        /// </summary>
        public virtual void Hide()
        {
            this.gameObject.SetActive(false);
            _viewShowStatus = ViewShowStatus.Hide;
            HideCallback();
        }
        protected virtual void HideCallback()
        {

        }

        /// <summary>
        /// 只允许UIManager调用，关闭面板
        /// </summary>
        public virtual void Close()
        {
            this.gameObject.SetActive(false);
            _viewShowStatus = ViewShowStatus.Hide;
            _viewOpenStatus = ViewOpenStatus.Close;
            CloseCallback();
        }
        protected virtual void CloseCallback()
        {

        }

        public virtual void SetData(object data)
        {
            this.dataStruct.data = data;
            SetDataCallback(data);
        }
        protected virtual void SetDataCallback(object data)
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

            // 面板打开状态
            ViewOpenStatus viewStatus = script.ViewOpenStatus;
            switch (viewStatus)
            {
                case ViewOpenStatus.Close:
                    EditorGUILayout.LabelField("面板打开状态：关闭");
                    break;
                case ViewOpenStatus.Open:
                    EditorGUILayout.LabelField("面板打开状态：打开");

                    // 面板显示状态
                    ViewShowStatus viewShowStatus = script.ViewShowStatus;
                    switch (viewShowStatus)
                    {
                        case ViewShowStatus.Show:
                            EditorGUILayout.LabelField("面板显示状态：显示");
                            break;
                        case ViewShowStatus.Hide:
                            EditorGUILayout.LabelField("面板显示状态：隐藏");
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }

            script.uiType.uiViewType = (UIViewType)EditorGUILayout.EnumPopup("面板类型：", script.uiType.uiViewType);
            if (script.uiType.uiViewType == UIViewType.Normal)
            {
                script.uiType.uiViewShowMode = (UIViewShowMode)EditorGUILayout.EnumPopup("面板显示类型：", script.uiType.uiViewShowMode);
                script.uiType.uiViewLucenyType = (UIViewLucenyType)EditorGUILayout.EnumPopup("面板背景类型：", script.uiType.uiViewLucenyType);
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        }
    }

#endif
}
