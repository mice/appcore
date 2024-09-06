using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public static class Utility
    {
        private static System.Action<System.Object> _ReleaseFunc = _Release;

        private static void _Release(System.Object obj)
        {

        }

        public static void SetRelease(System.Action<System.Object> releaseFunc)
        {
            _ReleaseFunc = releaseFunc == null ? _Release : releaseFunc;
        }

        public static void SetActiveChildren(Transform parent, bool is_active)
        {
            if (parent != null)
            {
                int i = 0;
                int childCount = parent.childCount;
                while (i < childCount)
                {
                    parent.GetChild(i).gameObject.SetActive(is_active);
                    i++;
                }
            }
        }

        /// <summary></summary>
        public static Transform FindChild(Transform transform, string name)
        {
            Transform transform2 = transform.Find(name);
            if (transform2 != null)
            {
                return transform2;
            }
            int i = 0;
            int childCount = transform.childCount;
            while (i < childCount)
            {
                transform2 = Utility.FindChild(transform.GetChild(i), name);
                if (transform2 != null)
                {
                    return transform2;
                }
                i++;
            }
            return null;
        }

        public static int CountChildLayer(Transform root, Transform child)
        {
            int count = 0;
            var tmpParent = child;
            while (true)
            {
                if (tmpParent == null || tmpParent == root)
                {
                    break;
                }
                tmpParent = tmpParent.parent;
                count++;
            }
            return count;
        }


        public static Transform CreateGameObject(string name, Transform parent, int layer = -1)
        {
            GameObject gameObject = new GameObject(name);
            Transform transform = gameObject.transform;
            if (parent != null)
            {
                Attach(parent, transform);
            }
            if (layer != -1)
            {
                gameObject.layer = layer;
            }
            return transform;
        }

        public static void Attach(Transform parent, Transform child)
        {
            Vector3 localPosition = child.localPosition;
            Quaternion localRotation = child.localRotation;
            Vector3 localScale = child.localScale;
            child.SetParent(parent);
            child.localPosition = localPosition;
            child.localRotation = localRotation;
            child.localScale = localScale;
        }

        public static void Norm(Transform target)
        {
            target.localPosition = Vector3.zero;
            target.localRotation = Quaternion.identity;
            target.localScale = Vector3.one;
        }

        public static Component AddComponentAndGo<T>(this GameObject go, string name) where T : Component
        {
            GameObject gameObject = new GameObject(name);
            gameObject.transform.SetParent(go.transform);
            return gameObject.AddComponent<T>();
        }

        public static bool GetScreenRaycastHitObject<T>(Camera camera, Vector3 touchPos, int layer, ref T obj)
        {
            Ray ray = camera.ScreenPointToRay(touchPos);
            RaycastHit rayhit;
            if (Physics.Raycast(ray, out rayhit, 500, layer))
            {
                obj = rayhit.collider.GetComponent<T>();
                return true;
            }
            else
            {
                obj = default(T);
                return false;
            }
        }

        public static Transform CreateOrGetInChild(Transform container, string name, System.Action<GameObject> onCreated)
        {
            if (container == null)
            {
                UnityEngine.Debug.LogWarning("container is Null");
                return null;
            }
            var fx_building_container = container.Find(name);
            if (fx_building_container == null)
            {
                fx_building_container = new GameObject(name).transform;
                Attach(container, fx_building_container);
                onCreated?.Invoke(fx_building_container?.gameObject);
            }
            return fx_building_container;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static T DisposeTarget<T>(T target) where T : class, IDisposable
        {
            if (target != null)
            {
                target.Dispose();
            }
            return default;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void DisposeTargets<T>(IList<T> targets) where T : class, IDisposable
        {
            foreach (var item in targets)
            {
                item?.Dispose();
            }
            targets.Clear();
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void DisposeAndReleaseTargets<T>(IList<T> targets) where T : class, IDisposable
        {
            foreach (var item in targets)
            {
                if (item != null)
                {
                    item.Dispose();
                    _ReleaseFunc.Invoke(item);
                }
            }
            targets.Clear();
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static T DisposeAndReleaseTarget<T>(T target) where T : class, IDisposable
        {
            if (target != null)
            {
                target.Dispose();
                _ReleaseFunc.Invoke(target);
            }
            return default(T);
        }

        public static bool IsParentOfSelf(Transform container, Transform child)
        {
            while (child != null)
            {
                if (child.parent == container)
                {
                    return true;
                }
                child = child.parent;
            }
            return false;
        }

        public static void Reactive(MonoBehaviour behavior)
        {
            if (behavior != null)
            {
                if (behavior.gameObject.activeSelf)
                {
                    behavior.gameObject.SetActive(false);
                }
                behavior.gameObject.SetActive(true);
            }
        }


        public static void LitOn(this Light cam, int layer)
        {
            var flags = cam.cullingMask;
            cam.cullingMask = flags | (1 << layer);
        }

        public static void LitOff(this Light cam, int layer)
        {
            var flags = cam.cullingMask;
            cam.cullingMask = flags & ~(1 << layer);
        }

        public static void SetActive(Transform tfm, bool isActive)
        {
            if (tfm != null && tfm.gameObject.activeSelf != isActive)
            {
                tfm.gameObject.SetActive(isActive);
            }
        }

        public static void SetActive(GameObject gObj, bool isActive)
        {
            if (gObj != null && gObj.activeSelf != isActive)
            {
                gObj.SetActive(isActive);
            }
        }

        public static void SetActiveEx(this Transform tfm, bool isActive)
        {
            SetActive(tfm, isActive);
        }

        public static void SetActiveEx(this GameObject gObj, bool isActive)
        {
            SetActive(gObj, isActive);
        }

        public static void SetUIActive(Transform uiTrans, bool active, float scale = 1f)
        {
            if (uiTrans == null)
                return;

            if (active)
            {
                uiTrans.localScale = Vector3.one * scale;
            }
            else
            {
                uiTrans.localScale = Vector3.zero;
            }
        }

        public static void SetLayer(Transform trans, int layer, bool includeChildren = true)
        {
            if (trans == null) return;
            _SetLayer(trans, layer, includeChildren);
        }

        public static void SetLayer(GameObject obj, int layer, bool includeChildren = true)
        {
            if (obj == null) return;
            _SetLayer(obj.transform, layer, includeChildren);
        }

        private static void _SetLayer(Transform trans, int layer, bool includeChildren)
        {
            trans.gameObject.layer = layer;
            if (includeChildren)
            {
                for (int i = 0; i < trans.childCount; i++)
                {
                    _SetLayer(trans.GetChild(i), layer, true);
                }
            }
        }

        public static Matrix4x4 CalcMatrix(Transform root, Transform subNode)
        {
            var tmpNode = subNode;
            var mtx = Matrix4x4.TRS(tmpNode.localPosition, tmpNode.localRotation, tmpNode.localScale);
            while (tmpNode.parent != root)
            {
                tmpNode = tmpNode.parent;
                mtx = Matrix4x4.TRS(tmpNode.localPosition, tmpNode.localRotation, tmpNode.localScale) * mtx;
            }
            return mtx;
        }

        public static Color FromRGBString(string s)
        {
            return FromRGBInt(Convert.ToUInt32(s, 16));
        }

        public static Color FromRGBInt(uint val)
        {
            return new Color((val >> 16 & 0xff) / 255f, (val >> 8 & 0xff) / 255f, (val & 0xff) / 255f, 1);
        }

        public static Color FromRGB(short r, short g, short b)
        {
            return new Color(r / 255f, g / 255f, b / 255f, 1);
        }

        public static Color FromRGB(short r, short g, short b, short a)
        {
            return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
        }
    }
}