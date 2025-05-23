using UnityEngine;

[CreateAssetMenu(menuName = "Configs/GameConfig", fileName = "GameConfig")]
public class GameConfig : ScriptableObject
{
    public string startSceneName = "MainMenu";
    public string playerPrefabKey = "PlayerPrefab";
}
