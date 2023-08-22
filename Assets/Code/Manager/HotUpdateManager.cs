using Kit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class HotUpdateManager : SingletonMono<HotUpdateManager>
{
    public enum HotState
    {
        None,
        Version,
        Hash,
        Json,
        Download,
        StopDownload,
    }
    private HotState m_HotState = HotState.None;
    [SerializeField]
    public bool Hot = true;
    private static int retryMax = 3;
    private int m_curRetry = 0;
    private readonly List<object> _updateKeys = new List<object>();
    private void Start()
    {
        
    }

    public IEnumerator Launch()
    {
        if (!Hot)
        {
            EnterGame();
            yield return 0;
        }
        else
        {
            yield return StartCoroutine(DoUpdateAddressadble());
        }
    }

    private IEnumerator DoUpdateAddressadble()
    {
        m_HotState = HotState.Hash;
        if(Application.internetReachability== NetworkReachability.NotReachable)
        {
            Debug.LogWarning("not net!");
            yield break;
        }

        AsyncOperationHandle<IResourceLocator> initHandle = Addressables.InitializeAsync();
        yield return initHandle;
        AsyncOperationHandle<List<string>> checkHandle = Addressables.CheckForCatalogUpdates(false);
        yield return checkHandle;
        if(checkHandle.Status!=AsyncOperationStatus.Succeeded)
        {
            Debug.LogWarning("版本失败");
            yield break;
        }
        if(checkHandle.Result.Count>0)
        {
            m_HotState = HotState.Json;
            AsyncOperationHandle<List<IResourceLocator>> updateHandle = Addressables.UpdateCatalogs(checkHandle.Result, false);
            yield return updateHandle;
            if(updateHandle.Status!= AsyncOperationStatus.Succeeded)
            {
                Debug.LogError("update 失败");
                yield break;
            }
            List<IResourceLocator> locators = updateHandle.Result;
            foreach(var locator in locators)
            {
                _updateKeys.AddRange(locator.Keys);
            }
            Addressables.Release(checkHandle);
            Addressables.Release(updateHandle);
        }
        else
        {
            IEnumerable<IResourceLocator> locators = Addressables.ResourceLocators;
            foreach (var locator in locators)
            {
                _updateKeys.AddRange(locator.Keys);
            }
        }
        AsyncOperationHandle<long> sizeHandle = Addressables.GetDownloadSizeAsync(_updateKeys as IEnumerable<object>);
        yield return sizeHandle;
        Debug.Log("热更新大小:" + sizeHandle.Result);
        if(sizeHandle.Result>0)
        {
            StartCoroutine(DownLoad());
        }
    }

    private IEnumerator DownLoad()
    {
        m_HotState = HotState.Download;
        AsyncOperationHandle downHandle = Addressables.DownloadDependenciesAsync(_updateKeys ,Addressables.MergeMode.Union);
        while(!downHandle.IsDone)
        {
            float _bar = downHandle.GetDownloadStatus().Percent;
            Debug.Log("download: " + _bar);
            yield return null;
        }

        yield return downHandle;
        if(downHandle.Status!=AsyncOperationStatus.Succeeded)
        {
            Debug.LogError("下载失败,尝试重新下载");
            if (m_curRetry<retryMax)
            {
                m_curRetry++;
                StartCoroutine(DownLoad());
                Addressables.Release(downHandle);
                yield break;
            }
        }
        else
        {
            EnterGame();
        }
    }

    public void EnterGame()
    {
        Debug.Log("EnterGame!");
    }
}
