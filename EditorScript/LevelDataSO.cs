using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/LevelData", order = 1)]
public class LevelDataSO : ScriptableObject {
    // id = file name
    public string ID => name;
    
    public string LevelName = "Level Name";
    public Sprite LevelImage;
    // public int LevelIndex = 0;

    public int MaxCrashes = 5;
    public float TimeToComplete = 45.0f;

    // list of checkpoints
    public CheckpointData StartCheckpointData;
    public CheckpointData EndCheckpointData;
    public List<CheckpointData> CheckpointDataList;
}

[Serializable]
public class CheckpointData
{
    public Vector3 Position;
    public Quaternion Rotation;
    public Vector3 Scale = Vector3.one;

    public GameObject CheckpointPrefab;
}