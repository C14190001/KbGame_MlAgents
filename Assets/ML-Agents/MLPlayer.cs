using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;


public class MLPlayer : MonoBehaviour
{
    //test gerak
    public float speed = 5.0f;
    private float horizontalInput;
    private float forwardInput;


    public float Force = 15f;
    private Rigidbody rb = null;

    private void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
    }

    private void FixedUpdate()
    {
        // if (Input.GetKey(KeyCode.UpArrow) == true){
        //     Thrust();
        // }
    }

    // private void Thrust(){
    //     rb.AddForce(Vector3.up * Force, ForceMode.Acceleration);
    // }

    void Update(){
        horizontalInput = Input.GetAxis("Horizontal");
        forwardInput = Input.GetAxis("Vertical");

        transform.Translate(Vector3.forward * Time.deltaTime * speed * forwardInput);
        transform.Translate(Vector3.right * Time.deltaTime * speed * horizontalInput);
    }
}
