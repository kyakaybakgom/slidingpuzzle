using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

/// <summary>
/// addresable data load and release
/// </summary>
public class AddressableLoader : MonoBehaviour
{
    private Dictionary<string, AsyncOperationHandle> _loadedAssets = new Dictionary<string, AsyncOperationHandle>();

    /// <summary>
    /// 비동기 Addressable 에셋 로드
    /// </summary>
    public void LoadAssetAsync<T>(string key, Action<T> onLoaded) where T : UnityEngine.Object
    {
        if (_loadedAssets.ContainsKey(key))
        {
            // 이미 로드된 경우 캐시된 에셋 반환
            if (_loadedAssets[key].IsDone && _loadedAssets[key].Result is T result)
            {
                onLoaded?.Invoke(result);
                return;
            }
        }

        AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(key);
        _loadedAssets[key] = handle;

        handle.Completed += op =>
        {
            if (op.Status == AsyncOperationStatus.Succeeded)
            {
                onLoaded?.Invoke(op.Result);
            }
            else
            {
                Debug.LogError($"[AddressableLoader] Failed to load: {key}");
            }
        };
    }

    /// <summary>
    /// 특정 에셋 해제
    /// </summary>
    public void ReleaseAsset(string key)
    {
        if (_loadedAssets.TryGetValue(key, out AsyncOperationHandle handle))
        {
            Addressables.Release(handle);
            _loadedAssets.Remove(key);
        }
    }

    /// <summary>
    /// 모든 로드된 에셋 해제
    /// </summary>
    public void ReleaseAll()
    {
        foreach (var handle in _loadedAssets.Values)
        {
            Addressables.Release(handle);
        }
        _loadedAssets.Clear();
    }

    private void OnDestroy()
    {
        ReleaseAll();
    }
}
