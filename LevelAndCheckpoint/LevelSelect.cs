using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelect : MonoBehaviour
{
    public static LevelSelect Instance;
    
    public LevelDataSO LevelToLoad => levelToLoad;
    [SerializeField] private LevelDataSO levelToLoad;
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else {
            Destroy(gameObject);
            return;
        }
        
        DontDestroyOnLoad(gameObject);
    }
    
    public void LoadLevel(LevelDataSO levelData)
    {
        levelToLoad = levelData;
        SceneLoader.Load(SceneLoader.Scene.GameScene);
    }

    public void LoadLeaderboard(LevelDataSO levelData) {
        levelToLoad = levelData;
        SceneLoader.Load(SceneLoader.Scene.LeaderboardScene);
    }
}
