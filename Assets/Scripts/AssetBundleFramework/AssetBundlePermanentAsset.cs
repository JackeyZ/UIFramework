using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

/// <summary>
/// 名称：常驻内存AB包资源配置
/// 作用：
/// </summary>
public class AssetBundlePermanentAsset : ScriptableObject, ISerializationCallbackReceiver
{
    // 外部调用(用HashSet是为了调用Contains时候的效率)
    public HashSet<string> BundleNameHS = new HashSet<string>();

    // 内部调用
    public List<BundleElement> BundleNameList = new List<BundleElement>();

    public const string ERROR_TIPS = "文件夹选择有误！";
    public void OnAfterDeserialize()
    {
        BundleNameHS.Clear();
        foreach (var item in BundleNameList)
        {
            if (item.bundleName != AssetBundlePermanentAsset.ERROR_TIPS && !string.IsNullOrEmpty(item.bundleName))
                BundleNameHS.Add(item.bundleName);
        }
    }

    public void OnBeforeSerialize()
    {
    }
}

[Serializable]
public class BundleElement
{
    public UnityEngine.Object bundleFolder;             // AB包所在文件夹
    public UnityEngine.Object lastBundleFolder;         // 上一次赋值的AB包所在文件，防止Editor下重复获取AB包名造成卡顿
    public string bundleName;                           // AB包名

    public BundleElement(string bundleName, UnityEngine.Object obj = null)
    {
        bundleFolder = obj;
        this.bundleName = bundleName;
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(AssetBundlePermanentAsset))]
public class AssetBundlePermanentAssetEditor : Editor
{
    private ReorderableList reorderableList;
    Color? elementDefaultColor = null;  // 用于储存默认GUI颜色, 问号表示允许该变量为null

    private void OnEnable()
    {
        ClearLastFolderObj();
        // 创建可排序列表
        reorderableList = new ReorderableList(serializedObject,
            serializedObject.FindProperty("BundleNameList"),
            true, true, true, true);
        if (elementDefaultColor == null)
        {
            elementDefaultColor = GUI.color;
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        AssetBundlePermanentAsset script = target as AssetBundlePermanentAsset;

        // GameObject列表
        SerializedProperty prop = serializedObject.FindProperty("BundleNameList");
        // 绘制子物体回调
        reorderableList.drawElementCallback = (rect, index, isActive, isFocused) =>
        {
            SetBundleName(index);
            EditorGUI.PropertyField(rect, prop.GetArrayElementAtIndex(index));
        };

        // 绘制列表标题
        reorderableList.drawHeaderCallback = (Rect rect) =>
        {
            GUI.Label(rect, "常驻内存的AB包列表");
        };

        // 加号回调
        reorderableList.onAddCallback = (ReorderableList list) =>
        {
            script.BundleNameList.Add(new BundleElement(""));
            list.index = script.BundleNameList.Count - 1;         // 选中最后一个
            };

        // 减号回调
        reorderableList.onRemoveCallback = (ReorderableList list) =>
        {
            if (EditorUtility.DisplayDialog("警告", "是否真的要删除" + script.BundleNameList[list.index].bundleName + "？", "是", "否"))
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
                string value = script.BundleNameList[index].bundleName;
            for (int i = 0; i < script.BundleNameList.Count; i++)
            {
                if ((i != index && value.Equals(script.BundleNameList[i].bundleName)) || (AssetBundlePermanentAsset.ERROR_TIPS == value))
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

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
    
    private void SetBundleName(int index)
    {
        AssetBundlePermanentAsset script = target as AssetBundlePermanentAsset;
        UnityEngine.Object folderObj = script.BundleNameList[index].bundleFolder;
        UnityEngine.Object lastFolderObj = script.BundleNameList[index].lastBundleFolder;
        if (folderObj != null && folderObj != lastFolderObj)
        {
            script.BundleNameList[index].bundleName = AssetBundlePermanentAsset.ERROR_TIPS;
            script.BundleNameList[index].lastBundleFolder = folderObj;
            string path = AssetDatabase.GetAssetPath(folderObj);                            // 获取资源路径
            // 判断是不是文件夹
            if (Directory.Exists(path))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(path);
                // 遍历子物体，直到找到一个有ab名的资源
                foreach (var childSysInfo in directoryInfo.GetFileSystemInfos())
                {
                    if (childSysInfo != null && childSysInfo.Extension != ".meta" && childSysInfo is FileInfo)
                    {
                        string unityPath = childSysInfo.FullName.Replace('\\', '/');          //反斜杠替换成斜杠
                        AssetImporter assetImporter = AssetImporter.GetAtPath(unityPath.Substring(unityPath.IndexOf("Assets")));
                        if (assetImporter != null && !string.IsNullOrEmpty(assetImporter.assetBundleName))
                        {
                            script.BundleNameList[index].bundleName = assetImporter.assetBundleName + "." + assetImporter.assetBundleVariant;
                            break;
                        }
                    }
                }//foreach_end
            }
        }
    }// fucntion_end

    // OnEnable的时候清空上一次obj，以便与刷新bundleName
    private void ClearLastFolderObj()
    {
        AssetBundlePermanentAsset script = target as AssetBundlePermanentAsset;
        for (int i = 0; i < script.BundleNameList.Count; i++)
        {
            script.BundleNameList[i].lastBundleFolder = null;
        }
    }
}


[CustomPropertyDrawer(typeof(BundleElement))]
public class BundleElementDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //创建一个属性包装器，用于将常规GUI控件与SerializedProperty一起使用
        using (new EditorGUI.PropertyScope(position, label, property))
        {
            //输入框高度，默认一行的高度
            position.height = EditorGUIUtility.singleLineHeight;


            Rect bundleNameRect = new Rect(position)
            {
                width = position.width / 2.1f,
            };

            Rect folderRect = new Rect(position)
            {
                x = position.width / 2 + 30,
                width = position.width / 2.1f,
            };

            //找到每个属性的序列化值
            SerializedProperty folderProperty = property.FindPropertyRelative("bundleFolder");
            SerializedProperty bundleNameProperty = property.FindPropertyRelative("bundleName");

            EditorGUI.LabelField(bundleNameRect, bundleNameProperty.stringValue);
            EditorGUI.PropertyField(folderRect, folderProperty, GUIContent.none);

        }
    }
}
#endif
