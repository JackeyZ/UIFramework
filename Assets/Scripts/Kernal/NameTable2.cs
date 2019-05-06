using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditorInternal;
using UnityEditor;
#endif

/// <summary>
/// GameObject列表，用于挂载到预制体上，通过命名找到子节点。（旧的，仍然可以使用）
/// </summary>
public class NameTable2 : MonoBehaviour, ISerializationCallbackReceiver
{
    [SerializeField]
    private int _Size = 0;
    [SerializeField]
    private List<string> _keys;
    [SerializeField]
    private List<GameObject> _values;

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
        int count = Mathf.Min(_keys.Count, _values.Count);
        for (int i = 0; i < count; ++i)
        {
            if (!DicGameObject.ContainsKey(_keys[i]))
            {
                DicGameObject.Add(_keys[i], _values[i]);
            }
        }
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(NameTable2))]
    public class NameTable2Editor : Editor
    {
        public override void OnInspectorGUI()
        {
            //获取脚本对象
            NameTable2 script = target as NameTable2;
            EditorGUILayout.LabelField("数量："+ script._Size);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+"))
            {
                Undo.RegisterCompleteObjectUndo(script, "NameTable2");
                script._Size += 1;
            }
            if (GUILayout.Button("-"))
            {
                Undo.RegisterCompleteObjectUndo(script, "NameTable2");
                script._Size -= 1;
                if (script._Size < 0)
                {
                    script._Size = 0;
                }
            }
            if (GUILayout.Button("排序"))
            {
                Undo.RegisterCompleteObjectUndo(script, "NameTable2");
                List<string> keyList = script._keys;
                List<GameObject> valueList = script._values;
                // 冒泡
                for (int key1 = 0; key1 < keyList.Count; key1++)
                {
                    for (int key = 0; key < keyList.Count - key1 - 1; key++)
                    {
                        if(keyList[key].CompareTo(keyList[key + 1]) > 0)
                        {
                            Swap(keyList, key, key + 1);
                            Swap(valueList, key, key + 1);
                        }
                    }
                }

            }
            EditorGUILayout.EndHorizontal();
            if (script._keys == null)
            {
                script._keys = new List<string>();
                script._values = new List<GameObject>();
            }
            int i = 0;
            for (i = 0; i < script._Size; i++)
            {
                if (i >= script._keys.Count)
                {
                    script._keys.Add("");
                    script._values.Add(null);
                }
                EditorGUILayout.BeginHorizontal();
                script._keys[i] = EditorGUILayout.TextField(script._keys[i]);
                script._values[i] = EditorGUILayout.ObjectField(script._values[i], typeof(GameObject), true) as GameObject;
                if (GUILayout.Button("X"))
                {
                    Undo.RegisterCompleteObjectUndo(script, "NameTable2");
                    script._Size -= 1;
                    script._keys.RemoveAt(i);
                }
                EditorGUILayout.EndHorizontal();
            }
            script._keys.RemoveRange(i, script._keys.Count - i);
            script._values.RemoveRange(i, script._values.Count - i);

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
#endif
}