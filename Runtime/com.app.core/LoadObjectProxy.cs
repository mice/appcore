using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LoadObjectProxy
{
    internal struct ResultNode
    {
        internal string Url;
        internal string Cat;
        internal Transform Target;

        public ResultNode(string cat,string url,Transform target)
        {
            this.Cat = cat;this.Url = url;this.Target = target;
        }
    }

    public Transform Container {get;private set;}
    public Action<LoadObjectProxy> OnLoadComplete{ get; private set;}
    public Action<GameObject> OnDespawn{ get; private set;}
    public Transform Target => Result?.Target;
    private System.Nullable<ResultNode> Result { get; set; }
    public string Url{get;private set;}
    public string Cat{get;private set;}
    private List<(string, string, IResourceRequest)> UnCompleteList { get; } = new List<(string, string, IResourceRequest)>();
    public bool HasEntityBind=>this.entityBind.HasValue;
    public bool IsLoading => UnCompleteList.Count > 0 && InLoading(Cat, Url);
    public bool IsTarget(string cat,string url)=> Url==url && Cat == cat;
    public bool IsResultTarget(string cat,string url)=>Result!=null && Result.Value.Url == url && Result.Value.Cat == cat && Result.Value.Target!=null;
    private bool IsBetweenLoading = false;
    //确保不为Null;
    public static IResourceProvider DefaultResLoader;
    public IResourceProvider ResLoader;
    public LoadObjectProxy Init(Transform container, Action<LoadObjectProxy> onLoadComplete
        , Action<GameObject> onDespawn = null
        ,IResourceProvider resLoader = null)
    {
        this.Container = container;
        this.OnLoadComplete = onLoadComplete;
        this.OnDespawn = onDespawn;
        this.ResLoader = resLoader ?? DefaultResLoader;
        return this;
    }

    protected System.Nullable<(System.Object levelData, short entityType,int isImport)> entityBind;
    public void BindEntity(System.Object levelData, short entityType,int isImport = 1)
    {
        if (levelData != null)
        {
            entityBind = (levelData, entityType, isImport);
        }
        else
        {
            entityBind = null;
        }
    }

    public void Load(string cat, string url)
    {
        if (Result.HasValue)
        {
            var item = Result.Value;
            if (item.Target != null && item.Cat == cat && item.Url == url)
            {
                OnLoadComplete?.Invoke(this);
                return;
            }
            if (item.Target != null)
            {
                OnDespawn?.Invoke(item.Target.gameObject);
                ResLoader.Despawn(item.Target.gameObject);
            }
            Result = null;
        }

        this.Cat = cat;
        this.Url = url;
        if (!InLoading(cat, Url))
        {
            IsBetweenLoading = true;
            IResourceRequest t;
            if (entityBind.HasValue)
            {
                //不会立马加载
                t = ResLoader.LoadAndInst(cat, url, Container, _OnEntityModelLoaded, entityBind.Value.levelData, entityBind.Value.entityType, entityBind.Value.isImport);
            }
            else
            {
                t = ResLoader.LoadAndInst(cat, url, Container, OnLoaded);
            }
            IsBetweenLoading = false;
            if (t.isCompleted)
            {
                RemoveReq(t);
                OnApplyLoad(t, true);
            }
            else
            {
                UnCompleteList.Add((Cat, Url, null));
            }
        }
    }

    private void _OnEntityModelLoaded(IResourceRequest request)
    {
        if (IsBetweenLoading)
            return;
        if (request == null)
        {
            Core.Debug.LogWarning("Should Not Null::" + Cat + ":" + Url);
            //表示已经进入了清理通道
            if(this.OnLoadComplete == InLoadingWatchComplete)
            {
                //会导致永远无法回收
                return;
            }
          
            if(entityBind==null){
                return;
            }else{
                //这里表示不在对于LOD
                //如果当前没有被清理.那么就需要尝试:ResManager.LoadAndInst
                this.entityBind = null;
                if(!string.IsNullOrEmpty(Url)){
                    this.UnCompleteList.Clear();
                    Load(this.Cat,this.Url);
                }
            }
        }else{
            var tmpCount = UnCompleteList.Count;
            bool findTarget = RemoveReq(request);
            OnApplyLoad(request, findTarget);
        }
    }

    private void OnLoaded(IResourceRequest request)
    {
        if (IsBetweenLoading)
            return;

        var tmpCount = UnCompleteList.Count;
        bool findTarget = RemoveReq(request);
        OnApplyLoad(request, findTarget);
    }

    private bool RemoveReq(IResourceRequest request)
    {
        bool findTarget = false;

        for (int i = UnCompleteList.Count - 1; i >= 0; i--)
        {
            var item = UnCompleteList[i];
            if (item.Item1 == request.category && item.Item2 == request.assetName)
            {
                UnCompleteList.RemoveAt(i);
                if (item.Item1 == Cat && item.Item2 == Url)
                {
                    findTarget = true;
                }
            }
        }
        return findTarget;
    }

    private void OnApplyLoad(IResourceRequest request, bool findTarget)
    {
        var go = GetObject(request);
        GameObject needDes = null;
        if (findTarget)
        {
            //上一次的
            if (go != null)
            {
                Result = new ResultNode(Cat,Url,go.transform);
            }
            else
            {
                Result = null;
            }

            OnLoadComplete?.Invoke(this);
        }
        else
        {
            needDes = go;
        }
        if (needDes != null)
        {
            OnDespawn?.Invoke(needDes);
            ResLoader.Despawn(needDes);
        }
    }

    private GameObject GetObject(IResourceRequest request)
    {
        if (request == null || request.isError || request.loadedObj == null)
        {
            if(!entityBind.HasValue){
                Core.Debug.LogWarning($"[LoadObjectProxy]:{request.assetName} Load Failed");
            }
            return null;
        }
        return request.loadedObj as GameObject;
    }

    public void ClearResult()
    {
        Url = Cat = string.Empty;
        if (Result.HasValue && Result.Value.Target != null)
        {
            OnDespawn?.Invoke(Result.Value.Target.gameObject);
            ResLoader.Despawn(Result.Value.Target.gameObject);
        }
       
        Result = null;
    }

    public void Clear()
    {
        UnCompleteList.Clear(); 
        ClearResult();
        this.Container = null;
        Result = null;
        this.entityBind = null;
        this.OnDespawn = null;
        OnLoadComplete = null;
    }

    private bool InLoading(string cat, string url)
    {
        foreach (var item in UnCompleteList)
        {
            if (item.Item1 == cat && item.Item2 == url)
            {
                return true;
            }
        }
        return false;
    }

    public static LoadObjectProxy Spawn()
    {
        return DataFactory.Get<LoadObjectProxy>();
    }

    public static void Despawn(LoadObjectProxy instance)
    {
        if (instance.UnCompleteList.Count > 0)
        {
            instance.ClearResult();
            instance.Container = null;
            instance.OnDespawn = null;
            instance.OnLoadComplete = InLoadingWatchComplete;
        }
        else
        {
            instance.Clear();
            DataFactory.Release(instance);
        }
    }

    private static void InLoadingWatchComplete(LoadObjectProxy instance)
    {
        if (instance.UnCompleteList.Count == 0)
        {
            instance.Clear();
            DataFactory.Release(instance);
        }
    }
}
