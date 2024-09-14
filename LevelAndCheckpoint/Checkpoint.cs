using System;
using UnityEngine;

[Serializable]
public class Checkpoint : MonoBehaviour
{
    [SerializeField] private Material _defaultMaterial;
    [SerializeField] private Material _highlightMaterial;
    [SerializeField] private Material _goalMaterial;
    
    [SerializeField] private LODGroup _lodGroup;
    // index
    
    public enum CheckpointType
    {
        Start,
        Default,
        Highlight,
        End
    }
    
    public CheckpointType CurrentType => _currentType;
    private CheckpointType _currentType = CheckpointType.Default;
    
    public void SetType(CheckpointType type)
    {
        _currentType = type;
        
        switch (type)
        {
            case CheckpointType.Start:
                UpdateMaterial(_defaultMaterial);
                break;
            case CheckpointType.Default:
                UpdateMaterial(_defaultMaterial);
                break;
            case CheckpointType.Highlight:
                UpdateMaterial(_highlightMaterial);
                break;
            case CheckpointType.End: 
                UpdateMaterial(_goalMaterial);
                break;
        }
    }
    
    public void Show() {
        _lodGroup.transform.gameObject.SetActive(true);
    }
    
    public void Hide() {
        _lodGroup.transform.gameObject.SetActive(false);
    }

    private void UpdateMaterial(Material targetMaterial) {
        foreach (LOD lod in _lodGroup.GetLODs()) {
            foreach (Renderer lodRenderer in lod.renderers) {
                lodRenderer.sharedMaterial = targetMaterial;
            }
        }
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 5f);
    }
#endif
}
