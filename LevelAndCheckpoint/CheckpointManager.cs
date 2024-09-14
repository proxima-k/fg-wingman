using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckpointManager : MonoBehaviour
{
    public event EventHandler OnPlayerFinished;

    [SerializeField] private Checkpoint _startCheckpoint;
    [SerializeField] private Checkpoint _endCheckpoint;
    [SerializeField] private List<Checkpoint> _checkpoints;

    private int lapsRequired = 1;

    private int _currentLap = 0;
    private bool _started;
    private bool _finished;

    private int _currentCheckpointIndex = -1;

    private int _checkpointsLeft;
    private int _totalCheckpoints;

    private void Start()
    {
        _started = false;
        _finished = false;

        EnableCheckpoints();
    }

    private void OnTriggerEnter(Collider other)
    {
        Checkpoint collidedCheckpoint = other.GetComponentInParent<Checkpoint>();

        if (collidedCheckpoint == null)
            return;

        if (!_started)
        {
            if (collidedCheckpoint != _startCheckpoint)
                return;

            _started = true;
            ShowNextCheckpoints();
            return;
        }

        if (_finished) return;

        if (collidedCheckpoint == _endCheckpoint)
        {
            if (!AllCheckpointsVisited())
                return;

            _currentLap++;

            if (_currentLap < lapsRequired)
            {
                // Reenable everything for the next lap
                // EnableCheckpoints();
            }
            else
            {
                _finished = true;
                DisableAllCheckpoints();

                OnPlayerFinished?.Invoke(this, EventArgs.Empty);
            }

            return;
        }

        if (collidedCheckpoint == GetNextCheckpoint())
        {
            _currentCheckpointIndex++;
            _checkpointsLeft++; // Increment checkpointsLeft
            ShowNextCheckpoints();
        }
    }

    public void ShowNextCheckpoints(int amountToShow = 2) {
        // based on the current checkpoint index
        // show the next two checkpoints
        // and set the next checkpoint to highlight
        
        if (_currentCheckpointIndex == -1) {
            // since player will be spawned at the start checkpoint
            // don't show the start checkpoint
            // show the next two checkpoints if there's any
            for (int i = 0; i < amountToShow; i++) {
                // if there's enough checkpoints
                if (_checkpoints.Count > i) {
                    _checkpoints[i].Show();
                    
                    if (i == 0) {
                        _checkpoints[i].SetType(Checkpoint.CheckpointType.Highlight);
                    }
                }
            }
        }
        else if (_currentCheckpointIndex == _checkpoints.Count - 1) {
            // if the player is at the last checkpoint
            // show the end checkpoint
            _endCheckpoint.Show();
        }
        else {
            // hide current checkpoint
            // show the next amount of checkpoints
            // set the next checkpoint to highlight
            _checkpoints[_currentCheckpointIndex].Hide();
            
            for (int i = 0; i < amountToShow; i++) {
                // if there's enough checkpoints
                if (_checkpoints.Count > _currentCheckpointIndex + i + 1) {
                    _checkpoints[_currentCheckpointIndex + i + 1].Show();
                    
                    if (i == 0) {
                        _checkpoints[_currentCheckpointIndex + i + 1].SetType(Checkpoint.CheckpointType.Highlight);
                    }
                }
                else {
                    // if there's no more checkpoints
                    // show the end checkpoint
                    _endCheckpoint.Show();
                }
            }
        }
    }

    private bool AllCheckpointsVisited()
    {
        return _currentCheckpointIndex == _checkpoints.Count - 1;
    }

    private void EnableCheckpoints()
    {
        _startCheckpoint.gameObject.SetActive(true);
        _endCheckpoint.gameObject.SetActive(true);

        foreach (var checkpoint in _checkpoints)
        {
            checkpoint.gameObject.SetActive(true);
        }

        _totalCheckpoints = _checkpoints.Count;
        _checkpointsLeft = 0; // Initialize checkpointsLeft to 0
    }

    private void DisableAllCheckpoints()
    {
        _startCheckpoint.gameObject.SetActive(false);
        _endCheckpoint.gameObject.SetActive(false);

        foreach (var checkpoint in _checkpoints)
        {
            checkpoint.Hide();
        }
    }

    public void SetupCheckpoints(List<Checkpoint> checkpointDataList, Checkpoint startCheckpointData, Checkpoint endCheckpointData)
    {
        _checkpoints = checkpointDataList;
        _startCheckpoint = startCheckpointData;
        _endCheckpoint = endCheckpointData;

        _startCheckpoint.Hide();
        _endCheckpoint.SetType(Checkpoint.CheckpointType.End);
        _endCheckpoint.Hide();

        foreach (var checkpoint in _checkpoints)
        {
            checkpoint.SetType(Checkpoint.CheckpointType.Default);
            checkpoint.Hide();
        }

        _totalCheckpoints = _checkpoints.Count;
        _checkpointsLeft = _totalCheckpoints;
    }

    private Checkpoint GetNextCheckpoint()
    {
        if (_currentCheckpointIndex == _checkpoints.Count - 1)
            return _endCheckpoint;

        return _checkpoints[_currentCheckpointIndex + 1];
    }

    public Checkpoint GetCurrentCheckpoint()
    {
        if (_currentCheckpointIndex == -1)
            return _startCheckpoint;

        return _checkpoints[_currentCheckpointIndex];
    }

    public Checkpoint GetStartCheckpoint()
    {
        return _startCheckpoint;
    }

    public int GetCheckpointsLeft()
    {
        return _checkpointsLeft;
    }

    public int GetTotalCheckpoints()
    {
        return _totalCheckpoints;
    }
}
