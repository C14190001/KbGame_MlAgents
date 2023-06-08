using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class AgentMovement : Agent
{
    [SerializeField] private Transform targetTransform;
    Rigidbody rb = null;
    RaycastHit hit;
    float moveSpeed = 3f;
    int checkpoints = 0;

    public override void OnEpisodeBegin()
    {
        rb = this.GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        //rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;

        Vector3 pos = new Vector3(-23.38f,1.215078f,-0.16f);
        transform.localPosition = pos;

        //Lokasi PressurePlate
        switch (checkpoints)
        {
            case 0:
                targetTransform.localPosition = new Vector3(-19f, 0.0f, 0f);
                break; 
            case 1:
                targetTransform.localPosition = new Vector3(-14f, 0.0f, 0f);
                break;
            case 2:
                targetTransform.localPosition = new Vector3(1f, 0.0f, 0f);
                break;
            case 3:
                targetTransform.localPosition = new Vector3(15f, 0.0f, 0f);
                break;
        }
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetTransform.localPosition);
        sensor.AddObservation(checkpoints);

        Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 10f);
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
        sensor.AddObservation(hit.distance);
        //Debug.Log("Forward: " + hit.distance);

        Physics.Raycast(transform.position, transform.TransformDirection(Vector3.back), out hit, 10f);
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.back) * hit.distance, Color.yellow);
        sensor.AddObservation(hit.distance);
        //Debug.Log("Back: " + hit.distance);

        Physics.Raycast(transform.position, transform.TransformDirection(Vector3.left), out hit, 10f);
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.left) * hit.distance, Color.yellow);
        sensor.AddObservation(hit.distance);
        //Debug.Log("Left: " + hit.distance);

        Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out hit, 10f);
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.right) * hit.distance, Color.yellow);
        sensor.AddObservation(hit.distance);
        //Debug.Log("Right: "+hit.distance);
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        transform.Translate(Vector3.forward * Time.deltaTime * moveSpeed * moveZ);
        transform.Translate(Vector3.right * Time.deltaTime * moveSpeed * moveX);

        //transform.localPosition += new Vector3(moveX, 0 , moveZ) * Time.deltaTime * moveSpeed;
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
            SetReward(10f);
            checkpoints++;
            if(checkpoints > 3)
            {
                checkpoints = 0;
            }

            EndEpisode();
        }
        if (other.TryGetComponent<Wall2>(out Wall2 wall2))
        {
            SetReward(-1f);
        }
        if (other.TryGetComponent<Wall>(out Wall wall)){
            SetReward(-5f);
            checkpoints = 0;
            EndEpisode();
        }
 
    }
}
