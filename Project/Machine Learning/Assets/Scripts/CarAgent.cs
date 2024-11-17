using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System;

public class CarAgent : Agent
{
    private CarController carDriver;
    [SerializeField] private TrackCheckpoints trackCheckpoints;
    [SerializeField] private Transform spawnPosition;


    private void Awake()
    {
        carDriver = GetComponent<CarController>();
    }

    private void Start()
    {
        trackCheckpoints.OnPlayerCorrectCheckpoint += TrackCheckpoints_OnCorrectCheckpoint;
        trackCheckpoints.OnPlayerWrongCheckpoint += TrackCheckpoints_OnWrongCheckpoint;
    }

    private void TrackCheckpoints_OnWrongCheckpoint(object sender, TrackCheckpoints.CheckpointEventArgs e)
    {
        if (e.carTransform == transform)
        {
            AddReward(-1f);
        }
    }

    private void TrackCheckpoints_OnCorrectCheckpoint(object sender, TrackCheckpoints.CheckpointEventArgs e)
    {
        if (e.carTransform == transform)
        {
            AddReward(1f);
            Debug.Log("Correct checkpoint reached by " + transform.name);
        }
    }

    public override void OnEpisodeBegin()
    {
        Debug.Log($"Resetting checkpoints for {transform.name}");
        transform.position = spawnPosition.position + new Vector3(UnityEngine.Random.Range(-11f, 15f), 0f, UnityEngine.Random.Range(-11f, 15f));
        transform.forward = spawnPosition.forward;
        trackCheckpoints.ResetCheckpoints(transform); // Make sure this transform exists in the list
        carDriver.StopCompletely();
    }


    public override void OnActionReceived(ActionBuffers actions)
    {
        // Get the car's rigidbody
        //sensor.AddObservation(carRigidbody.position);
        float forwardAmount = 0f;
        float turnAmount = 0f;

        switch (actions.DiscreteActions[0])
        {
            case 0: forwardAmount = 0f; break;
            case 1: forwardAmount = +1f; break;
            case 2: forwardAmount = -1f; break;
        }
        switch (actions.DiscreteActions[1])
        {
            case 0: turnAmount = 0f; break;
            case 1: turnAmount = +1f; break;
            case 2: turnAmount = -1f; break;
        }
        carDriver.SetInputs(forwardAmount, turnAmount);
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        Vector3 checkpointForward = trackCheckpoints.GetNextCheckpoint(transform).transform.forward;
        float directionDot = Vector3.Dot(checkpointForward, transform.forward);
        sensor.AddObservation(directionDot);
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Walls"))
        {
            Debug.Log("Hit a wall");
            AddReward(-0.5f);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Walls"))
        {
            AddReward(-0.1f);
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        int forwardAction = 0;
        if (Input.GetKey(KeyCode.W))
        {
            forwardAction = 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            forwardAction = 2;
        }

        int turnAction = 0;
        if (Input.GetKey(KeyCode.A))
        {
            turnAction = 2;
        }
        if (Input.GetKey(KeyCode.D))
        {
            turnAction = 1;
        }

        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = forwardAction;
        discreteActions[1] = turnAction;
        //Debug.Log($"Heuristic Actions: forward = {forwardAction}, turn = {turnAction}");
    }

}
