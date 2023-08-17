using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
namespace Kit {
    public class GeneralPool<T> : List<T> where T : new()
    {
        private readonly Func<T> _mCreator;
        private LinkedList<T> ActiveList { get; } = new LinkedList<T>();

        private GeneralPool(Func<T> creator) => _mCreator = creator;

        public GeneralPool()
            : this(CreateObject)
        {
        }

        private static T CreateObject() => new T();

        public bool FreeListEmpty => Count == 0;

        public int ActiveCount => ActiveList.Count;

        public T Request()
        {
            T obj;
            if (FreeListEmpty)
            {
                obj = _mCreator();
                ActiveList.AddLast(obj);
                return obj;
            }

            obj = this[Count - 1];
            RemoveAt(Count - 1);
            ActiveList.AddLast(obj);
            return obj;
        }

        public void Return(T obj)
        {
            if (obj == null) {
                return;
            }

            if (ActiveList.Contains(obj)) {
                ActiveList.Remove(obj);
            }

            if (!Contains(obj))
            {
                Add(obj);
            }

        }

        public void Destroy()
        {
            ActiveList.Clear();
            Clear();
        }
    }

    public class GameObjectPool<T> : List<T> where T : Object {
        private readonly LinkedList<T> _activeList = new LinkedList<T>();
        private readonly Func<T> _creator;
        private readonly T _prefab;
        private int _requestCount;

        public GameObjectPool(T prefab) {
            _creator = null;
            _prefab = prefab;
        }

        public GameObjectPool(Func<T> creator) {
            _creator = creator;
            _prefab = null;
        }

        public bool FreeListEmpty => Count == 0;

        public int ActiveCount => _activeList.Count;

        private T CreateObject() {
            T obj;
            if (_prefab) {
                obj = Object.Instantiate(_prefab);
                obj.name = _prefab.name + "_" + ++_requestCount;
            } else if (_creator != null) {
                obj = _creator();
            } else {
                Debug.LogError("Pool Request prefab == null && create == null");
                return null;
            }
            return obj;
        }

        public T Request() {
            T obj;
            if (FreeListEmpty) {
                obj = CreateObject();
                _activeList.AddLast(obj);
                return obj;
            }

            obj = this[Count - 1];
            RemoveAt(Count - 1);
            _activeList.AddLast(obj);
            return obj;
        }

        public T Request(Vector3 pos, Quaternion rot) {
            var obj = Request();
            try {
                var comp = obj as Component;
                if (comp != null) {
                    var transform = comp.transform;
                    transform.position = pos;
                    transform.rotation = rot;
                } else if (obj is GameObject go) {
                    go.transform.position = pos;
                    go.transform.rotation = rot;
                }
                _activeList.AddLast(obj);
            } catch (MissingReferenceException) {
                return null;
            }
            return obj;
        }

        public void Return(T obj) {
            _activeList.Remove(obj);

            if (!Contains(obj)) {
                Add(obj);
            }
        }

        public void RemoveObj(T obj) {
            _activeList.Remove(obj);
            Remove(obj);
        }

        public void Destroy() {
            foreach (var obj in _activeList) {
                Object.Destroy(obj);
            }

            _activeList.Clear();

            foreach (var obj in this) {
                Object.Destroy(obj);
            }

            Clear();
            _requestCount = 0;
        }

        public void DestroyImmediate() {
            foreach (var obj in _activeList) {
                Object.DestroyImmediate(obj);
            }

            _activeList.Clear();

            foreach (var obj in this) {
                Object.DestroyImmediate(obj);
            }

            Clear();
            _requestCount = 0;
        }
    }
}
