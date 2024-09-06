using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public static class RenderSortUtils
{
    /// <summary>
    /// 特效Render 用于修改Depth
    /// </summary>
    public static void SetMiniSortOrderFromRendererQueue(GameObject gObj, int topDepth, out int getMinDepth)
    {
        var _fxRenders = DataFactory.GetList<Renderer>();
        gObj.GetComponentsInChildren(true, _fxRenders);
        getMinDepth = 6000;
        foreach (var r in _fxRenders)
        {
            if (r.sharedMaterial != null)
            {
                getMinDepth = Mathf.Min(r.sharedMaterial.renderQueue, getMinDepth);
            }
        }

        foreach (var r in _fxRenders)
        {
            r.sortingOrder = topDepth + (r.sharedMaterial.renderQueue - getMinDepth);
        }
        DataFactory.Release(_fxRenders);
    }

    public static void SetMiniSortOrder(Transform gObj, int baseOrder)
    {
        var _fxRenders = DataFactory.GetList<Renderer>();
        gObj.GetComponentsInChildren(true, _fxRenders);
        var getMinDepth = 6000;
        foreach (var r in _fxRenders)
        {
            getMinDepth = Mathf.Min(r.sortingOrder, getMinDepth);
        }

        foreach (var r in _fxRenders)
        {
            r.sortingOrder = baseOrder + (r.sortingOrder - getMinDepth);
        }
        DataFactory.Release(_fxRenders);
    }

    public static void UpdateUISortOrder(Transform gameObject, int diff)
    {
        var spriteList = DataFactory.GetList<SpriteRenderer>();
        var meshProList = DataFactory.GetList<MeshRenderer>();//TextMeshPro的renderer为MeshRenderer
        var particleList = DataFactory.GetList<ParticleSystemRenderer>();
        gameObject.GetComponentsInChildren<SpriteRenderer>(true, spriteList);
        gameObject.GetComponentsInChildren<MeshRenderer>(true, meshProList);
        gameObject.GetComponentsInChildren<ParticleSystemRenderer>(true, particleList);

        foreach (var item in spriteList)
        {
            item.sortingOrder += diff;
        }
        foreach (var item in meshProList)
        {
            item.sortingOrder += diff;
        }
        foreach (var item in particleList)
        {
            item.sortingOrder += diff;
        }
        DataFactory.Release(spriteList);
        DataFactory.Release(meshProList);
        DataFactory.Release(particleList);
    }

    /// <summary>
    /// 更新FX的深度
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="diff"></param>
    public static void UpdateFXSortOrder(Transform gameObject, int diff)
    {
        var spriteList = DataFactory.GetList<SpriteRenderer>();
        var meshObjList = DataFactory.GetList<MeshRenderer>();
        var particleList = DataFactory.GetList<ParticleSystemRenderer>();
        var sortGroupList = DataFactory.GetList<SortingGroup>();
        gameObject.GetComponentsInChildren<SpriteRenderer>(true, spriteList);
        gameObject.GetComponentsInChildren<MeshRenderer>(true, meshObjList);
        gameObject.GetComponentsInChildren<ParticleSystemRenderer>(true, particleList);
        gameObject.GetComponentsInChildren<SortingGroup>(true, sortGroupList);

        foreach (var item in spriteList)
        {
            item.sortingOrder += diff;
        }
        foreach (var item in meshObjList)
        {
            item.sortingOrder += diff;
        }
        foreach (var item in particleList)
        {
            item.sortingOrder += diff;
        }

        foreach (var item in sortGroupList)
        {
            item.sortingOrder += diff;
        }

        DataFactory.Release(spriteList);
        DataFactory.Release(meshObjList);
        DataFactory.Release(particleList);
        DataFactory.Release(sortGroupList);
    }


    public static void SetSpriteRendererOrder(Transform gameObject, int sortOrder)
    {
        var rendererList = DataFactory.GetList<Renderer>();
        gameObject.GetComponentsInChildren<Renderer>(gameObject, rendererList);
        foreach (var item in rendererList)
        {
            item.sortingOrder += sortOrder;
        }
        DataFactory.Release(rendererList);
    }

    public static void UpdateParticlesOrder(Transform gameObject, int sortOrder)
    {
        var particleList = DataFactory.GetList<ParticleSystemRenderer>();
        gameObject.GetComponentsInChildren<ParticleSystemRenderer>(true, particleList);
        foreach (var item in particleList)
        {
            item.sortingOrder += sortOrder;
        }
        DataFactory.Release(particleList);
    }


    internal static void ShowRenderers(GameObject gObj, bool tmpShow)
    {
        var _fxRenders = DataFactory.GetList<Renderer>();
        gObj.GetComponentsInChildren(true, _fxRenders);

        foreach (var r in _fxRenders)
        {
            r.enabled = tmpShow;
        }
        DataFactory.Release(_fxRenders);
    }

    public static void SetSortGroupLayer(Transform target, int layerID =0)
    {
        if(target==null)
            return;

        var tmpInt = target.GetInstanceID();
        if(renderSortingLayerDict.TryGetValue(tmpInt,out var old_id) && old_id==layerID)
        {
            return;
        }
        renderSortingLayerDict[tmpInt] = layerID;
        var sortGroupList = DataFactory.GetList<SortingGroup>();
        target.GetComponentsInChildren<SortingGroup>(true, sortGroupList);

        if (sortGroupList.Count > 0)
        {
            foreach (var item in sortGroupList)
            {
                item.sortingLayerID = layerID;
            }
        }

        DataFactory.Release(sortGroupList);
    }

    public static void UpdateSortGroupOrder(Transform gameObject, int minDepth)
    {
        var sortGroupList = DataFactory.GetList<SortingGroup>();
        gameObject.GetComponentsInChildren<SortingGroup>(true, sortGroupList);

        if (sortGroupList.Count > 0)
        {
            var tmpMinDepth = sortGroupList[0].sortingOrder;
            foreach (var item in sortGroupList)
            {
                if (item.sortingOrder < tmpMinDepth)
                {
                    tmpMinDepth = item.sortingOrder;
                }
            }
            var tmpDiff = minDepth - tmpMinDepth;
            
            foreach (var item in sortGroupList)
            {
                item.sortingOrder += tmpDiff;
            }
        }

        DataFactory.Release(sortGroupList);
    }

    private static List<Type> NoneSortingTypes = new List<Type>(){typeof(SpriteMask)};
    private static HashSet<int> renderSortSetting = new HashSet<int>();
    private static Dictionary<int,int> renderSortingLayerDict = new Dictionary<int, int>();

    public static void AddToNoneSorting(System.Type type){
        ListExtensions.UniqAdd(NoneSortingTypes,type);
    }

    public static void AddSortGroupOnRenderer(GameObject gObj)
    {
        if(gObj==null)
            return;
        var tmpInt = gObj.GetInstanceID();
        if(renderSortSetting.Contains(tmpInt))
            return;
        renderSortSetting.Add(tmpInt);
        
        var rendererList = DataFactory.GetList<Renderer>();
        gObj.GetComponentsInChildren<Renderer>(true, rendererList);
        foreach (var item in rendererList)
        {
            var sortGroup = item.GetComponent<SortingGroup>();
            if (item.CompareTag("NOSORTING"))
            {
                if (sortGroup != null)
                {
                    sortGroup.enabled = false;
                }
                continue;
            }
            if (!NoneSortingTypes.Contains(item.GetType()))
            {
                if(sortGroup == null){
                    sortGroup = item.gameObject.AddComponent<SortingGroup>();
                }
                sortGroup.sortingOrder = item.sortingOrder;
            }
          
        }
        DataFactory.Release(rendererList);
    }
}
