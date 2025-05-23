using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameConfig config;

    private void Start()
    {
        Debug.Log($"[GameManager] Loaded config: {config.startSceneName}");
        //SceneController.Instance.LoadScene(config.startSceneName);

        //AddressableLoader.Instance.LoadAsset<GameObject>(config.playerPrefabKey, prefab =>
        //{
        //    Instantiate(prefab);
        //});
    }

    public void RestartGame()
    {
        //AddressableLoader.Instance.ReleaseAll();
        //SceneController.Instance.ReloadCurrentScene();
    }
}
