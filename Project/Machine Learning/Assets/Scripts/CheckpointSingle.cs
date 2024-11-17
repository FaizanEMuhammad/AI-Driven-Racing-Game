using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointSingle : MonoBehaviour
{
    private TrackCheckpoints trackCheckpoints;
    void Awake()
    {
        trackCheckpoints = GetComponentInParent<TrackCheckpoints>();
        if (trackCheckpoints == null)
        {
            Debug.LogError("CheckpointSingle does not have a reference to TrackCheckpoints");
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        CarController carController = other.GetComponentInParent<CarController>();
        if (carController != null)
        {
            trackCheckpoints.CarThroughCheckpoint(this, carController.transform);
            Debug.Log("Checkpoint reached by " + other.name);
        }
    }
    public void SetTrackCheckpoints(TrackCheckpoints trackCheckpoints)
    {
        this.trackCheckpoints = trackCheckpoints;
    }
}
