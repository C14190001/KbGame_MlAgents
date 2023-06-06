using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class AgentMovement : Agent
{
    [SerializeField] private Transform targetTransform;

    public override void OnEpisodeBegin()
    {
        Vector3 pos = new Vector3(-23.38f,1.215078f,-0.16f);
        transform.localPosition = pos;
        
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetTransform.localPosition);
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];


        float moveSpeed = 1f;
        transform.localPosition += new Vector3(moveX, 0 , moveZ) * Time.deltaTime * moveSpeed;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions =  actionsOut.ContinuousActions;

        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<Goal>(out Goal goal)){
            SetReward(1f);
            EndEpisode();
        }
        if(other.TryGetComponent<Wall>(out Wall wall)){
            SetReward(-1f);
            EndEpisode();
        }
 
    }
}
