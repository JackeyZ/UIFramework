using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
public class CopyTools
{
    static Component component;                                         //用于单个复制
    static List<Component> componentArr = new List<Component>();        //用于多个复制
    static Vector3 position = Vector3.zero;

    //%=Ctrl #=Shift &=Alt  
    [MenuItem("CopyTools/Copy/CopyImage #e", false, 1)]  //快捷键为Shift + e 
    static void CopyImage()
    {
        Debug.Log("Image复制");
        component = Selection.activeGameObject.GetComponent<Image>();
    }
    [MenuItem("CopyTools/Copy/CopyAddImage &e", false, 11)]  //快捷键为Alt + e 
    static void CopyAddImage()
    {
        Debug.Log("Image添加到剪切板");
        componentArr.Add(Selection.activeGameObject.GetComponent<Image>());
    }


    [MenuItem("CopyTools/Copy/CopyRectTransform #c", false, 2)]
    static void CopyRectTransform()
    {
        Debug.Log("RectTransform复制");
        component = Selection.activeGameObject.GetComponent<RectTransform>();
    }

    [MenuItem("CopyTools/Copy/CopyAddRectTransform &r", false, 12)]
    static void CopyAddRectTransform()
    {
        Debug.Log("RectTransform添加到剪切板");
        componentArr.Add(Selection.activeGameObject.GetComponent<RectTransform>());
    }
    [MenuItem("CopyTools/Copy/CopyAddRtanImgTxt &a", false, 13)]
    static void CopyAddRtanImgTxt()
    {
        Debug.Log("Rtran、Img、Txt添加到剪切板");
        componentArr.Add(Selection.activeGameObject.GetComponent<RectTransform>());
        componentArr.Add(Selection.activeGameObject.GetComponent<Image>());
        componentArr.Add(Selection.activeGameObject.GetComponent<Text>());
    }


    [MenuItem("CopyTools/Copy/CopyText #t", false, 3)]
    static void CopyText()
    {
        Debug.Log("Text复制");
        component = Selection.activeGameObject.GetComponent<Text>();
    }
    [MenuItem("CopyTools/Copy/CopyAddText &t", false, 14)]
    static void CopyAddText()
    {
        Debug.Log("Text添加到剪切板");
        componentArr.Add(Selection.activeGameObject.GetComponent<Text>());
    }

    [MenuItem("CopyTools/Copy/CopyAddTextOutlineShadow &g", false, 15)]
    static void CopyAddTextOutlineShadow()
    {
        Debug.Log("Text添加到剪切板");
        componentArr.Add(Selection.activeGameObject.GetComponent<Text>());
        Component[] components = Selection.activeGameObject.GetComponents<Shadow>();
        foreach (var item in components)
        {
            componentArr.Add(item);
        }
    }
    

    [MenuItem("CopyTools/Copy/CopyShadow #s", false, 5)]
    static void CopyShadow()
    {
        Debug.Log("Shadow复制");
        Component[] components = Selection.activeGameObject.GetComponents<Shadow>();
        foreach (var item in components)
        {
            if (item.GetType() == typeof(Shadow))
                component = item;
        }
    }

    [MenuItem("CopyTools/Copy/CopyOutline #o", false, 6)]
    static void CopyOutline()
    {
        Debug.Log("Outline复制");
        component = Selection.activeGameObject.GetComponent<Outline>();
    }
    

