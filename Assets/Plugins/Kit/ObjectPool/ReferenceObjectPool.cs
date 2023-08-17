using System;
using System.Collections.Generic;
namespace Kit {
    public static partial class ReferenceObjectPool {
        private static readonly Dictionary<Type, ReferenceObject> Pools = new Dictionary<Type, ReferenceObject>();
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        // ReSharper disable once MemberCanBePrivate.Global
        public static bool EnableStrictCheck { get; set; }
        // ReSharper disable once UnusedMember.Global
        public static void ClearAll() {
            lock (Pools)
            {
                foreach (var item in Pools)
                {
                    item.Value.RemoveAll();
                }

                Pools.Clear();
            }
        }
        private static ReferenceObject GetPool(Type referenceType)
        {
            if (referenceType == null)
            {
                throw new Exception("Type is illegal!!!");
            }

            ReferenceObject referenceCollection;
            lock (Pools) {
                if (Pools.TryGetValue(referenceType, out referenceCollection)) {
                    return referenceCollection;
                }
                referenceCollection = new ReferenceObject(referenceType);
                Pools.Add(referenceType, referenceCollection);
            }

            return referenceCollection;
        }
        public static T GetNew<T>() where T:class, IObjectPoolItem, new () => GetPool(typeof(T)).GetNew<T>();
        // ReSharper disable once UnusedMember.Global
        public static IObjectPoolItem GetNew(Type type) {
            CheckReferenceType(type);
            return GetPool(type).GetNew();
        }

        public static void GiveBack(IObjectPoolItem reference)
        {
            if (reference == null)
            {
                throw new Exception("Reference is null.");
            }

            var referenceType = reference.GetType();
            CheckReferenceType(referenceType);
            GetPool(referenceType).GiveBack(reference);
        }

        // ReSharper disable once UnusedMember.Global
        public static void Add<T>(int count)where T : class, IObjectPoolItem, new() => GetPool(typeof(T)).Add<T>(count);
        // ReSharper disable once UnusedMember.Global
        public static void Add(Type type, int count) {
            CheckReferenceType(type);
            GetPool(type).Add(count);
        }
        // ReSharper disable once UnusedMember.Global
        public static void Remove<T>(int count) where T : class, IObjectPoolItem => GetPool(typeof(T)).Remove(count);
        // ReSharper disable once UnusedMember.Global
        public static void Remove(Type type, int count) {
            CheckReferenceType(type);
            GetPool(type).Remove(count);
        }
        // ReSharper disable once UnusedMember.Global
        public static void RemoveAll<T>() where T : class, IObjectPoolItem => GetPool(typeof(T)).RemoveAll();

        // ReSharper disable once UnusedMember.Global
        public static void RemoveAll(Type referenceType)
        {
            CheckReferenceType(referenceType);
            GetPool(referenceType).RemoveAll();
        }

        private static void CheckReferenceType(Type referenceType)
        {
            if (!EnableStrictCheck)
            {
                return;
            }

            if (referenceType == null)
            {
                throw new Exception("type is illegal!");
            }

            if (!referenceType.IsClass || referenceType.IsAbstract)
            {
                throw new Exception("type is not a non-abstract class type.");
            }

            if (!typeof(IObjectPoolItem).IsAssignableFrom(referenceType))
            {
                throw new Exception($"type '{referenceType.FullName}' is illegal.");
            }
        }

    }
}
