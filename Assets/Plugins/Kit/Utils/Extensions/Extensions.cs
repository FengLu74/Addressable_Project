using System;
using System.Collections.Generic;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Kit
{
    public static partial class Extensions
    {
        #region Unity Object

        public static bool IsNull(this UnityObject obj) => (object)obj == null;

        public static bool IsNotNull(this UnityObject obj) => (object)obj != null;

        public static void Reset(this Transform tran) {
            tran.localPosition = Vector3.zero;
            tran.localRotation = Quaternion.identity;
            tran.localScale = Vector3.one;
        }

        public static void SetParent(this GameObject go, Transform par, bool reset = true) {
            if (go == null) {
                return;
            }
            var goTransform = go.transform;
            goTransform.SetParent(par);
            if (reset) {
                goTransform.Reset();
            }
        }

        public static void SetParentEx(this UnityObject uo, Transform parent, bool setLastSibling = true)
        {
            Transform t;
            switch (uo)
            {
                case GameObject gameObject:
                    t = gameObject.transform;
                    break;
                case Component component:
                    t = component.transform;
                    break;
                default:
                    throw new NotSupportedException();
            }

            if (t == null)
            {
                return;
            }

            t.SetParent(parent);
            t.localPosition = Vector3.zero;
            t.localScale = Vector3.one;
            t.localRotation = Quaternion.identity;
            if (setLastSibling)
            {
                t.SetAsLastSibling();
            }
            else
            {
                t.SetAsFirstSibling();
            }
        }

        public static GameObject GameObject(this UnityObject uo) {
            switch (uo) {
            case GameObject gameObject:
                return gameObject;
            case Component component:
                return component.gameObject;
            default:
                return null;
            }
        }

        public static void SetActiveEx(this Component comp, bool active)
        {
            // ReSharper disable once InvertIf
            if (comp)
            {
                var go = comp.gameObject;
                if (go.activeSelf != active)
                {
                    go.SetActive(active);
                }
            }
        }

        public static void SetActiveEx(this GameObject go, bool active)
        {
            // ReSharper disable once InvertIf
            if (go)
            {
                if (go.activeSelf != active)
                {
                    go.SetActive(active);
                }
            }
        }

        public static T GetOrAddComponent<T>(this UnityObject uo) where T : Component =>
            uo.GetComponent<T>().AsUnityNull() ?? uo.AddComponent<T>();

        public static T AddComponent<T>(this UnityObject uo) where T : Component
        {
            switch (uo)
            {
                case GameObject gameObject:
                    return gameObject.AddComponent<T>();
                case Component component:
                    return component.gameObject.AddComponent<T>();
                default:
                    throw new NotSupportedException();
            }
        }

        public static T GetComponent<T>(this UnityObject uo) where T : Component
        {
            switch (uo)
            {
                case GameObject gameObject:
                    return gameObject.GetComponent<T>();
                case Component component:
                    return component.GetComponent<T>();
                default:
                    throw new NotSupportedException();
            }
        }

        public static T GetComponent<T>(this UnityObject uo, string name) where T : Component
        {
            Transform t;
            switch (uo)
            {
                case GameObject gameObject:
                    t = gameObject.transform;
                    break;
                case Component component:
                    t = component.transform;
                    break;
                default:
                    throw new NotSupportedException();
            }

            if (t == null)
            {
                return null;
            }

            var tr = t.Find(name);
            return tr == null ? null : tr.GetComponent<T>();
        }

        public static T AsUnityNull<T>(this T obj) where T : UnityObject =>
            // 伪空转成实空，允许合并操作符 destroyedObject.AsUnityNull() ?? otherObject
            obj == null ? null : obj;

        public static Material CloneMaterial(this Material material)
        {
            var newMaterial = UnityObject.Instantiate(material);
            newMaterial.name = $"{material.name}(Clone)";
            return newMaterial;
        }

        public static void ChangeLayer(this Transform trans, string targetLayer) {
            if (LayerMask.NameToLayer(targetLayer) == -1) {
                Debug.Log("Layer中不存在,请手动添加LayerName");
                return;
            }
            trans.gameObject.layer = LayerMask.NameToLayer(targetLayer);
            foreach (Transform child in trans) {
                ChangeLayer(child, targetLayer);
            }
        }

        #endregion

        #region GetComponentInChildren
        public static Component GetComponentInChildren(this UnityObject uo, Type type,
            bool includeInactive) {
            switch (uo) {
            case GameObject gameObject:
                return gameObject.GetComponentInChildren(type, includeInactive);
            case Component component:
                return component.GetComponentInChildren(type, includeInactive);
            default:
                throw new NotSupportedException();
            }
        }

        public static Component GetComponentInChildren(this UnityObject uo, Type type) {
            switch (uo) {
            case GameObject gameObject:
                return gameObject.GetComponentInChildren(type);
            case Component component:
                return component.GetComponentInChildren(type);
            default:
                throw new NotSupportedException();
            }
        }

        public static T GetComponentInChildren<T>(this UnityObject uo) where T : Component {
            switch (uo) {
            case GameObject gameObject:
                return gameObject.GetComponentInChildren<T>();
            case Component component:
                return component.GetComponentInChildren<T>();
            default:
                throw new NotSupportedException();
            }
        }

        public static T GetComponentInChildren<T>(this UnityObject uo, bool includeInactive)
            where T : Component {
            switch (uo) {
            case GameObject gameObject:
                return gameObject.GetComponentInChildren<T>(includeInactive);
            case Component component:
                return component.GetComponentInChildren<T>(includeInactive);
            default:
                throw new NotSupportedException();
            }
        }
        #endregion

        #region GetComponentInParent
        public static Component GetComponentInParent(this UnityObject uo, Type type) {
            switch (uo) {
            case GameObject gameObject:
                return gameObject.GetComponentInParent(type);
            case Component component:
                return component.GetComponentInParent(type);
            default:
                throw new NotSupportedException();
            }
        }

        public static T GetComponentInParent<T>(this UnityObject uo) where T : Component {
            switch (uo) {
            case GameObject gameObject:
                return gameObject.GetComponentInParent<T>();
            case Component component:
                return component.GetComponentInParent<T>();
            default:
                throw new NotSupportedException();
            }
        }
        #endregion

        #region Dictionary

        public static bool TryAddValue<T, TV>(this Dictionary<T, TV> dic, T key, TV value, bool overwrite = true)
        {
            if (dic.ContainsKey(key))
            {
                if (!overwrite)
                {
                    return false;
                }

                dic[key] = value;
                return true;
            }

            dic.Add(key, value);
            return true;
        }

        public static bool TryAddValueNotNull<T, TV>(this Dictionary<T, TV> dic, T key, TV value, bool overwrite = true)
        {
            if (key == null || value == null)
            {
                return false;
            }

            return TryAddValue(dic, key, value, overwrite);
        }

        public static bool TryAddValue<T, TV>(this SortedDictionary<T, TV> dic, T key, TV value, bool overwrite = true)
        {
            if (dic.ContainsKey(key))
            {
                if (!overwrite)
                {
                    return false;
                }

                dic[key] = value;
                return true;
            }

            dic.Add(key, value);
            return true;
        }

        public static void TryRemove<T, TV>(this Dictionary<T, TV> dic, T key)
        {
            if (dic.ContainsKey(key))
            {
                dic.Remove(key);
            }
        }

        #endregion

        #region List

        public static bool TryGet<T>(this List<T> list, Predicate<T> match, out T t)
        {
            t = list.Find(match);
            return t != null;
        }

        #endregion
    }
}