    //控制对应按钮是否可用（灰）
    [MenuItem("CopyTools/Copy/CopyImage #e", true, 1)]
    [MenuItem("CopyTools/Copy/CopyRectTransform #c", true, 2)]
    [MenuItem("CopyTools/Copy/CopyText #t", true, 3)]
    //[MenuItem("CopyTools/Copy/CopyGradient #g", true, 4)]
    [MenuItem("CopyTools/Copy/CopyShadow #s", true, 5)]
    [MenuItem("CopyTools/Copy/CopyOutline #o", true, 6)]
    //[MenuItem("CopyTools/Copy/CopyLoadRawImage #r", true, 7)]
    [MenuItem("CopyTools/Copy/PasteComponent #v", true, 8)]
    [MenuItem("CopyTools/Copy/CopyAddImage &e", true, 11)]
    [MenuItem("CopyTools/Copy/CopyAddRectTransform &r", true, 12)]
    [MenuItem("CopyTools/Copy/CopyAddRtanImgTxt &a", true, 13)]
    [MenuItem("CopyTools/Copy/CopyAddText &t", true, 14)]
    [MenuItem("CopyTools/Copy/CopyAddTextOutlineShadow &g", true, 15)]
    [MenuItem("CopyTools/Copy/PasteComponents &v", true, 16)]
    [MenuItem("CopyTools/Copy/CopyWorldPosition #&c", true, 101)]
    [MenuItem("CopyTools/Copy/PasteWorldPosition #&v", true, 102)]
    [MenuItem("CopyTools/Copy/PasteTextExceptTxt #&t", true, 103)]
    static bool CopyValidate()
    {
        if (Selection.activeGameObject == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    [MenuItem("CopyTools/Copy/PasteComponent #v", false, 8)]
    static void PasteComponent()
    {
        if (component == null)
        {
            Debug.Log("组件不存在");
            return;
        }
        //便利当前选中的所有gameobject
        foreach (var item in Selection.gameObjects)
        {
            GameObject targetObject = item;                                             //当前选中的gameobject
            UnityEditorInternal.ComponentUtility.CopyComponent(component);              //把当前获取的组件复制到组件剪切板
            Component oldComponent = targetObject.GetComponent(component.GetType());	//获取选中的gameobject中的和剪切板组件一样的组件
            System.Type curType = component.GetType();

            if (component.GetType() == typeof(Shadow))
            {
                Component[] components = targetObject.GetComponents<Shadow>();
                foreach (var comp in components)
                {
                    if (comp.GetType() == typeof(Shadow))
                        oldComponent = comp;
                }
            }

            //检查目标物体是否已经存在相同的组件，如果有相同的组件则赋值，如果没有则新增组件
            if (oldComponent != null && curType == oldComponent.GetType())
            {
                if (UnityEditorInternal.ComponentUtility.PasteComponentValues(oldComponent))
                {
                    Debug.Log("Paste Values " + component.GetType().ToString() + " Success");
                }
                else {
                    Debug.Log("Paste Values " + component.GetType().ToString() + " Failed");
                }
            }
            else {
                if (UnityEditorInternal.ComponentUtility.PasteComponentAsNew(targetObject))
                {
                    Debug.Log("Paste New Component " + component.GetType().ToString() + " Success");
                }
                else {
                    Debug.Log("Paste New Component " + component.GetType().ToString() + " Failed");
                }
            }
        }
    }

    [MenuItem("CopyTools/Copy/PasteComponents &v", false, 16)]
    static void DoPasteComponent()
    {
        if (componentArr.Count == 0)
        {
            return;
        }
        foreach (var item in Selection.gameObjects)
        {
            GameObject targetObject = item;
            for (int i = 0; i < componentArr.Count; i++)
            {
                Component newComponent = componentArr[i];
                if (newComponent == null)
                {
                    continue;
                }
                UnityEditorInternal.ComponentUtility.CopyComponent(newComponent);
                Component oldComponent = targetObject.GetComponent(newComponent.GetType());

                System.Type curType = newComponent.GetType();

                if (newComponent.GetType() == typeof(Shadow))
                {
                    Component[] components = targetObject.GetComponents<Shadow>();
                    foreach (var comp in components)
                    {
                        if (comp.GetType() == typeof(Shadow))
                            oldComponent = comp;
                    }
                }

                if (oldComponent != null && curType == oldComponent.GetType())
                {
                    if (UnityEditorInternal.ComponentUtility.PasteComponentValues(oldComponent))
                    {
                        Debug.Log("Paste Values " + newComponent.GetType().ToString() + " Success");
                    }
                    else {
                        Debug.Log("Paste Values " + newComponent.GetType().ToString() + " Failed");
                    }
                }
                else {
                    if (UnityEditorInternal.ComponentUtility.PasteComponentAsNew(targetObject))
                    {
                        Debug.Log("Paste New Values " + newComponent.GetType().ToString() + " Success");
                    }
                    else {
                        Debug.Log("Paste New Values " + newComponent.GetType().ToString() + " Failed");
                    }
                }
            }
        }
        componentArr.Clear();
    }

    [MenuItem("CopyTools/Copy/ClearComponentArr &c", false, 15)]
    static void ClearComponentArr()
    {
        Debug.Log("ComponentArr清空");
        componentArr.Clear();
    }

    //复制世界坐标
    [MenuItem("CopyTools/Copy/CopyWorldPosition #&c", false, 101)]
    static void CopyPosition()
    {
        Debug.Log("Position复制");
        var rt = Selection.activeGameObject.GetComponent<RectTransform>();
        if (rt != null)
        {
            position = rt.position;
        }
    }

    [MenuItem("CopyTools/Copy/PasteWorldPosition #&v", false, 102)]
    static void PastePosition()
    {
        Debug.Log("Position粘贴");
        foreach (var item in Selection.gameObjects)
        {
            GameObject targetObject = item;                                             //当前选中的gameobject
            RectTransform trans = targetObject.GetComponent<RectTransform>();
            trans.position = position;
        }
    }

    //粘贴text组件，但是不替换文字内容
    [MenuItem("CopyTools/Copy/PasteTextExceptTxt #&t", false, 103)]
    static void PasteTextExceptTxt()
    {
        Debug.Log("Text粘贴，不替换文字");
        if (component != null && component.GetType() == typeof(Text))
        {
            foreach (var item in Selection.gameObjects)
            {
                GameObject targetObject = item;                                             //当前选中的gameobject
                UnityEditorInternal.ComponentUtility.CopyComponent(component);              //把当前获取的组件复制到组件剪切板
                var target_text = targetObject.GetComponent<Text>();
                string str = target_text.text;
                //检查目标物体是否已经存在相同的组件，如果有相同的组件则赋值，如果没有则新增组件
                if (target_text != null)
                {
                    if (UnityEditorInternal.ComponentUtility.PasteComponentValues(target_text))
                    {
                        targetObject.GetComponent<Text>().text = str;
                        Debug.Log("Paste Values " + component.GetType().ToString() + " Success");
                    }
                    else {
                        Debug.Log("Paste Values " + component.GetType().ToString() + " Failed");
                    }
                }
                else {
                    if (UnityEditorInternal.ComponentUtility.PasteComponentAsNew(targetObject))
                    {
                        Debug.Log("Paste New Component " + component.GetType().ToString() + " Success");
                    }
                    else {
                        Debug.Log("Paste New Component " + component.GetType().ToString() + " Failed");
                    }
                }
            }
        }
    }

    //去掉没有图片资源的Image组件和CanvasRenderer
    //[MenuItem("CopyTools/removeCanvasRenderer &a", false, 15)]
    static void removeCanvasRenderer()
    {
        //Component[] componentList = Selection.activeGameObject.GetComponentsInChildren<CanvasRenderer> (true);
        //foreach (var componentItem in componentList) {
        //	GameObject.DestroyImmediate (componentItem,true);
        //}

        //Image[] imageList = Selection.activeGameObject.GetComponentsInChildren<Image> (true);
        //foreach (var imageItem in imageList) {
        //	if (imageItem.sprite == null && imageItem.IsActive() == true) {
        //		GameObject.DestroyImmediate (imageItem, true);
        //	}
        //}
        Component[] componentList = Selection.activeGameObject.GetComponentsInChildren<CanvasRenderer>(true);
        foreach (var componentItem in componentList)
        {
            try
            {
                GameObject.DestroyImmediate(componentItem, true);
            }

            catch (System.Exception e)
            {
                Debug.Log(e);
            }

        }
        /*GameObjectAttach[] componentList2 = Selection.activeGameObject.GetComponentsInChildren<GameObjectAttach> (true);
		foreach (var componentItem in componentList2) {
			Debug.Log (componentItem.isActiveAndEnabled);
			//if(componentItem.Asset())
			//GameObject.DestroyImmediate (componentItem, true);
		}*/
    }

    //[MenuItem("CopyTools/Set/SetTransform &v", false, 1)]
    static void SetTransform()
    {
        foreach (var item in Selection.gameObjects)
        {
            GameObject obj = FindHideChildGameObject(item, "weapon_point1");
            if (!obj)
            {
                return;
            }
            Transform objTransform = obj.GetComponent<Transform>();
            if (objTransform != null)
            {
                objTransform.localPosition = new Vector3(0.0475692f, -0.3431357f, 0.00349453f);
                objTransform.localEulerAngles = new Vector3(-84.65501f, 113.49f, 1.869f);
            }
        }
        foreach (var item in Selection.gameObjects)
        {
            GameObject obj = FindHideChildGameObject(item, "weapon_point2");
            if (!obj)
            {
                return;
            }
            Transform objTransform = obj.GetComponent<Transform>();
            if (objTransform != null)
            {
                objTransform.localPosition = new Vector3(-0.3270334f, 1.977906f, -0.3626547f);
                objTransform.localEulerAngles = new Vector3(-6.315001f, -18.789f, 28.048f);
            }
        }


    }

    public static GameObject FindHideChildGameObject(GameObject parent, string childName)
    {
        if (parent.name == childName)
        {
            return parent;
        }
        if (parent.transform.childCount < 1)
        {
            return null;
        }
        GameObject obj = null;
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            GameObject go = parent.transform.GetChild(i).gameObject;
            obj = FindHideChildGameObject(go, childName);
            if (obj != null)
            {
                break;
            }
        }
        return obj;
    }
    //粘贴text组件，但是不替换文字内容
    [MenuItem("CopyTools/Change/ChangeText", false, 1)]
    static void ChangeText()
    {
        GameObject[] gameObjects = Selection.gameObjects;
        if (gameObjects == null || gameObjects.Length == 0)
        {
            Debug.LogError("No Select!!");
            return;
        }
        foreach (var go in gameObjects)
        {
            Check(go);
        }
        AssetDatabase.SaveAssets();
    }

    static void Check(GameObject obj)
    {
        var texts = obj.GetComponentsInChildren<Text>(true);
        foreach (var textItem in texts)
        {
            Outline outline = textItem.gameObject.GetComponent<Outline>();
            Shadow[] shadows = textItem.gameObject.GetComponents<Shadow>();
            if (outline != null && shadows.Length == 1)
            {
                Debug.LogError("替换名称：" + textItem.gameObject.name);
                Shadow shadow = textItem.gameObject.AddComponent<Shadow>();
                shadow.effectColor = outline.effectColor;
                shadow.effectDistance = outline.effectDistance;
                GameObject.DestroyImmediate(outline, true);
            }
        }
    }
}