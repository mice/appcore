#define USE_DICT
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Pool
{
    public class SpawnPool
    {
        public static Vector3 zeroVector = new Vector3(0.0001f, 0, 0);
        public static Vector3 resetPosition = new Vector3(10000, 10000, 0);
        public PoolManager poolManager;
        public string category { private set; get; }
        public string assetName { private set; get; }
        private bool cullingActive = false;
        public int maxDespawnedCount = 4;
        public bool cullDespawned = true;
        public UnityEngine.Coroutine cullDespawnedCoroutine;
        public int cullDelay = 20;
        public int cullAbove = 0;
        public int cullMaxPerPass = 5;
        public GameObject prefabObj;

        public bool despawnedParent = true;
        public bool despawnedScale = false;

        private float _cullingTime = 0;

        public SpawnPool(PoolManager manager, string category, string assetname)
        {
            this.poolManager = manager;
            this.category = category;
            this.assetName = assetname;
        }

        public int totalCount
        {
            get
            {
                // Add all the items in the pool to get the total count
                int count = 0;
                count += this._spawned.Count;
                count += this._despawned.Count;
                return count;
            }
        }

        public string poolName = string.Empty;
#if USE_DICT
        internal Dictionary<int, Transform> _spawned = new Dictionary<int, Transform>();
        public System.Collections.Generic.List<Transform> spawned { get { return new System.Collections.Generic.List<Transform>(this._spawned.Values); } }
#else
        internal List<Transform> _spawned = new List<Transform>();
        public List<Transform> spawned { get { return new List<Transform>(this._spawned); } }
#endif
        internal System.Collections.Generic.List<Transform> _despawned = new System.Collections.Generic.List<Transform>();
        public System.Collections.Generic.List<Transform> despawned { get { return new System.Collections.Generic.List<Transform>(this._despawned); } }



        public bool IsSpawned(Transform instance)
        {
#if USE_DICT
            return instance != null && this._spawned.ContainsKey(instance.GetInstanceID());
#else
            return this._spawned.Contains(instance);
#endif
        }
        public void DespawnAll()
        {
            var spawned = this.spawned;
            for (int i = 0; i < spawned.Count; i++)
            {
                var inst = spawned[i];
                poolManager.OnDespawn(inst);
                this.Despawn(inst);
            }

        }

        public void RemoveAllMissObject()
        {
            //int count = 0;
            //int missCount = 0;
            var tempList = this._despawned;
            for (int i = tempList.Count - 1; i >= 0; i--)
            {
                var inst = tempList[i];
                if (inst == null)
                {
                    //count++;
                    tempList.RemoveAt(i);
                }
            }
#if USE_DICT
            if (_spawned.Count > 0)
            {
                System.Collections.Generic.List<int> missObjectIDs = new System.Collections.Generic.List<int>();
                foreach (var item in _spawned)
                {
                    if (item.Value == null)
                    {
                        missObjectIDs.Add(item.Key);
                        //missCount++;
                    }
                }
                foreach (var item in missObjectIDs)
                {
                    _spawned.Remove(item);
                }
            }
#else
            tempList = this._spawned;            
            for (int i = tempList.Count - 1; i >= 0; i--)
            {
                var inst = tempList[i];
                if (inst == null)
                {
                    //count++;
                    tempList.RemoveAt(i);                    
                }                    
            }
#endif
            //if(count >0 || missCount > 0)
            //Debug.LogError($" {Category} {AssetName}  count : {count}   missCount:{missCount}");
        }

        public void ReleaseAll()
        {
            DespawnAll();
            if (this.cullingActive && cullDespawnedCoroutine != null)
            {
                poolManager.StopCoroutine(cullDespawnedCoroutine);
                cullDespawnedCoroutine = null;
                this.cullingActive = false;
            }
            var despawnedObjs = this._despawned;
            for (int i = despawnedObjs.Count - 1; i >= 0; i--)
            {
                var inst = despawnedObjs[i];
                if (inst != null)
                    DestroyInstance(inst.gameObject);
            }
            _despawned.Clear();
        }

        public void DestroyAll()
        {
            ReleaseAll();
            if (prefabObj != null)
            {
                DestroyInstance(prefabObj);
            }

        }

        public void Despawn(Transform instance, bool worldPositionStays = false)
        {
            DespawnInstance(instance, worldPositionStays);
        }
        public Transform SpawnNew(Transform parent = null)
        {


            GameObject instGO = null;
            if (prefabObj != null)
            {
                if (poolManager.instantiateDelegate != null)
                {
                    instGO = poolManager.instantiateDelegate(prefabObj, parent);
                }
                else
                {
                    instGO = GameObject.Instantiate(prefabObj, parent);
                }

            }
            else
            {
                try
                {
                    instGO = poolManager.getInstance(category, assetName, parent);
                }
                catch (Exception e)
                {
                    LogError($"[SpawnPool.SpawnNew] cate={category} path={assetName} {e.ToString()}");
                    return null;
                }
            }
            var poolObject = poolManager.AddPoolObject(this, instGO);
            poolObject.Pool = this;
            Transform inst = instGO.transform;
            bool worldPositionStays = false;// !(inst is RectTransform);
            if (parent != null)  // User explicitly provided a parent
            {
                if (inst.parent != parent)
                    inst.SetParent(parent, worldPositionStays);
                else if (!worldPositionStays)
                {
                    inst.localPosition = Vector3.zero;
                }
            }
            else if (inst.parent != poolManager.GetInactiveParent())
            {
                inst.SetParent(poolManager.GetInactiveParent(), worldPositionStays);
            }
            AddInstance(inst);

            return inst;
        }

        internal bool CanSpawn()
        {
            return this._despawned.Count > 0;
        }
        internal Transform SpawnFormDespawned()
        {
            Transform inst = null;
            if (this._despawned.Count > 0)
            {
                inst = this._despawned[0];
                this._despawned.RemoveAt(0);
                if (inst == null && this._despawned.Count > 0)
                {
                    return SpawnFormDespawned();
                }
            }
            return inst;
        }
        internal Transform SpawnInstance(Transform parent)
        {
            Transform inst = null;
            if (this._despawned.Count == 0)
            {
                inst = this.SpawnNew(parent);
            }
            else
            {
                inst = SpawnFormDespawned();
                if (inst == null)
                {
                    Log($" {category} {assetName} , Make sure you didn't delete a despawned instance directly.");
                    inst = this.SpawnNew(parent);
                }else
                {
                    AddInstance(inst);
                }

                if (parent != null)  // User explicitly provided a parent
                {
                    if (inst.parent != parent)
                    {
                        inst.SetParent(parent,false);
                        
                    }
                    RectTransform rectTransform = inst as RectTransform;
                    if (rectTransform == null)
                    {
                        inst.localPosition = Vector3.zero;
                        inst.rotation = Quaternion.identity;
                    }
                    else
                    {
                        rectTransform.anchoredPosition3D = Vector3.zero;
                    }
                }
                else if (inst.parent != poolManager.GetInactiveParent())
                {
                    inst.SetParent(poolManager.GetInactiveParent(), false);
                }

               
                if (despawnedScale)
                {
                    inst.localScale = Vector3.one;
                }

                if (!despawnedParent)
                {
                    GameObject go = inst.gameObject;
                    if (!go.activeSelf)
                        go.SetActive(true);
                }
            }
            poolManager.OnSpawn(inst);
            return inst;
        }

        public bool Contains(Transform transform)
        {
            bool contains;
#if USE_DICT
            contains = transform != null && this._spawned.ContainsKey(transform.GetInstanceID());
#else
            contains = this._spawned.Contains(transform);
#endif
            if (contains)
                return true;

            contains = this._despawned.Contains(transform);
            if (contains)
                return true;

            return false;
        }

        internal void AddInstance(Transform transform)
        {
            _cullingTime = Time.realtimeSinceStartup + this.cullDelay*2;
#if USE_DICT
            int instanceID = transform.GetInstanceID();
            if (this._spawned.TryGetValue(instanceID, out Transform tf))
            {
                if(tf == null)
                {
                    this._spawned[instanceID] = transform;
                }else
                {
                    LogError($"AddInstance Error {transform} {instanceID}");
                }
            }else
            {
                this._spawned.Add(instanceID, transform);
            }
            
#else
            _spawned.Add(transform);
#endif
        }

        internal bool DespawnInstance(Transform xform, bool worldPositionStays = false)
        {
#if USE_DICT
            bool removed = xform != null && this._spawned.Remove(xform.GetInstanceID());
#else
            bool removed = this._spawned.Remove(xform);
#endif
            if (this._despawned.Count > maxDespawnedCount)
            {
                DestroyInstance(xform.gameObject);
                return true;
            }
            if (removed)
            {
                this._despawned.Add(xform);
                if (despawnedParent)
                {
                    xform.SetParent(poolManager.GetInactiveParent(), worldPositionStays);
                }
                else
                {
                    GameObject go = xform.gameObject;
                    if (go.activeSelf)
                    {
                        xform.localPosition = resetPosition;
                    }
                }
                if (despawnedScale)
                {
                    xform.localScale = zeroVector;
                }
                if (!this.cullingActive &&   // Cheap & Singleton. Only trigger once!
                     this.cullDespawned)
                {
                    this.cullingActive = true;
                    cullDespawnedCoroutine = this.poolManager.StartCoroutine(CullDespawned());
                }
            }



            return true;
        }


        internal void DestroyInstance(GameObject instance)
        {
            poolManager.allPoolObjs.Remove(instance.GetInstanceID());
            UnityEngine.Object.Destroy(instance);
        }

        [System.Diagnostics.Conditional("ENABLE_LOG")]
        internal void Log(String content){
            UnityEngine.Debug.Log(content);
        }

        [System.Diagnostics.Conditional("ENABLE_LOG")]
        internal void LogError(String content){
            UnityEngine.Debug.LogError(content);
        }

        private WaitForSeconds cullDelayCor;
        internal IEnumerator CullDespawned()
        {
            if( cullDelayCor==null ) {
                cullDelayCor = new WaitForSeconds(this.cullDelay);
            }
            // First time always pause, then check to see if the condition is
            //   still true before attempting to cull.
            yield return cullDelayCor;

            while (this._despawned.Count > 0)
            {
                if(Time.realtimeSinceStartup >= _cullingTime)
                {
                    for(var i=0; i<_despawned.Count; i++) {
                    
                        Transform inst = this._despawned[i];
                        if (inst != null)
                        {
                            DestroyInstance(inst.gameObject);
                        }
                    }
                    _despawned.Clear();
                    PoolManager.I.isPoolCleared = true;
                    // Attempt to delete an amount == this.cullMaxPerPass
                    // for (int i = 0; i < this.cullMaxPerPass; i++)
                    // {
                    //     // Break if this.cullMaxPerPass would go past this.cullAbove
                    //     if (this._despawned.Count <= this.cullAbove)
                    //         break;  // The while loop will stop as well independently

                    //     // Destroy the last item in the list
                    //     if (this._despawned.Count > 0)
                    //     {
                    //         Transform inst = this._despawned[this._despawned.Count-1];
                    //         this._despawned.RemoveAt(this._despawned.Count-1);
                    //         if (inst != null)
                    //         {
                    //             DestroyInstance(inst.gameObject);
                    //         }

                    //     }
                    // }
                }
                

                // Check again later
                yield return cullDelayCor;
            }


            // Reset the singleton so the feature can be used again if needed.
            this.cullingActive = false;
            cullDespawnedCoroutine = null;
            yield return null;
        }
    }
}