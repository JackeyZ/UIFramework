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
    ///     1、定义面板类型：见UIType类
    ///     2、设置面板状态：
    ///         打开（关闭）状态、显示（隐藏）状态
    /// </summary>
    [RequireComponent(typeof(NameTable))]
    public class BaseView : MonoBehaviour
    {
        #region 字段
        [SerializeField]
        public UIType uiType = new UIType();                            // 面板类型

        [SerializeField]
        private ViewOpenStatus _viewOpenStatus = ViewOpenStatus.Close;  // 打开状态
        [SerializeField]
        private ViewShowStatus _viewShowStatus = ViewShowStatus.Hide;   // 显示状态，如果面板处于关闭状态，那么面板将会是隐藏状态

        private NameTable _comNameTable;                                // NameTabel组件，第一次获取GameObjects的时候初始化

        private Dictionary<string, GameObject> _GameObjects;

        private ViewOpenStruct dataStruct;
        #endregion

        #region 属性
        /// <summary>
        /// 面板数据
        /// </summary>
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

        /// <summary>
        /// 面板打开状态
        /// </summary>
        public ViewOpenStatus ViewOpenStatus{ get { return _viewOpenStatus; } }

        /// <summary>
        /// 面板显示状态
        /// </summary>
        public ViewShowStatus ViewShowStatus { get { return _viewShowStatus; } }

        #endregion

        #region 公共方法
        /// <summary>
        /// 用于外部调用，用于设置面板数据
        /// </summary>
        /// <param name="data"></param>
        public virtual void SetData(object data)
        {
            this.dataStruct.data = data;
            SetDataCallback(data);
        }

        /// <summary>
        /// 用于外部调用，关闭窗口
        /// </summary>
        public virtual void ClickClose()
        {
            UIManager.Instance.Close(dataStruct.asset.ToString());
        }
        #endregion

        #region UIManager专用接口
        /// <summary>
        /// 只允许UIManager调用，显示面板
        /// </summary>
        public virtual void Display()
        {
            this.gameObject.SetActive(true);
            _viewShowStatus = ViewShowStatus.Show;
            DisplayCallback();
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

        /// <summary>
        /// 只允许UIManager调用，打开面板
        /// </summary>
        public virtual void Open()
        {
            _viewOpenStatus = ViewOpenStatus.Open;
            OpenCallback();
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
        #endregion
        
        #region 回调方法
        /// <summary>
        /// 打开面板之后调用
        /// </summary>
        protected virtual void OpenCallback() { }

        /// <summary>
        /// 关闭面板之后调用
        /// </summary>
        protected virtual void CloseCallback() { }

        /// <summary>
        /// 显示面板之后调用
        /// </summary>
        protected virtual void DisplayCallback() { }

        /// <summary>
        /// 隐藏面板之后调用
        /// </summary>
        protected virtual void HideCallback() { }

        /// <summary>
        /// 设置面板数据后调用
        /// </summary>
        /// <param name="data">设置的数据</param>
        protected virtual void SetDataCallback(object data) { }

        #endregion
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
