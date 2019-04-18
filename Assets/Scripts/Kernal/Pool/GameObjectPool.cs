/*
*Title:"地下守护神"项目开发
*
*Description:
*	对象池技术，减少实例化对象和销毁对象的次数
*
*Date:2017
*
*Version:0.1
*
*Modify Recoder:
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//对象池
//节省性能的方法，important，
namespace Kernal
{
    public class GameObjectPool : MonoSingleton<GameObjectPool>
    {
        //创建一个字典_Dictionary 键为实例化的对象名（字符串），值为数组（存放多个同样的处于未激活状态的对象）
        //private static Dictionary<string, ArrayList> _Dictionary = new Dictionary<string, ArrayList>();
        private static Dictionary<string, Pool> _Dictionary = new Dictionary<string, Pool>();
        // Use this for initialization
        void Awake()
        {
            _Dictionary.Clear();
        }
        void Start()
        {
            StartCoroutine("CheckRedundantGameObjectList"); //开启该协程，用于把长时间不用的对象删除
            StartCoroutine("PreLoadGameObject");            // 预加载
        }
        public void ClearPool()
        {
            _Dictionary.Clear();
        }

        /// <summary>
        /// 用于增加缓冲池中对应路径的对象数量
        /// </summary>
        /// <param name="path">prefab路径</param>
        /// <param name="number">需要增加的数目</param>
        public static void AddGameObject(string abName, string assetName, int number = 1)
        {
            PrefabLoader.LoadPrefab(abName, assetName, (prefab) =>
            {
                for (int i = 0; i < number; i++)
                {
                    //根据路径读取并且实例化对象
                    Quaternion qu = new Quaternion();
                    qu.eulerAngles = Vector3.zero;
                    //随机摆放实例化的对象池对象，因为怕对象之间的物理力相互影响
                    GameObject go = GameObject.Instantiate(prefab, new Vector3(Random.Range(1, 100), Random.Range(1, 100), Random.Range(1, 100)), qu) as GameObject;
                    //把实例化的对象取消激活，打入缓冲池
                    Instance.DestroyGO(go);
                }
            });
        }

        public static void AddGameObject(AssetBundleFramework.ABAsset asset, int number = 1)
        {
            AddGameObject(asset.ABPath, asset.AssetName, number);
        }

        /// <summary>
        /// 用于增加缓冲池中对应Prefab的对象数量
        /// </summary>
        /// <param name="path">prefab引用</param>
        /// <param name="number">需要增加的数目</param>
        public static void AddGameObject(GameObject prefab, int number = 1)
        {
            for (int i = 0; i < number; i++)
            {
                //实例化prefab
                //GameObject go = GameObject.Instantiate(prefab,Instance.transform,false);
                Quaternion qu = new Quaternion();
                qu.eulerAngles = Vector3.zero;
                //随机摆放实例化的对象池对象，因为怕对象之间的物理力相互影响
                GameObject go = GameObject.Instantiate(prefab, new Vector3(Random.Range(1, 100), Random.Range(1, 100), Random.Range(1, 100)), qu) as GameObject;
                //把实例化的对象取消激活，打入缓冲池
                Instance.DestroyGO(go);
            }
        }
        /// <summary>
        /// 获取对应prefab在对象池中的数量
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        public static int GetGameObjectNumber(GameObject prefab)
        {
            string GameObjectName = prefab.name + "(Clone)";
            if (_Dictionary.ContainsKey(GameObjectName))
            {
                return _Dictionary[GameObjectName].gameObjectList.Count;
            }else
            {
                return 0;
            }
        }
        /// <summary>
        /// 调用后会返回一个已经实例化的对应键值的GameObject
        /// </summary>
        /// <param name="key">键：对象名</param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public GameObject InstantiateGO(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            string key = prefab.name;
            //创建返回值对象
            GameObject go;
            //拼接制作dic的key名，因为instantiate出的gameobject都会自动命名为gameobject(Clone),这里是为了通下面return方法里给key的命名匹配
            string GameObjectName = key + "(Clone)";
            //如果字典里有GameObjectName这个key 并且key对应的数组不为空（有该种类子弹，且该种类子弹中有《已经创建过的》（未激活）的子弹gameobject）
            if (_Dictionary.ContainsKey(GameObjectName) && _Dictionary[GameObjectName].gameObjectList.Count > 0)
            {
                //从gameobjectname这个key位置取出数组
                ArrayList list = _Dictionary[GameObjectName].gameObjectList;
                //取出一号位的对象
                go = (GameObject)list[0];
                //从列表中去除这个子弹（因为已经拿出来用了）
                list.RemoveAt(0);
                //参数坐标赋于GameObject      
                go.transform.position = position;
                //参数旋转值赋于GameObject
                go.transform.rotation = rotation;
                //将GameObject激活
                go.SetActive(true);
            }
            //如果字典里没有该键（种类）的对象
            else
            {
                //在给定位置创建一个resources中名为给定key的预设体的gameobject
                go = GameObject.Instantiate(prefab, position, rotation) as GameObject;
            }
            //返回创建的东西
            return go;

        }
        public static GameObject InstantiateGO(GameObject prefab, Vector3 position)
        {
            string key = prefab.name;
            //创建返回值对象
            GameObject go;
            //拼接制作dic的key名，因为instantiate出的gameobject都会自动命名为gameobject(Clone),这里是为了通下面return方法里给key的命名匹配
            string GameObjectName = key + "(Clone)";
            //如果字典里有GameObjectName这个key 并且key对应的数组不为空（有该种类子弹，且该种类子弹中有《已经创建过的》（未激活）的子弹gameobject）
            if (_Dictionary.ContainsKey(GameObjectName) && _Dictionary[GameObjectName].gameObjectList.Count > 0)
            {
                //从gameobjectname这个key位置取出数组
                ArrayList list = _Dictionary[GameObjectName].gameObjectList;
                //取出一号位的对象
                go = (GameObject)list[0];
                //从列表中去除这个子弹（因为已经拿出来用了）
                list.RemoveAt(0);
                //参数坐标赋于GameObject      
                go.transform.position = position;
                //将GameObject激活
                go.SetActive(true);
            }
            //如果字典里没有该键（种类）的对象
            else
            {
                Quaternion rotation = Quaternion.Euler(0,0,0);
                //在给定位置创建一个resources中名为给定key的预设体的gameobject
                go = GameObject.Instantiate(prefab, position, rotation) as GameObject;
            }
            //返回创建的东西
            return go;

        }
        public static GameObject InstantiateGO(GameObject prefab, Transform transform)
        {
            string key = prefab.name;
            //创建返回值对象
            GameObject go;
            //拼接制作dic的key名，因为instantiate出的gameobject都会自动命名为gameobject(Clone),这里是为了通下面return方法里给key的命名匹配
            string GameObjectName = key + "(Clone)";
            //如果字典里有GameObjectName这个key 并且key对应的数组不为空（有该种类子弹，且该种类子弹中有《已经创建过的》（未激活）的子弹gameobject）
            if (_Dictionary.ContainsKey(GameObjectName) && _Dictionary[GameObjectName].gameObjectList.Count > 0)
            {
                //从gameobjectname这个key位置取出数组
                ArrayList list = _Dictionary[GameObjectName].gameObjectList;
                //取出一号位的对象
                go = (GameObject)list[0];
                //从列表中去除这个子弹（因为已经拿出来用了）
                list.RemoveAt(0);
                //参数坐标赋于GameObject      
                go.transform.position = transform.position;
                //参数旋转值赋于GameObject
                go.transform.rotation = transform.rotation;
                //将GameObject激活
                go.SetActive(true);
            }
            //如果字典里没有该键（种类）的对象
            else
            {
                //在给定位置创建一个resources中名为给定key的预设体的gameobject
                go = GameObject.Instantiate(prefab,transform) as GameObject;
            }
            //返回创建的东西
            return go;

        }
        public GameObject InstantiateGO(GameObject prefab)
        {
            string key = prefab.name;
            //创建返回值对象
            GameObject go;
            //拼接制作dic的key名，因为instantiate出的gameobject都会自动命名为gameobject(Clone),这里是为了通下面return方法里给key的命名匹配
            string GameObjectName = key + "(Clone)";
            //如果字典里有GameObjectName这个key 并且key对应的数组不为空（有该种类子弹，且该种类子弹中有《已经创建过的》（未激活）的子弹gameobject）
            if (_Dictionary.ContainsKey(GameObjectName) && _Dictionary[GameObjectName].gameObjectList.Count > 0)
            {
                //从gameobjectname这个key位置取出数组
                ArrayList list = _Dictionary[GameObjectName].gameObjectList;
                //取出一号位的对象
                go = (GameObject)list[0];
                //从列表中去除这个子弹（因为已经拿出来用了）
                list.RemoveAt(0);
                //将GameObject激活
                go.SetActive(true);
            }
            //如果字典里没有该键（种类）的对象
            else
            {
                //在给定位置创建一个resources中名为给定key的预设体的gameobject
                go = GameObject.Instantiate(prefab) as GameObject;
            }
            //返回创建的东西
            return go;
        }
        /// <summary>
        /// 将需要取消激活的对象取消激活
        /// </summary>
        /// <param name="go">参数是需要取消激活的对象</param>
        /// <param name="time">在time秒后取消激活</param>
        /// <param name = "isNeedDestroyTime">表明是否需要定时清理</param>
        /// <param name="remainingNumber">表明剩余多少数量不再定时清理</param>
        public void DestroyGO(GameObject go, float time, bool isNeedDestroyTime = false,int remainingNumber = 0)
        {
            StartCoroutine(DelayDestroyGameObject(go, time, isNeedDestroyTime, remainingNumber));
        }
        public void DestroyGO(GameObject go, bool isNeedDestroyTime = false,int remainingNumber = 0)
        {
            StartCoroutine(DelayDestroyGameObject(go, 0, isNeedDestroyTime, remainingNumber));
            go = null;
        }

        IEnumerator DelayDestroyGameObject(GameObject go,float time, bool isNeedDestroyTime = false, int remainingNumber = 0)
        {
            yield return new WaitForSeconds(time);
            string key = go.name;
            //如果字典里有这个key
            if (_Dictionary.ContainsKey(key))
            {   //就在这个key所对应的数组中加入这个go  （这个g就是已经用完的对象，放到这个数组里的gameobjet都是不销毁只是取消激活等待再次利用的gameobject）
                
                _Dictionary[key].gameObjectList.Add(go);
                if(isNeedDestroyTime == true)
                {
                    _Dictionary[key].isNeedDestroyTime = true;
                }
            }
            //如果没有这个key
            else
            {
                //建立一个这个key的arraylist 并把g加进去
                _Dictionary[key] = new Pool(new ArrayList() { go },30, isNeedDestroyTime, remainingNumber);
                //_Dictionary[key].delectTime = 10;
            }
            //不销毁而是取消激活
            go.SetActive(false);
            go.transform.SetParent(gameObject.transform);
            go = null;
        }

        /// <summary>
        /// 检查多余的预设（太久没有使用的空闲预设）
        /// </summary>
        /// <returns></returns>
        IEnumerator CheckRedundantGameObjectList()
        {
            while (enabled)
            {
                yield return new WaitForSeconds(10f);
                if (_Dictionary.Count > 0)
                {
                    foreach (KeyValuePair<string, Pool> _DictionaryItem in _Dictionary)
                    {
                        if (_DictionaryItem.Value.isNeedDestroyTime == true)//需要定时清理
                        {
                            if (_DictionaryItem.Value.gameObjectList.Count <= _DictionaryItem.Value.remainingNumber)//如果对象池中只剩下3个对象
                            {
                                _DictionaryItem.Value.delectTime = 30;
                            }
                            else
                            {
                                _DictionaryItem.Value.delectTime -= 10;
                                if (_DictionaryItem.Value.delectTime <= 0.1f)
                                {
                                    _DictionaryItem.Value.delectTime = 10;
                                    GameObject.Destroy(_DictionaryItem.Value.gameObjectList[_DictionaryItem.Value.gameObjectList.Count - 1] as GameObject);
                                    _DictionaryItem.Value.gameObjectList.RemoveAt(_DictionaryItem.Value.gameObjectList.Count - 1);
                                }
                            }
                        }
                    }//foreach_end
                }
            }//while_end
        }//function_end

        /// <summary>
        /// 预先把GameObjectPreLoadAsset配置上的预制体打入对象池
        /// </summary>
        /// <returns></returns>
        IEnumerator PreLoadGameObject()
        {
            string abName = "assetconfigs.u3dassetbundle";
            string assetName = "GameObjectPreLoadAsset";

#if UNITY_EDITOR
            UnityEngine.Object prefab = AssetBundleFramework.AssetLoadInEditor.LoadObject<UnityEngine.Object>(abName, assetName);
#else
            yield return AssetBundleFramework.AssetBundleMgr.Instance.LoadBundleAsset(abName, assetName, false);
            UnityEngine.Object prefab = AssetBundleFramework.AssetBundleMgr.Instance.GetYieldBundleAsset(abName, assetName);
#endif

            if (prefab != null){
                GameObjectPreLoadAsset goPreLoadAsset = prefab as GameObjectPreLoadAsset;
                foreach (var item in goPreLoadAsset.preLoadAssetList)
                {
                    yield return new WaitForSeconds(0.1f);
                    AddGameObject(item.prefab, item.preLoadNum);
                }
            }
        }

        class Pool
        {
            public ArrayList gameObjectList;    //储存对象的列表
            public float delectTime;            //用于记录定期清理多余对象的时间
            public bool isNeedDestroyTime;      //表明是否需要定期清理多余对象
            public int remainingNumber;         //剩余多少数量不再定时清理
            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="gameObjectList">储存对象的列表</param>
            /// <param name="delectTime">用于记录定期清理多余对象的时间</param>
            /// <param name="isNeedDestroyTime">表明是否需要定期清理多余对象</param>
            /// <param name="remainingNumber">剩余多少数量不再定时清理</param>
            public Pool(ArrayList gameObjectList, float delectTime, bool isNeedDestroyTime = false, int remainingNumber = 0)
            {
                this.gameObjectList = gameObjectList;
                this.delectTime = delectTime;
                this.isNeedDestroyTime = isNeedDestroyTime;
                if (remainingNumber < 0)
                {
                    this.remainingNumber = 0;
                }
                else
                {
                    this.remainingNumber = remainingNumber;
                }
            }
        }
    }
}

