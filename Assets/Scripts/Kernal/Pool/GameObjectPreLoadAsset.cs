using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 名称：预加载配置
/// 作用：记录需要打入对象池的预制体
/// </summary>
public class GameObjectPreLoadAsset : ScriptableObject
{
    public int size = 0;
    public List<GameObjectPreLoadAssetStruct> preLoadAssetList;
}

[Serializable]
public class GameObjectPreLoadAssetStruct
{
    public GameObject prefab;
    public int preLoadNum;          // 需要预打入多少个
}

#if UNITY_EDITOR
[CustomEditor(typeof(GameObjectPreLoadAsset))]
public class GameObjectPreLoadAssetEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GameObjectPreLoadAsset script = target as GameObjectPreLoadAsset;
        EditorGUILayout.LabelField("对象池预加载对象");
        EditorGUILayout.LabelField("数量：" + script.size);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("+"))
        {
            Undo.RegisterCompleteObjectUndo(script, "GameObjectPreLoadAsset");
            script.size += 1;
        }
        if (GUILayout.Button("-"))
        {
            Undo.RegisterCompleteObjectUndo(script, "GameObjectPreLoadAsset");
            if (script.size > 0)
            {
                script.size -= 1;
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("预制体");
        EditorGUILayout.LabelField("数量");
        EditorGUILayout.EndHorizontal();

        if (script.preLoadAssetList == null)
        {
            script.preLoadAssetList = new List<GameObjectPreLoadAssetStruct>();
        }
        int i = 0;
        for (i = 0; i < script.size; i++)
        {
            if (i >= script.preLoadAssetList.Count)
            {
                GameObjectPreLoadAssetStruct preloadAsset = new GameObjectPreLoadAssetStruct();
                preloadAsset.preLoadNum = 1;
                script.preLoadAssetList.Add(preloadAsset);
            }
            EditorGUILayout.BeginHorizontal();
            script.preLoadAssetList[i].prefab = EditorGUILayout.ObjectField(script.preLoadAssetList[i].prefab, typeof(GameObject), true) as GameObject;
            script.preLoadAssetList[i].preLoadNum = EditorGUILayout.IntField(script.preLoadAssetList[i].preLoadNum);
            if (script.preLoadAssetList[i].preLoadNum < 0)
            {
                script.preLoadAssetList[i].preLoadNum = 0;
            }
            if (GUILayout.Button("X"))
            {
                Undo.RegisterCompleteObjectUndo(script, "GameObjectPreLoadAsset");
                script.size -= 1;
                script.preLoadAssetList.RemoveAt(i);
            }
            EditorGUILayout.EndHorizontal();
        }

        if(script.preLoadAssetList.Count > 0)
        {
            script.preLoadAssetList.RemoveRange(i, script.preLoadAssetList.Count - i);
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
#endif
