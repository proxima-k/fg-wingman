using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectUI : MonoBehaviour {
    [SerializeField] private List<LevelDataSO> _levelDataList;
    [SerializeField] private Transform _levelButtonPrefab;
    [SerializeField] private Transform _levelButtonsContainer;

    private void Start() {
        foreach (LevelDataSO levelData in _levelDataList) {
            if (levelData == null)
                continue;
            
            Transform newLevelButton = Instantiate(_levelButtonPrefab, _levelButtonsContainer);

            Button loadLevelButton = newLevelButton.GetChild(0).GetComponent<Button>();
            loadLevelButton.GetComponentInChildren<TextMeshProUGUI>().text = levelData.LevelName;
            loadLevelButton.onClick.AddListener(() => LevelClick(levelData));
            
            Button loadLeaderboardButton = newLevelButton.GetChild(1).GetComponent<Button>();
            loadLeaderboardButton.onClick.AddListener(() => LoadLeaderboard(levelData));
        }
    }
    
    private void LevelClick(LevelDataSO levelData)
    {
        LevelSelect.Instance.LoadLevel(levelData);
    }
    
    private void LoadLeaderboard(LevelDataSO levelData)
    {
        LevelSelect.Instance.LoadLeaderboard(levelData);
    }
}
