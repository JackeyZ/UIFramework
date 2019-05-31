using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

/// <summary>
/// 名称：Json配置管理器
/// 作用：管理项目中的Json配置文件
/// </summary>
public class ConfigManager : MonoSingleton<ConfigManager>
{
    void Awake()
    {

    }

    public void ReadConfig()
    {
        PrefabLoader.LoadPrefab("jsonconfig.u3dassetbundle", "excel1", (obj) => {
            string json = (obj as TextAsset).ToString();
            JObject jsonObj = JObject.Parse(json);

            Debug.Log(jsonObj["ItemList"].ToString());
            Debug.Log(jsonObj["EquipList"].ToString());

            JArray ints = (JArray)jsonObj["ItemList"];

            foreach (var inter in ints)
            {
                Debug.Log(inter);
                Debug.Log(inter["Name"]);
            }
        });
    }
}
