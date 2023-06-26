using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.Burst.CompilerServices;


public class MLPlayer : MonoBehaviour
{
    //| Pake AgentMovement.cs   |
    //| Yang ini JANGAN DIPAKAI |
    
    //test gerak
    //public float speed = 5.0f;
    //private float horizontalInput;
    //private float forwardInput;
    //bool isGrounded = true;


    //public float Force = 15f;
    private Rigidbody rb = null;

    private void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        //rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
    }

    private void FixedUpdate()
    {
        //if (Input.GetKey(KeyCode.UpArrow) == true)
        //{
        //    Thrust();
        //}
    }

    //private void Thrust()
    //{
    //    rb.AddForce(Vector3.up * Force, ForceMode.Acceleration);
    //}

    void Update(){
        //horizontalInput = Input.GetAxis("Horizontal");
        //forwardInput = Input.GetAxis("Vertical");
        //if (Physics2D.Raycast(transform.position, Vector2.down, 0.5f))
        //{
        //    isGrounded = true;
        //    Debug.Log("Ground");
        //}
        //else
        //{
        //    isGrounded = false;
        //    Debug.Log("Not Ground");
        //}

        //if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        //{
        //    Debug.Log("Jump button");
        //    rb.AddForce(transform.up * 50f, ForceMode.Impulse);
        //}

        //transform.Translate(Vector3.forward * Time.deltaTime * speed * forwardInput);
        //transform.Translate(Vector3.right * Time.deltaTime * speed * horizontalInput);
    }
}
