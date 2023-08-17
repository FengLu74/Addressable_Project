using UnityEngine;

namespace Kit
{
    public class Singleton<T> where T : class, new()
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }

                _instance = new T();
                (_instance as Singleton<T>)?.InitSingleton();
                return _instance;
            }
        }

        protected virtual void InitSingleton()
        {
        }
    }

    public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }

                _instance = (T)FindObjectOfType(typeof(T));
                if (_instance != null)
                {
                    (_instance as SingletonMono<T>)?.InitSingleton();
                    return _instance;
                }

                var go = new GameObject();
                _instance = go.AddComponent<T>();
                (_instance as SingletonMono<T>)?.InitSingleton();
                go.name = $"{typeof(T)} (Singleton)";
                DontDestroyOnLoad(go);
                return _instance;
            }
        }

        protected virtual void InitSingleton()
        {
        }
    }
}
