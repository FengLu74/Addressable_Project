using System;
using System.Collections.Generic;

namespace Kit {
    public static partial class ReferenceObjectPool {
        private sealed class ReferenceObject {
            private readonly Queue<IObjectPoolItem> _references;
            public ReferenceObject(Type referenceType)
            {
                _references = new Queue<IObjectPoolItem>(16);
                ReferenceType = referenceType;
            }

            // ReSharper disable once MemberCanBePrivate.Local
            public Type ReferenceType { get; }

            private static T NewInstance<T>() where T : new() => new T();
            private static IObjectPoolItem NewInstance(Type type) => (IObjectPoolItem) Activator.CreateInstance(type);
            /// <summary>
            /// 从池中获取新的类
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <returns></returns>
            /// <exception cref="Exception"></exception>
            // ReSharper disable once MemberHidesStaticFromOuterClass
            public T GetNew<T>() where T : class, IObjectPoolItem, new()
            {
                if (typeof(T) != ReferenceType)
                {
                    throw new Exception("Type is illegal!!!");
                }
                lock (_references) {
                    if (_references.Count > 0) {
                        return (T)_references.Dequeue();
                    }
                }
                return NewInstance<T>();
            }

            public IObjectPoolItem GetNew() {
                lock (_references) {
                    if (_references.Count > 0) {
                        return _references.Dequeue();
                    }
                }
                return NewInstance(ReferenceType);
            }

            // ReSharper disable once MemberHidesStaticFromOuterClass
            public void GiveBack(IObjectPoolItem reference) {
                reference.CleanUp();
                lock (_references) {
                    if (EnableStrictCheck && _references.Contains(reference)) {
                        return;
                    }
                    _references.Enqueue(reference);
                }
            }

            // ReSharper disable once MemberHidesStaticFromOuterClass
            public void Add<T>(int count) where T : class, IObjectPoolItem, new() {
                if (typeof(T) != ReferenceType)
                {
                    throw new Exception("Type is illegal!!!");
                }
                lock (_references) {
                    while (count-- > 0) {
                        _references.Enqueue(NewInstance<T>());
                    }
                }
            }

            public void Add(int count) {
                lock (_references) {
                    while (count-- > 0) {
                        _references.Enqueue(NewInstance(ReferenceType));
                    }
                }
            }
            public void RemoveAll() {
                lock (_references)
                {
                    _references.Clear();
                }
            }
            public void Remove(int count) {
                lock (_references) {
                    if (count > _references.Count) {
                        RemoveAll();
                    } else {
                        while (count-- > 0) {
                            _references.Dequeue();
                        }
                    }
                }
            }

        }
    }

}
