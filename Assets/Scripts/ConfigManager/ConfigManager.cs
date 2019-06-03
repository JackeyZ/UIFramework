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

    public JObject LoadConfig(string configName)
    {
        UnityEngine.Object obj = PrefabLoader.LoadPrefabSync("jsonconfig.u3dassetbundle", configName, true);
        string json = (obj as TextAsset).ToString();
        JObject jsonObj = JObject.Parse(json);

        #region 遍历示例
        //Debug.Log(jsonObj["ItemList"].ToString());
        //foreach (var item in jsonObj)
        //{
        //    Debug.Log(item.Key);
        //    Debug.Log(item.Value);
        //    Debug.Log(item.Value[0]);
        //    Debug.Log(item.Value[1]);
        //    JArray array = item.Value as JArray; // 是数组（[])才可以转换成JArray， 不是数组转换会返回Null
        //    foreach (var rowItem in array)
        //    {
        //        Debug.Log(rowItem);
        //        Debug.Log("Name" + ":" + rowItem["Name"]);
        //        Debug.Log("Id" + ":" + rowItem["Id"]);
        //        foreach (var colItem in rowItem)
        //        {
        //            Debug.Log(colItem.ToString());
        //        }
        //    }
        //}
        #endregion

        return jsonObj;
    }

    // 加载excel表中的一个分页
    public JToken LoadConfigSheet(string configName, string sheetName)
    {
        JObject config = LoadConfig(configName);
        if(config[sheetName] != null)
        {
            return config[sheetName];
        }
        else
        {
            Debug.LogError(configName + "配置中的" + sheetName + "分页不存在，请检查");
            return null;
        }
    }
}
