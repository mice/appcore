using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Pool
{
    public class PoolManager : MonoBehaviour
    {
        public bool isPoolCleared = false;
        /// <summary>
        /// 当前继承IPoolObject类
        /// </summary>
        public System.Type poolObjectType;          
        public delegate GameObject GetInstanceDelegate(string category, string assetname,Transform parent);
        public delegate GameObject InstantiateDelegate(GameObject prefab, Transform parent);
        /// <summary>
        /// 获取一个实例
        /// </summary>
        public GetInstanceDelegate getInstance;
        /// <summary>
        /// 创建一个实例，主要是对应SpawnPool里设置了Prefab的SpawnInstance
        /// </summary>
        public InstantiateDelegate instantiateDelegate;

        public static void Init()
        {
            I = new GameObject("PoolManager").AddComponent<PoolManager>();
            if(Application.isPlaying)
            {
                DontDestroyOnLoad(I.gameObject);
            }
            GameObject inactiveObj = new GameObject("inactive");
            inactiveObj.SetActive(false);
            I.inactiveParent = inactiveObj.transform;
            I.inactiveParent.localPosition = new Vector3(33333, 0, 0);
            //I.inactiveParent.localScale = new Vector3(0.001f,0,0);            
            I.inactiveParent.SetParent(I.transform, false);

            UnityEngine.SceneManagement.SceneManager.sceneUnloaded += SceneManager_sceneUnloaded;
        }

        private static void SceneManager_sceneUnloaded(Scene arg0)
        {
            I.OnSceneUnload();
        }

        private void OnDestroy() {
            UnityEngine.SceneManagement.SceneManager.sceneUnloaded -= SceneManager_sceneUnloaded;
            DataFactory.Clear();
        }
        
        public static PoolManager I;
        /// <summary>
        /// 所有SpawnPool
        /// </summary>
        private Dictionary<string, Dictionary<string, SpawnPool>> spawnPools = new Dictionary<string, Dictionary<string, SpawnPool>>();
       /// <summary>
       /// 所有PoolObject，用于快速查找避免调用GetCompoent
       /// </summary>
        public Dictionary<int, IPoolObject> allPoolObjs = new Dictionary<int, IPoolObject>();
        /// <summary>
        /// 所有SpawnPool
        /// </summary>
        public Dictionary<string, Dictionary<string, SpawnPool>> SpawnPools { get { return spawnPools; } }

        /// <summary>
        /// 在获取IPoolObject时 避免GetComponents时频繁New List
        /// </summary>
        private System.Collections.Generic.List<Component> components = new System.Collections.Generic.List<Component>();

        private System.Collections.Generic.List<IPoolObject> tempPoolObjectList = new System.Collections.Generic.List<IPoolObject>();

        /// <summary>
        /// 回收物体的父级
        /// </summary>
        public Transform inactiveParent;

        /// <summary>
        /// 获取或创建一个SpawnPool，永不为 null
        /// </summary>
        /// <param name="category"></param>
        /// <param name="assetname"></param>
        /// <returns></returns>
        public SpawnPool GetOrCreatePool(string category, string assetname)
        {            
            SpawnPool spawnPool = null;
            Dictionary<string, SpawnPool> sp = null;
            spawnPools.TryGetValue(category, out sp);
            if (sp == null)
            {
                sp = new Dictionary<string, SpawnPool>();
                spawnPools.Add(category, sp);
            }
            else
            {
                sp.TryGetValue(assetname, out spawnPool);

            }

            if (spawnPool == null)
            {
                spawnPool = new SpawnPool(this, category, assetname);
                sp.Add(assetname, spawnPool);
            }
            return spawnPool;
        }
        /// <summary>
        /// 获取一个SpawnPool，有可能为 null
        /// </summary>
        /// <param name="category"></param>
        /// <param name="assetname"></param>
        /// <returns></returns>
        public SpawnPool GetPool(string category,string assetname)
        {
            SpawnPool spawnPool = null;
            Dictionary<string, SpawnPool> sp = null;
            spawnPools.TryGetValue(category, out sp);
            if (sp == null)
            {
                return null;
            }
            else
            {
                sp.TryGetValue(assetname, out spawnPool);

            }
            return spawnPool;
        }

        /// <summary>
        /// 获取一个新实例，Despawn列表为空时候直接SpawnNew
        /// </summary>
        /// <param name="category"></param>
        /// <param name="assetname"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public Transform SpawnInstance(string category, string assetname, Transform parent = null)
        {
            SpawnPool spawnPool = GetPool(category, assetname);
            if (spawnPool == null)
            {
                return null;
            }
            Transform trans = spawnPool.SpawnInstance(parent);
            return trans;
        }

        /// <summary>
        /// 从Despawn列表从获取一个实例，列表内没有多余的话返回 null
        /// </summary>
        /// <param name="category"></param>
        /// <param name="assetname"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public Transform Spawn(string category, string assetname,Transform parent = null)
        {
            SpawnPool spawnPool = GetPool(category, assetname);
            if(spawnPool == null)
            {
                return null;
            }
            if(spawnPool.CanSpawn())
            {
                Transform trans = spawnPool.SpawnInstance(parent);
                return trans;
            }
            return null;
        }

        /// <summary>
        /// 当前物体是否被池管理
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public bool IsPoolObject(GameObject go)
        {
            return allPoolObjs.ContainsKey(go.GetInstanceID());
        }
        /// <summary>
        /// 回收一个物体，不在池中管理的直接Destory
        /// </summary>
        /// <param name="go"></param>
        /// <param name="worldPositionStays"></param>
        /// <returns></returns>
        public bool Despawn(GameObject go, bool worldPositionStays = false)
        {
            if(go == null)
            {
                return false;
            }       
            if(!_Despawn(go,worldPositionStays))
            {
//                Debug.LogError($"{go} {go.name} not in pool");
                UnityEngine.Object.Destroy(go);
                return false;
            }
            
            return true;
        }

        private bool _Despawn(GameObject go, bool worldPositionStays = false)
        {
            IPoolObject poolObject = null;
            allPoolObjs.TryGetValue(go.GetInstanceID(), out poolObject);
            if (poolObject != null)
            {
                SpawnPool spawnPool = poolObject.Pool;
                //if(spawnPool.IsSpawned(poolObject.GetTransform()))
                {
                    OnDespawn(poolObject.GetTransform());
                    spawnPool.Despawn(poolObject.GetTransform(),worldPositionStays);
                }
                return true;
            }
            return false;
        }
        public void Despawn(IPoolObject poolObj, bool worldPositionStays = false)
        {
            if (poolObj == null)
                return;
            var trans = poolObj.GetTransform();            
            poolObj.OnDespawn();
            poolObj.Pool?.Despawn(trans, worldPositionStays);
        }
        /// <summary>
        /// 如果有对应的池则创建一个实例，没有池则返回null
        /// </summary>
        /// <param name="category"></param>
        /// <param name="assetname"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public Transform CheckAndSpawn(string category, string assetname,Transform parent = null)
        {
            SpawnPool spawnPool = GetPool(category, assetname);
            if(spawnPool != null)
            {
                return spawnPool.SpawnInstance(parent);
            }
            return null;
        }

        /// <summary>
        /// 是否有对应的池
        /// </summary>
        /// <param name="category"></param>
        /// <param name="assetname"></param>
        /// <returns></returns>
        public bool IsInPool(string category, string assetname)
        {
            return GetPool(category, assetname) != null;
        }
        /// <summary>
        /// 是否有多余的Despawn的实例
        /// </summary>
        /// <param name="category"></param>
        /// <param name="assetname"></param>
        /// <returns></returns>
        public bool CanSpawn(string category, string assetname)
        {
            SpawnPool spawnPool = GetPool(category, assetname);
            return spawnPool != null && spawnPool.CanSpawn();
        }

        /// <summary>
        /// 获取Despawn的默认父级
        /// </summary>
        /// <returns></returns>
        public Transform GetInactiveParent()
        {
            return inactiveParent;
        }
        /// <summary>
        /// 把物体添加到池中管理，没有池则创建一个新池来管理
        /// </summary>
        /// <param name="category"></param>
        /// <param name="assetname"></param>
        /// <param name="go"></param>
        public void AddPool(string category, string assetname, GameObject go)
        {
            SpawnPool spawnPool = GetOrCreatePool(category, assetname);
            var poolObject = AddPoolObject(spawnPool, go);
            OnSpawn(poolObject.GetTransform());
            spawnPool.AddInstance(poolObject.GetTransform());
        }

        /// <summary>
        /// 把物体添加到池中管理，没有池则创建一个新池来管理
        /// </summary>
        /// <param name="category"></param>
        /// <param name="assetname"></param>
        /// <param name="go"></param>
        public void AddPool(string category, string assetname, IPoolObject go)
        {
            SpawnPool spawnPool = GetOrCreatePool(category, assetname);
            var poolObject = AddPoolObject(spawnPool, go);
            OnSpawn(poolObject.GetTransform());
            spawnPool.AddInstance(poolObject.GetTransform());
        }

        /// <summary>
        /// 移除一个池
        /// </summary>
        /// <param name="category"></param>
        /// <param name="assetname"></param>
        public void RemovePool(string category, string assetname)
        {
            SpawnPool spawnPool = GetPool(category, assetname);
            if(spawnPool != null)
            {
                Dictionary<string, SpawnPool> sp = null;
                spawnPools.TryGetValue(category, out sp);
                sp.Remove(assetname);
                spawnPools.Remove(category);
                spawnPool.DestroyAll();
            }
        }
        /// <summary>
        /// 释放一个池，销毁所有池里的物体
        /// </summary>
        /// <param name="category"></param>
        /// <param name="assetname"></param>
        public void ReleasePool(string category,string assetname)
        {
            SpawnPool spawnPool = GetPool(category, assetname);
            if (spawnPool != null)
            {
                spawnPool.ReleaseAll();
            }
        }
        /// <summary>
        /// 创建一个Prefab池
        /// </summary>
        /// <param name="category"></param>
        /// <param name="assetname"></param>
        /// <param name="prefab"></param>
        public void AddPrefabPool(string category, string assetname, GameObject prefab)
        {
            SpawnPool spawnPool = GetOrCreatePool(category, assetname);
            IPoolObject poolObject= AddPoolObject(spawnPool, prefab);
            if(spawnPool.prefabObj != null)
            {
                spawnPool.DestroyAll();
                spawnPool.prefabObj = null;
            }
            spawnPool.prefabObj = GameObject.Instantiate(prefab, spawnPool.poolManager.GetInactiveParent());
            spawnPool.AddInstance(poolObject.GetTransform());
        }

        /// <summary>
        /// 设置当前的IPoolObject类型
        /// </summary>
        /// <param name="t"></param>
        public void SetPoolType(System.Type t)
        {
#if UNITY_EDITOR
            if(t.GetInterface(typeof(IPoolObject).Name) == null)
            {
                UnityEngine.Debug.LogError(t + " is not IPoolObject");
            }
#endif
            poolObjectType = t;
        }

        //public IPoolObject AddPoolObject(GameObject go)
        //{
        //    var poolObject = go.GetComponent(poolObjectType) as IPoolObject;
        //    if(poolObject != null)
        //    {
        //        allPoolObjs.Add(go.GetInstanceID(), poolObject);
        //    }
        //    return poolObject;
        //}

        /// <summary>
        /// 为物体添加池引用关系
        /// </summary>
        /// <param name="pool"></param>
        /// <param name="go"></param>
        /// <returns></returns>
        public IPoolObject AddPoolObject(SpawnPool pool, GameObject go)
        {
            var poolObject = go.GetComponent(poolObjectType) as IPoolObject;
            if (poolObject == null)
            {
                poolObject = go.AddComponent(poolObjectType) as IPoolObject;
                poolObject.Pool = pool;
            }
            else
            {
                poolObject.Pool = pool;
            }
            poolObject.Cached = true;            
            allPoolObjs.Add(go.GetInstanceID(), poolObject);
            return poolObject;
        }

        /// <summary>
        /// 为物体添加池引用关系
        /// </summary>
        /// <param name="pool"></param>
        /// <param name="go"></param>
        /// <returns></returns>
        public IPoolObject AddPoolObject(SpawnPool pool, IPoolObject poolObject)
        {
            poolObject.Pool = pool;            
            allPoolObjs.Add(poolObject.GetTransform().gameObject.GetInstanceID(), poolObject);
            return poolObject;
        }

        internal void OnSpawn(Transform trans)
        {
            components.Clear();
            trans.GetComponents(poolObjectType, components);

            if (components.Count > 1)
            {
                if(tempPoolObjectList.Count > 0)
                {
                    tempPoolObjectList.Clear();
                }
                System.Collections.Generic.List<IPoolObject> poolObjects = tempPoolObjectList;
                for (int i = 0; i < components.Count; i++)
                {
                    poolObjects.Add(components[i] as IPoolObject);
                }
                for (int i = 0; i < poolObjects.Count; i++)
                {
                    poolObjects[i].OnSpawn();
                }
                tempPoolObjectList.Clear();

            }
            else if (components.Count == 1)
            {
                IPoolObject p = components[0] as IPoolObject;
                p.OnSpawn();
            }
            components.Clear();
        }

        internal void OnDespawn(Transform trans)
        {
            components.Clear();
            trans.GetComponents(poolObjectType, components);

            if(components.Count > 1)
            {
                if(tempPoolObjectList.Count > 0)
                {
                    tempPoolObjectList.Clear();
                }
                System.Collections.Generic.List<IPoolObject> poolObjects = tempPoolObjectList;
                for (int i = 0; i < components.Count; i++)
                {
                    poolObjects.Add(components[i] as IPoolObject);                    
                }
                for (int i = 0; i < poolObjects.Count; i++)
                {                    
                    poolObjects[i].OnDespawn();
                }
                tempPoolObjectList.Clear();

            }
            else if(components.Count == 1)
            {
                IPoolObject p = components[0] as IPoolObject;
                p.OnDespawn();
            }
            components.Clear();
        }

        /// <summary>
        /// 获取被池管理的物体上的IPoolObject
        /// </summary>
        /// <param name="trans"></param>
        /// <returns></returns>
        public IPoolObject GetPoolObject(Transform trans)
        {
            IPoolObject poolObject = null;
            if(trans !=null)
                allPoolObjs.TryGetValue(trans.gameObject.GetInstanceID(), out poolObject);
            return poolObject;
        }

        /// <summary>
        /// 当一个场景卸载时清理Miss的Object
        /// </summary>
        public void OnSceneUnload()
        {
            foreach (var item in spawnPools)
            {
                foreach (var v in item.Value)
                {
                    v.Value.RemoveAllMissObject();
                }
            }
        }


#if UNITY_EDITOR
        [ContextMenu("LogAll")]
        public void LogAll()
        {
            foreach (var item in spawnPools)
            {
                foreach (var v in item.Value)
                {
                    UnityEngine.Debug.LogError($"{item.Key}   {v.Key} count:{v.Value.totalCount} spawned:{v.Value._spawned.Count} despawned:{v.Value._despawned.Count}");
                }
            }
        }

        [ContextMenu("LogAlive")]
        public void LogAlive()
        {
            foreach (var item in spawnPools)
            {
                foreach (var v in item.Value)
                {
                    if(v.Value.totalCount>0)
                        UnityEngine.Debug.LogError($"{item.Key}   {v.Key} count:{v.Value.totalCount} spawned:{v.Value._spawned.Count} despawned:{v.Value._despawned.Count}");
                }
            }
        }
#endif

    }
}
