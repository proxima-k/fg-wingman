using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelDataEditor : EditorWindow {

    private SerializedObject _serializedObject;
    
    // generate level data
    private SerializedProperty _startCheckpointProperty;
    private SerializedProperty _endCheckpointProperty;
    private SerializedProperty _checkpointListProperty;
    private SerializedProperty _fileToGenerateNameProperty;
    [SerializeField] private Checkpoint _startCheckpoint;
    [SerializeField] private Checkpoint _endCheckpoint;
    [SerializeField] private List<Checkpoint> _checkpoints;
    [SerializeField] private string _fileToGenerateName = "LevelData";
    
    // load level data
    private SerializedProperty _levelDataSOToLoadProperty;
    [SerializeField] private LevelDataSO _levelDataSOToLoad;
    
    private float _windowPadding = 10f;


    [MenuItem("Tools/Level Data Editor")]
    public static void ShowEditor() {
        EditorWindow window = GetWindow<LevelDataEditor>();
        window.titleContent = new GUIContent("Level Data Editor");
    }

    private void OnEnable() {
        _serializedObject = new SerializedObject(this);
        
        _startCheckpointProperty = _serializedObject.FindProperty("_startCheckpoint");
        _endCheckpointProperty = _serializedObject.FindProperty("_endCheckpoint");
        _checkpointListProperty = _serializedObject.FindProperty("_checkpoints");
        _fileToGenerateNameProperty = _serializedObject.FindProperty("_fileToGenerateName");

        _levelDataSOToLoadProperty = _serializedObject.FindProperty("_levelDataSOToLoad");
    }
    
    // called when the window is opened, creates elements for the window and will persist through the lifetime of the window
    public void CreateGUI() {
        VisualElement root = rootVisualElement;
        
        // add a container to the root for padding
        root.style.paddingLeft = _windowPadding;
        root.style.paddingRight = _windowPadding;
        root.style.paddingTop = _windowPadding;
        root.style.paddingBottom = _windowPadding;
        
        CreateGenerateLevelDataGUI();
        
        // add a space between the two sections
        var space = new VisualElement();
        space.style.height = 20f;
        root.Add(space);
        
        // add a line between the two sections
        var line = new VisualElement();
        line.style.height = 1f;
        line.style.backgroundColor = Color.gray;
        root.Add(line);
        
        space = new VisualElement();
        space.style.height = 20f;
        root.Add(space);
        
        CreateLoadLevelDataGUI();
    }
    
    // to ensure all properties are updated
    private void OnGUI() {
        _serializedObject.Update();
        
        // Generate Level Data =============================================================
        Button generateButton = rootVisualElement.Q<Button>("Generate Level Data Button");


        bool canGenerateLevelData = _startCheckpoint != null &&
                                    _endCheckpoint != null &&
                                    IsCheckpointListValid() &&
                                    !string.IsNullOrEmpty(_fileToGenerateName);
        
        generateButton.SetEnabled(canGenerateLevelData);
        
        
        // Load Level Data ==================================================================
        Button loadButton = rootVisualElement.Q<Button>("Load Level Data Button");
        
        bool canLoadLevelData = _levelDataSOToLoad != null;
        
        loadButton.SetEnabled(canLoadLevelData);
        
        _serializedObject.ApplyModifiedProperties();
    }
    
    private void CreateGenerateLevelDataGUI() {
        VisualElement root = rootVisualElement;
        
        // add a label
        Label label = new Label("Generate Level Data");
        label.style.unityFontStyleAndWeight = FontStyle.Bold;
        root.Add(label);
        
        AddPropertyFieldToRoot(_startCheckpointProperty, root);
        AddPropertyFieldToRoot(_endCheckpointProperty, root);
        AddPropertyFieldToRoot(_checkpointListProperty, root);
        AddPropertyFieldToRoot(_fileToGenerateNameProperty, root);
        
        Button button = new Button();
        button.name = "Generate Level Data Button";
        button.text = "Generate Level Data";
        root.Add(button);
        
        button.clicked += GenerateLevelData;
    }
    
    private void CreateLoadLevelDataGUI() {
        VisualElement root = rootVisualElement;
        
        // add a label
        Label label = new Label("Load Level Data");
        label.style.unityFontStyleAndWeight = FontStyle.Bold;
        root.Add(label);
        
        AddPropertyFieldToRoot(_levelDataSOToLoadProperty, root);

        Button button = new Button();
        button.name = "Load Level Data Button";
        button.text = "Load Level Data";
        root.Add(button);
        
        button.clicked += LoadLevelData;
    }


    private void AddPropertyFieldToRoot(SerializedProperty property, VisualElement root) {
        PropertyField propertyField = new PropertyField(property);
        propertyField.Bind(_serializedObject);
        root.Add(propertyField);
    }
    
    private void GenerateLevelData() {
        LevelDataSO levelData = ScriptableObject.CreateInstance<LevelDataSO>();
        
        // create start checkpoint
        CheckpointData startCheckpointData = CreateCheckpointData(_startCheckpoint);
        levelData.StartCheckpointData = startCheckpointData;
        
        // create end checkpoint
        CheckpointData endCheckpointData = CreateCheckpointData(_endCheckpoint);
        levelData.EndCheckpointData = endCheckpointData;
        
        // create list of checkpoints
        levelData.CheckpointDataList = new List<CheckpointData>();
        foreach (var checkpoint in _checkpoints) {
            CheckpointData checkpointData = CreateCheckpointData(checkpoint);
            levelData.CheckpointDataList.Add(checkpointData);
        }
        
        if (!AssetDatabase.IsValidFolder("Assets/Data")) {
            AssetDatabase.CreateFolder("Assets", "Data");
        }
        
        string fileName = _fileToGenerateName;
        // check if file name exists, if it does, add a number to the end of the file name
        if (AssetDatabase.FindAssets(fileName).Length > 0) {
            Debug.Log("Found existing file name" + fileName);
            int i = 1;
            while (AssetDatabase.FindAssets(fileName + i).Length > 0) {
                i++;
            }
            
            fileName += i;
        }
        
        AssetDatabase.CreateAsset(levelData, "Assets/Data/" + fileName + ".asset");
        AssetDatabase.SaveAssets();
        
        // highlight the asset in the project window
        EditorGUIUtility.PingObject(levelData);
        
        Debug.Log("Generated Level Data!");
    }

    private void LoadLevelData() {
        GameObject parent = new GameObject($"{_levelDataSOToLoad.name} CHECKPOINTS -----");
        
        // load the level data
        LevelDataSO levelData = _levelDataSOToLoad;
        
        // instantiate the checkpoints
        Checkpoint startCheckpoint = CreateCheckpoint(levelData.StartCheckpointData);
        startCheckpoint.name = "Start Checkpoint";
        startCheckpoint.transform.SetParent(parent.transform, true);
        
        Checkpoint endCheckpoint = CreateCheckpoint(levelData.EndCheckpointData);
        endCheckpoint.name = "End Checkpoint";
        endCheckpoint.transform.SetParent(parent.transform, true);
        
        for (int i = 0; i < levelData.CheckpointDataList.Count; i++) {
            Checkpoint checkpoint = CreateCheckpoint(levelData.CheckpointDataList[i]);
            checkpoint.name = "Checkpoint " + i;
            checkpoint.transform.SetParent(parent.transform, true);
        }
        
        Debug.Log("Load Level Data");
    }
    
    private bool IsCheckpointListValid() {
        if (_checkpoints == null) {
            return false;
        }
        
        if (_checkpoints.Count == 0) {
            return false;
        }
        
        if (_checkpoints.Contains(null)) {
            return false;
        }
        
        // check if all the checkpoints are an instance of a prefab
        for (int i = 0; i < _checkpoints.Count; i++) {
            if (!IsCheckpointAPrefabInstance(_checkpoints[i])) {
                return false;
            }
        }
        
        if (!IsCheckpointAPrefabInstance(_startCheckpoint)) {
            return false;
        }
        
        if (!IsCheckpointAPrefabInstance(_endCheckpoint)) {
            return false;
        }
        
        return true;
    } 
    
    private CheckpointData CreateCheckpointData(Checkpoint checkpoint) {
        CheckpointData checkpointData = new CheckpointData();
        checkpointData.Position = checkpoint.transform.position;
        checkpointData.Rotation = checkpoint.transform.rotation;
        checkpointData.Scale = checkpoint.transform.localScale;
        
        GameObject checkpointInstance = checkpoint.gameObject;
        PrefabInstanceStatus status = PrefabUtility.GetPrefabInstanceStatus(checkpointInstance);
        if (status == PrefabInstanceStatus.Connected) {
            checkpointData.CheckpointPrefab = PrefabUtility.GetCorrespondingObjectFromSource(checkpointInstance);
        } else {
            checkpointData.CheckpointPrefab = null;
        }
        
        return checkpointData;
    }
    
    private bool IsCheckpointAPrefabInstance(Checkpoint checkpoint) {
        GameObject checkpointInstance = checkpoint.gameObject;
        PrefabInstanceStatus status = PrefabUtility.GetPrefabInstanceStatus(checkpointInstance);
        return status == PrefabInstanceStatus.Connected;
    }
    
    private Checkpoint CreateCheckpoint(CheckpointData checkpointData) {
        if (checkpointData.CheckpointPrefab == null) {
            Debug.LogError("Checkpoint prefab is null. Make sure you have a prefab asset assigned to your LevelDataSO");
            return null;
        }
        if (checkpointData.CheckpointPrefab.GetComponent<Checkpoint>() == null) {
            Debug.LogError("Checkpoint prefab does not have a Checkpoint component");
            return null;
        }
        
        GameObject checkpointPrefab = checkpointData.CheckpointPrefab;
        
        GameObject checkpoint = PrefabUtility.InstantiatePrefab(checkpointPrefab) as GameObject;
        
        checkpoint.transform.position = checkpointData.Position;
        checkpoint.transform.rotation = checkpointData.Rotation;
        checkpoint.transform.localScale = checkpointData.Scale;
        
        return checkpoint.GetComponent<Checkpoint>();
    }
}
