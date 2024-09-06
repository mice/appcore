using UnityEngine;

public interface ITimer
{
    uint SetTimeOut(System.Action action, float second);
    uint SetInterval(System.Action action, float intervalInSec);
    uint NextFrame(System.Action action);
    uint ClearTimer(uint timeID);
}

public interface ILocalCache
{

}


public interface IResourceRequest
{
    bool isCompleted { get; }
    bool isError { get; }

    string category { get; }
    //as packageName
    string assetName { get; }

    string[] resourceNames {  get; }

    UnityEngine.Object loadedObj { get; }
}


public interface IResourceProvider
{
    IResourceRequest LoadAndInst(string cat, string res, Transform container,
        System.Action<IResourceRequest> callBack);

    IResourceRequest LoadAndInst(string cat, string res, Transform container,
        System.Action<IResourceRequest> callBack,
        System.Object levelData, short entityType, int isImport
        );

    void LoadASync<T>(string pack, string resName, System.Action<int, T> OnComplete) where T : UnityEngine.Object;

    void Despawn(GameObject gObject);
}