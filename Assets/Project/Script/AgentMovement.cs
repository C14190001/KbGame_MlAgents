using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine.UI;
using Unity.VisualScripting;

public class AgentMovement : Agent
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Transform enemyTransform;
    [SerializeField] private Transform bulletTransform;
    [SerializeField] private GameObject level1_Door;
    Rigidbody rb = null;
    RaycastHit hit;
    bool isGrounded = true;
    int fails = 0; //Jika masuk jurang lebih dari 3 kali, balik lagi ke Checkpoint 0.
    bool isHitByProjectile = false;
    bool isTimerRunning = true;
    float timerRemaining = 0f;

    //Config ====
    Vector3[] pressurePlateLocations = new Vector3[8];
    int checkpoints = 0; //Level Select (0 - 7)
    float moveSpeed = 3f;
    float jumpForce = 10f;
    float raycastLength = 5f;
    float timer = 120f;
    //============
    public override void OnEpisodeBegin()
    {
        rb = this.GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        ////Lokasi PressurePlate =
        //Level 1
        pressurePlateLocations[0] = new Vector3(-20f, 0f, 0f);
        pressurePlateLocations[1] = new Vector3(-4f, 0f, 0f);
        pressurePlateLocations[2] = new Vector3(15f, 0f, 0f);
        pressurePlateLocations[3] = new Vector3(23f, 1.25f, 0f);
        //Level 2
        pressurePlateLocations[4] = new Vector3(46f, 0.4f, 1.1f);
        pressurePlateLocations[5] = new Vector3(68f, 0.4f, 1.1f);
        pressurePlateLocations[6] = new Vector3(85.5f, 1f, 0.6f);
        pressurePlateLocations[7] = new Vector3(95.5f, 1f, 0.3f);
        ////======================

        //Set Timer
        isTimerRunning = true;
        timerRemaining = timer;

        //Set isHitByProjectile
        isHitByProjectile = false;

        //Spawn Player
        rb.velocity = Vector3.zero;
        if (checkpoints - 1 >= 0)
        {
            transform.localPosition = new Vector3(pressurePlateLocations[checkpoints - 1].x, 2f, pressurePlateLocations[checkpoints - 1].z);
        }
        else
        {
            transform.localPosition = new Vector3(-25f, 2f, 0f);
        }

        //Level 1 Door
        level1_Door.transform.localPosition = new Vector3(18f, 2f, 0);
        if (checkpoints < 3)
        {
            level1_Door.SetActive(true);
        }
        else
        {
            level1_Door.SetActive(false);
        }

        //Spawn Pressure Plate
        targetTransform.localPosition = pressurePlateLocations[checkpoints];
    }
    void Update()
    {
        if (isTimerRunning)
        {
            if (timerRemaining > 0)
            {
                timerRemaining -= Time.deltaTime;
            }
            else
            {
                isTimerRunning = false;
                AddReward(-5f);
                EndEpisode();
            }
        }
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetTransform.localPosition);
        sensor.AddObservation(enemyTransform.localPosition);
        sensor.AddObservation(checkpoints);
        sensor.AddObservation(isGrounded);
        sensor.AddObservation(timerRemaining);
        sensor.AddObservation(isHitByProjectile);
        sensor.AddObservation(fails);
        sensor.AddObservation(Vector3.Distance(transform.localPosition, targetTransform.localPosition));
        sensor.AddObservation(Vector3.Distance(transform.localPosition, enemyTransform.localPosition));

        //Debug.Log(Vector3.Distance(transform.localPosition, enemyTransform.localPosition));

        Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, raycastLength);
        sensor.AddObservation(hit.distance);

        //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
        //Debug.Log("Forward: " + hit.distance);

        Physics.Raycast(transform.position, transform.TransformDirection(Vector3.back), out hit, raycastLength);
        sensor.AddObservation(hit.distance);

        //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.right) * hit.distance, Color.yellow);
        //Debug.Log("Back: " + hit.distance);

        Physics.Raycast(transform.position, transform.TransformDirection(Vector3.left), out hit, raycastLength);
        sensor.AddObservation(hit.distance);

        //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.left) * hit.distance, Color.yellow);
        //Debug.Log("Left: " + hit.distance);

        Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out hit, raycastLength);
        sensor.AddObservation(hit.distance);

        //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.back) * hit.distance, Color.yellow);
        //Debug.Log("Right: "+hit.distance);

        Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, raycastLength);
        sensor.AddObservation(hit.distance);

        //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.back) * hit.distance, Color.yellow);
        //Debug.Log("Down: "+hit.distance);
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];
        float jumpV = actions.ContinuousActions[2];

        transform.Translate(Vector3.forward * Time.deltaTime * moveSpeed * moveZ);
        transform.Translate(Vector3.right * Time.deltaTime * moveSpeed * moveX);

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 1f))
        {
            isGrounded = true;
            //Debug.Log("Ground");
        }
        else
        {
            isGrounded = false;
            //Debug.Log("Not Ground");
        }

        if (jumpV >= 0.5)
        {
            jump();
        }
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;

        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(KeyCode.Space))
        {
            //Debug.Log("Jump button");
            jump();
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<Wall2>(out Wall2 wall2))
        {
            AddReward(-1f);
            //Debug.Log("Wall Hit");
        }
        if (collision.gameObject.TryGetComponent<Bullet>(out Bullet bullet))
        {
            //AddReward(-3f);
            isHitByProjectile = true;
        }
    }
    private void jump()
    {
        if (isGrounded)
        {
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Goal>(out Goal goal))
        {
            AddReward(10f);
            if (checkpoints < 7) {
                checkpoints++;
            }
            else
            {
                checkpoints = 0;
            }
            EndEpisode();
        }

        if (other.TryGetComponent<Wall>(out Wall wall))
        {
            //AddReward(-5f);

            if (isHitByProjectile)
            {
                AddReward(-8f);
            }
            else
            {
                AddReward(-5f);
            }

            fails++;
            if(fails > 2)
            {
                fails = 0;
                checkpoints = 0;
            }
            EndEpisode();
        }

        //if(other.TryGetComponent<Bullet>(out Bullet bullet))
        //{
        //    AddReward(-5f);
        //    EndEpisode();
        //}

    }
}
