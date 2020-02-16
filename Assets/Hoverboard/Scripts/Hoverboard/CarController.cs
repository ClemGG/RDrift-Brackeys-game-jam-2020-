using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [SerializeField] float acc;     //Vitesse de déplacement
    [SerializeField] float rotSpeed;    //Vitesse de rotation
    [SerializeField] float turnRotAngle; //Angle maximal sur l'axe Z quand le joueur va pivoter sur sa planche
    [SerializeField] float turnRotSeekSpeed;    //Le temps que prendra la planche pour atteindre turnRotAngle sur l'axe Z

    float rotVelocity;
    float groundAngleVelocity;


    Transform t;
    Rigidbody rb;




    private void Start()
    {
        t = transform;
        rb = GetComponent<Rigidbody>();
    }


    private void FixedUpdate()
    {
        rb.drag = Physics.Raycast(t.position, -t.up, 3f) ? 1f : 0f;

        Vector3 forwardForce = t.forward * acc * Input.GetAxis("Vertical");
        forwardForce *= rb.mass * Time.deltaTime;
        rb.AddForce(forwardForce);

        Vector3 turnTorque = Vector3.up * rotSpeed * Input.GetAxis("Horizontal");
        turnTorque *= rb.mass * Time.deltaTime;
        rb.AddTorque(turnTorque);

        Vector3 newRot = t.eulerAngles;
        newRot.z = Mathf.SmoothDampAngle(newRot.z, Input.GetAxis("Horizontal") * -turnRotAngle, ref rotVelocity, turnRotSeekSpeed);
        t.eulerAngles = newRot;

    }
}
