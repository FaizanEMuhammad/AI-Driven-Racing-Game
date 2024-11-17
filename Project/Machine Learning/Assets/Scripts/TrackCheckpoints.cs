using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class TrackCheckpoints : MonoBehaviour
{
    public event EventHandler<CheckpointEventArgs> OnPlayerCorrectCheckpoint;
    public event EventHandler<CheckpointEventArgs> OnPlayerWrongCheckpoint;

    [SerializeField] private List<Transform> carTransformList;

    private List<CheckpointSingle> checkpointSingleList;
    private List<int> nextCheckpointSingleIndexList;

    public class CheckpointEventArgs : EventArgs
    {
        public Transform carTransform;
    }

    private void Awake()
    {
        Transform checkpointsTransform = transform.Find("Checkpoints");

        checkpointSingleList = new List<CheckpointSingle>();
        foreach (Transform checkpointSingleTransform in checkpointsTransform)
        {
            CheckpointSingle checkpointSingle = checkpointSingleTransform.GetComponent<CheckpointSingle>();

            checkpointSingle.SetTrackCheckpoints(this);

            checkpointSingleList.Add(checkpointSingle);
        }

        nextCheckpointSingleIndexList = new List<int>();
        foreach (Transform carTransform in carTransformList)
        {
            nextCheckpointSingleIndexList.Add(0);
        }
    }

    public void CarThroughCheckpoint(CheckpointSingle checkpointSingle, Transform carTransform)
    {
        int nextCheckpointSingleIndex = nextCheckpointSingleIndexList[carTransformList.IndexOf(carTransform)];
        if (checkpointSingleList.IndexOf(checkpointSingle) == nextCheckpointSingleIndex)
        {
            // Correct checkpoint
             Debug.Log("Correct checkpoint reached by " + carTransform.name);;
            CheckpointSingle correctCheckpointSingle = checkpointSingleList[nextCheckpointSingleIndex];


            nextCheckpointSingleIndexList[carTransformList.IndexOf(carTransform)]
                = (nextCheckpointSingleIndex + 1) % checkpointSingleList.Count;
            OnPlayerCorrectCheckpoint?.Invoke(this, new CheckpointEventArgs { carTransform = carTransform });
        }
        else
        {
            // Wrong checkpoint
            Debug.Log("Wrong");
            OnPlayerWrongCheckpoint?.Invoke(this, new CheckpointEventArgs { carTransform = carTransform });

            CheckpointSingle correctCheckpointSingle = checkpointSingleList[nextCheckpointSingleIndex];

        }
    }

    public void ResetCheckpoints(Transform carTransform)
    {
        int carIndex = carTransformList.IndexOf(carTransform);
        if (carIndex != -1)
        {
            nextCheckpointSingleIndexList[carIndex] = 0;
        }
        else
        {
            Debug.Log("Wrong checkpoint reached by " + carTransform.name);
        }
    }


    public CheckpointSingle GetNextCheckpoint(Transform carTransform)
    {
        int nextCheckpointSingleIndex = nextCheckpointSingleIndexList[carTransformList.IndexOf(carTransform)];
        CheckpointSingle nextCheckpointSingle = checkpointSingleList[nextCheckpointSingleIndex];
        return nextCheckpointSingle;
    }
}
