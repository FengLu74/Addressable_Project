using System;
using System.Collections.Generic;
using System.Reflection;
namespace Kit {
    public static class ReflectionUtils {
        private static readonly Assembly[] LoadedAssemblies =
            AppDomain.CurrentDomain.GetAssemblies();

        private static bool IsCustomAssembly(Assembly assembly) {
            var bannedPrefixes = new[] {
                // ReSharper disable StringLiteralTypo
                "System", "Unity", "Microsoft", "Mono.", "mscorlib", "NSubstitute", "JetBrains",
                "nunit.", "QFSW",
                "com.unity", "Mirror", "OSA", "QHierarchy", "Sirenix", "spine"
                // ReSharper restore StringLiteralTypo
            };
            var bannedAssemblies = new[] {
                "mcs", "AssetStoreTools", "FlatBuffers", "kcp2k", "LitJson", "Cinemachine",
            };
            var assemblyFullName = assembly.FullName;
            foreach (var prefix in bannedPrefixes) {
                if (assemblyFullName.StartsWith(prefix)) {
                    return false;
                }
            }
            var assemblyShortName = assembly.GetName().Name;
            foreach (var name in bannedAssemblies) {
                if (assemblyShortName == name) {
                    return false;
                }
            }
            return true;
        }

        public static IEnumerable<Assembly> GetAllCustomAssemblies() {
            var list = new List<Assembly>();
            foreach (var loadedAssembly in LoadedAssemblies) {
                if (IsCustomAssembly(loadedAssembly)) {
                    list.Add(loadedAssembly);
                }
            }
            return list;
        }

        public static IEnumerable<Type> GetAllTypeOfAttribute<T>() where T : Attribute {
            var result = new List<Type>();
            var assemblies = GetAllCustomAssemblies();
            foreach (var assembly in assemblies) {
                var types = assembly.GetTypes();
                foreach (var type in types) {
                    if (type.GetCustomAttribute<T>() != null) {
                        result.Add(type);
                    }
                }
            }
            return result;
        }

        public static IEnumerable<Type> GetAllTypeOfAttribute<T>(Assembly assembly)
            where T : Attribute {
            var result = new List<Type>();
            var types = assembly.GetTypes();
            foreach (var type in types) {
                if (type.GetCustomAttribute<T>() != null) {
                    result.Add(type);
                }
            }
            return result;
        }

        public static Type GetTypeByName(string fullName) {
            var assemblies = GetAllCustomAssemblies();
            foreach (var assembly in assemblies) {
                var types = assembly.GetTypes();
                foreach (var type in types) {
                    if (type.FullName == fullName) {
                        return type;
                    }
                }
            }
            return null;
        }
    }
}
