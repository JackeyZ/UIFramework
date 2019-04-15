using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class NameTable : MonoBehaviour, ISerializationCallbackReceiver
{
    [SerializeField]
    private int _Size;
    [SerializeField]
    private List<string> _keys;
    [SerializeField]
    private List<GameObject> _values;

    [SerializeField]
    public Dictionary<string, GameObject> _DicGameObject;

    public GameObject Find(string name)
    {
        return _DicGameObject[name];
    }
    
    public void OnBeforeSerialize()
    {
         
    }

    // After the serialization we create the dictionary from the two lists
    public void OnAfterDeserialize()
    {
        if (_DicGameObject == null)
        {
            _DicGameObject = new Dictionary<string, GameObject>();
        }
        _DicGameObject.Clear();
        int count = Mathf.Min(_keys.Count, _values.Count);
        for (int i = 0; i < count; ++i)
        {
            if (!_DicGameObject.ContainsKey(_keys[i]))
            {
                _DicGameObject.Add(_keys[i], _values[i]);
            }
        }
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(NameTable))]
    public class NameTableEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            //获取脚本对象
            NameTable script = target as NameTable;
            script._Size = Mathf.Min(EditorGUILayout.IntField("数量：", script._Size), 100);
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
                EditorGUILayout.EndHorizontal();
            }
            script._keys.RemoveRange(i, script._keys.Count - i);


            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        }
    }

#endif
}