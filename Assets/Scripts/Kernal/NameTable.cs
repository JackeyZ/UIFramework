using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditorInternal;
using UnityEditor;
#endif

/// <summary>
/// GameObject列表，用于挂载到预制体上，通过命名找到子节点。
/// </summary>
public class NameTable : MonoBehaviour, ISerializationCallbackReceiver
{
    [SerializeField]
    private List<NameTableElement> nameTableElementList = new List<NameTableElement>();

    [SerializeField]
    private Dictionary<string, GameObject> dicGameObject;

    public Dictionary<string, GameObject> DicGameObject
    {
        get
        {
            return dicGameObject;
        }

        set
        {
            dicGameObject = value;
        }
    }

    public GameObject Find(string name)
    {
        return DicGameObject[name];
    }

    public void OnBeforeSerialize()
    {

    }

    // After the serialization we create the dictionary from the two lists
    public void OnAfterDeserialize()
    {
        if (DicGameObject == null)
        {
            DicGameObject = new Dictionary<string, GameObject>();
        }
        DicGameObject.Clear();
        for (int i = 0; i < nameTableElementList.Count; ++i)
        {
            if (DicGameObject.ContainsKey(nameTableElementList[i].key))
                continue;
            DicGameObject.Add(nameTableElementList[i].key, nameTableElementList[i].value);
        }
    }

    [Serializable]
    public class NameTableElement
    {
        public string key;
        public GameObject value;
        public NameTableElement(string key, GameObject go = null)
        {
            this.key = key;
            this.value = go;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(NameTable))]
    public class NameTableEditor : Editor
    {
        ReorderableList reorderableList;
        Color? elementDefaultColor = null;  // 用于储存默认GUI颜色, 问号表示允许该变量为null
        private void OnEnable()
        {
            // 创建可排序列表
            reorderableList = new ReorderableList(serializedObject,
                serializedObject.FindProperty("nameTableElementList"),
                true, true, true, true);
            if (elementDefaultColor == null)
            {
                elementDefaultColor = GUI.color;
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            NameTable script = target as NameTable;

            // GameObject列表
            SerializedProperty prop = serializedObject.FindProperty("nameTableElementList");
            // 绘制子物体回调
            reorderableList.drawElementCallback = (rect, index, isActive, isFocused) => {
                EditorGUI.PropertyField(rect, prop.GetArrayElementAtIndex(index));
            };

            // 绘制列表标题
            reorderableList.drawHeaderCallback = (Rect rect) =>
            {
                GUI.Label(rect, "GameObjectKeys");
            };

            // 加号回调
            reorderableList.onAddCallback = (ReorderableList list) =>
            {
                script.nameTableElementList.Add(new NameTableElement(""));
            };

            // 减号回调
            reorderableList.onRemoveCallback = (ReorderableList list) =>
            {
                if (EditorUtility.DisplayDialog("警告", "是否真的要删除" + script.nameTableElementList[list.index].key + "？", "是", "否"))
                {
                    ReorderableList.defaultBehaviours.DoRemoveButton(list);
                }
            };

            // 绘制列表背景
            reorderableList.drawElementBackgroundCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                if (index == -1)
                {
                    return;
                }
                bool repeat = false;                                                        // 属性key值是否重复
                string key = script.nameTableElementList[index].key;
                for (int i = 0; i < script.nameTableElementList.Count ; i++)
                {
                    if (i != index && key.Equals(script.nameTableElementList[i].key))
                    {
                        repeat = true;
                    }
                }

                if (repeat)
                {
                    GUI.color = new Color(1f, 0f, 0f, 0.8f);                                // 修改GUI颜色
                }
                else
                {
                    GUI.color = (Color)elementDefaultColor;                                 // 还原GUI颜色
                }
                ReorderableList.defaultBehaviours.DrawElementBackground(rect, index, isActive, isFocused, true);
            };

            reorderableList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
            
            // 绘制排序按钮
            if (GUILayout.Button("排序"))
            {
                Undo.RegisterCompleteObjectUndo(script, "NameTable");
                // 冒泡
                for (int i = 0; i < script.nameTableElementList.Count; i++)
                {
                    for (int j = 0; j < script.nameTableElementList.Count - i - 1; j++)
                    {
                        if (script.nameTableElementList[j].key.CompareTo(script.nameTableElementList[j + 1].key) > 0)
                        {
                            Swap(script.nameTableElementList, j, j + 1);
                        }
                    }
                }
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        }
        void Swap<T>(List<T> list, int index1, int index2)
        {
            T goTemp = list[index1];
            list[index1] = list[index2];
            list[index2] = goTemp;
        }
    }

    [CustomPropertyDrawer(typeof(NameTableElement))]
    public class NameTableElementDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //创建一个属性包装器，用于将常规GUI控件与SerializedProperty一起使用
            using (new EditorGUI.PropertyScope(position, label, property))
            {
                //输入框高度，默认一行的高度
                position.height = EditorGUIUtility.singleLineHeight;


                Rect keyRect = new Rect(position)
                {
                    width = position.width / 2.1f,
                };

                Rect valueRect = new Rect(position)
                {
                    x = position.width / 2 + 30,
                    width = position.width / 2.1f,
                };

                //找到每个属性的序列化值
                SerializedProperty keyProperty = property.FindPropertyRelative("key");
                SerializedProperty valueProperty = property.FindPropertyRelative("value");
                
                keyProperty.stringValue = EditorGUI.TextField(keyRect, keyProperty.stringValue);
                EditorGUI.PropertyField(valueRect, valueProperty, GUIContent.none);
            }
        }
    }
#endif
}