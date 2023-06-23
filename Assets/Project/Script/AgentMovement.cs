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
    bool isGrounded = true;

    public override void OnEpisodeBegin()
    {
        rb = this.GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        //rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;

        Vector3 pos = new Vector3(-18.5f,1f,0f);
        transform.localPosition = pos;

        ////Lokasi PressurePlate
        targetTransform.localPosition = new Vector3(UnityEngine.Random.Range(-26.0f, -8.0f), 0.0f, UnityEngine.Random.Range(-2.0f, 2.0f));

        //switch (checkpoints)
        //{
        //    case 0:
        //        targetTransform.localPosition = new Vector3(-19f, 0.0f, 0f);
        //        break;
        //    case 1:
        //        targetTransform.localPosition = new Vector3(-14f, 0.0f, 0f);
        //        break;
        //    case 2:
        //        targetTransform.localPosition = new Vector3(1f, 0.0f, 0f);
        //        break;
        //    case 3:
        //        targetTransform.localPosition = new Vector3(15f, 0.0f, 0f);
        //        break;
        //}
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetTransform.localPosition);
        sensor.AddObservation(checkpoints);

        Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 10f);
        //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
        sensor.AddObservation(hit.distance);
        //Debug.Log("Forward: " + hit.distance);

        Physics.Raycast(transform.position, transform.TransformDirection(Vector3.back), out hit, 10f);
        //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.back) * hit.distance, Color.yellow);
        sensor.AddObservation(hit.distance);
        //Debug.Log("Back: " + hit.distance);

        Physics.Raycast(transform.position, transform.TransformDirection(Vector3.left), out hit, 10f);
        //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.left) * hit.distance, Color.yellow);
        sensor.AddObservation(hit.distance);
        //Debug.Log("Left: " + hit.distance);

        Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out hit, 10f);
        //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.right) * hit.distance, Color.yellow);
        sensor.AddObservation(hit.distance);
        //Debug.Log("Right: "+hit.distance);
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];
        float jumpV = actions.ContinuousActions[2];

        transform.Translate(Vector3.forward * Time.deltaTime * moveSpeed * moveZ);
        transform.Translate(Vector3.right * Time.deltaTime * moveSpeed * moveX);

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down),out hit, 1f))
        {
            isGrounded = true;
            Debug.Log("Ground");
        }
        else
        {
            isGrounded = false;
            Debug.Log("Not Ground");
        }

        if (jumpV >= 1)
        {
            jump();
        }

        //transform.localPosition += new Vector3(moveX, 0 , moveZ) * Time.deltaTime * moveSpeed;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions =  actionsOut.ContinuousActions;

        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
        if (Input.GetKeyDown(KeyCode.Space)) {
            Debug.Log("Jump button");
            jump();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<Wall2>(out Wall2 wall2))
        {
            AddReward(-1f);
        }
    }

    private void jump()
    {
        if (isGrounded)
        {
            rb.AddForce(transform.up * 10f, ForceMode.Impulse);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<Goal>(out Goal goal)){
            AddReward(10f);
            checkpoints++;
            if(checkpoints > 3)
            {
                checkpoints = 0;
            }

            EndEpisode();
        }
        
        if (other.TryGetComponent<Wall>(out Wall wall)){
            AddReward(-5f);
            checkpoints = 0;
            EndEpisode();
        }
 
    }
}
