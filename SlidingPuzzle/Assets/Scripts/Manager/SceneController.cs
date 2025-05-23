using Cysharp.Threading.Tasks;

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// scene load and release
/// </summary>
public class SceneController : MonoBehaviour
{
    private async UniTask LoadSceneAsync(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, mode);
        while (!asyncLoad.isDone)
            await UniTask.Yield();
    }

    public async Task LoadScene(string sceneName)
    {
        await LoadSceneAsync(sceneName);
    }

    public async Task ReloadCurrentScene()
    {
        await LoadSceneAsync(SceneManager.GetActiveScene().name);
    }
}
