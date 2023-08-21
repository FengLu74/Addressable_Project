using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Kit;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using Object = UnityEngine.Object;

    public class AddressableManager : Singleton<AddressableManager>
    {
        //加载中的任务
        private readonly Dictionary<uint, AsyncOperationHandle> _loadingTaskMap = new Dictionary<uint, AsyncOperationHandle>();

        private uint _counter;

        private uint GetCounter()
        {
            ++_counter;
            if (_counter == uint.MaxValue)
            {
                _counter = 1;
            }

            return _loadingTaskMap.ContainsKey(_counter) ? GetCounter() : _counter;
        }

        public static async UniTaskVoid InitAsync()
        {
            var handle = Addressables.InitializeAsync();
            await handle;
        }

        #region Release

        /// 取消加载中的任务
        public void ReleaseTask(uint taskId)
        {
            if (!_loadingTaskMap.TryGetValue(taskId, out var asset))
            {
                return;
            }

            Addressables.Release(asset);
            _loadingTaskMap.Remove(taskId);
        }

        public static void Release<T>(T asset) where T : Object => Addressables.Release(asset);

        public static bool ReleaseGameObject(GameObject instance) => Addressables.ReleaseInstance(instance);

        public static void ReleaseScene(SceneInstance scene,Action callback) => UnLoadSceneAsync(scene,callback);

        #endregion

        #region Task

        public uint LoadAssetAsync<T>(string assetName, Action<T> callback) where T : Object
        {
            if (string.IsNullOrEmpty(assetName))
            {
                callback?.Invoke(null);
                return 0u;
            }

            var task = 0u;
            LoadAsync(assetName, index => task = index, callback);
            return task;
        }

        public uint LoadAssetLabelAsync<T>(string assetLabel, Action<T> callback) where T : Object {
            if (string.IsNullOrEmpty(assetLabel)) {
                callback?.Invoke(null);
                return 0u;
            }

            var task = 0u;
            AssetLabelReference assetLabelReference = new AssetLabelReference();
            assetLabelReference.labelString = assetLabel;
            Addressables.LoadAssetsAsync<T>(assetLabelReference, (res) => {
                //Log.LogInfo("res " + res.name);
            });
            return task;
        }

        public static T LoadAssetSync<T>(string assetName) where T : Object =>
            string.IsNullOrEmpty(assetName) ? null : LoadSync<T>(assetName);

        public uint LoadGameObjectAsync(string goName, Action<GameObject> callback, Transform parent = null)
        {
            if (string.IsNullOrEmpty(goName))
            {
                callback?.Invoke(null);
                return 0;
            }

            var task = 0u;
            InstantiateAsync(goName, parent, index => task = index, callback);
            return task;
        }

        public static GameObject LoadGameObjectSync(string goName, Transform parent = null) =>
            string.IsNullOrEmpty(goName) ? null : InstantiateSync(goName, parent);

        public static async UniTaskVoid LoadScene(string sceneName, Action<float> perCallBack,
            Action<SceneInstance> callback) =>
            await LoadSceneAsync(sceneName, perCallBack, callback);

        /// <summary>
        /// 同步加载音效
        /// </summary>
        /// <param name="audioPath"></param>
        /// <returns></returns>
        public static AudioClip LoadAudioClipSync(string audioPath) {
            return LoadAssetSync<AudioClip>(audioPath);
        }
        #endregion

        #region Addressable Load

        private async void LoadAsync<T>(string asset, Action<uint> beginCall, Action<T> callback = null)
            where T : Object
        {
            var handle = Addressables.LoadAssetAsync<T>(asset);
            var index = GetCounter();
            _loadingTaskMap.Add(index, handle);
            beginCall?.Invoke(index);
            await handle.Task;
            // ReSharper disable once InvertIf
            if (_loadingTaskMap.ContainsKey(index))
            {
                _loadingTaskMap.Remove(index);
                callback?.Invoke(handle.Result);
                if (handle.Result.IsNull())
                {
                    Addressables.Release(handle);
                }
            }
        }

        private static T LoadSync<T>(string asset) where T : Object
        {
            var handle = Addressables.LoadAssetAsync<T>(asset);
            var result = handle.WaitForCompletion();
            if (result.IsNotNull())
            {
                return result;
            }

            Addressables.Release(handle);
            return null;
        }

        private async void InstantiateAsync(string asset, Transform parent, Action<uint> beginCall,
            Action<GameObject> callback = null)
        {
            var handle = parent.IsNull()
                ? Addressables.InstantiateAsync(asset, Vector3.one * -10000f, Quaternion.identity)
                : Addressables.InstantiateAsync(asset, parent);
            var index = GetCounter();
            _loadingTaskMap.Add(index, handle);
            beginCall?.Invoke(index);
            await handle.Task;
            // ReSharper disable once InvertIf
            if (_loadingTaskMap.ContainsKey(index))
            {
                _loadingTaskMap.Remove(index);
                callback?.Invoke(handle.Result);
                if (handle.Result.IsNull())
                {
                    Addressables.Release(handle);
                }
            }
        }

        private static GameObject InstantiateSync(string asset, Transform parent)
        {
            var handle = parent.IsNull()
                ? Addressables.InstantiateAsync(asset, Vector3.one * -10000f, Quaternion.identity)
                : Addressables.InstantiateAsync(asset, parent);
            var result = handle.WaitForCompletion();
            if (result.IsNotNull())
            {
                return result;
            }

            Addressables.Release(handle);


            return null;
        }

        private static async UniTask LoadSceneAsync(string asset, Action<float> perCallBack = null,
            Action<SceneInstance> callback = null)
        {
            var handle = Addressables.LoadSceneAsync(asset);
            while (!handle.IsDone)
            {
                perCallBack?.Invoke(handle.PercentComplete);
                await UniTask.Yield();
            }

            callback?.Invoke(handle.Result);
        }

        private static async void UnLoadSceneAsync(SceneInstance scene, Action callback)
        {
            var handle = Addressables.UnloadSceneAsync(scene);
            await handle.Task;
            callback?.Invoke();
        }

        #endregion
    }

