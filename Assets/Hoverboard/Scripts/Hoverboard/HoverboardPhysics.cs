using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverboardPhysics : MonoBehaviour
{
    [Space(10)]
    [Header("Scripts & Components :")]
    [Space(10)]



    Transform t;
    Transform body;

    BoxCollider bc;
    ConstantForce constantForce;
    Rigidbody rb;

    Transform[] cornersTransforms = new Transform[5];


    [Space(10)]
    [Header("Values :")]
    [Space(10)]


    [SerializeField] float forwardPower;
    [SerializeField] float steerPower;
    [SerializeField] float landingPower;
    [SerializeField] float jumpingPower;
    [SerializeField] float hoverHeight;
    [SerializeField] float stability = 1f;

    [SerializeField] float speedUpdate;

    Vector3[] hitNormals = new Vector3[5];
    Vector3[] lastNormals = new Vector3[5];
    Quaternion rotation;
    bool physicsSetup = false;
    float increment;
    float yBounce;
    float dst;
    Vector3 boxDim; //Les dimensions du BoxCollider de la planche
    Vector3 lastPosition;
    Vector3 average;





    [Space(10)]
    [Header("Gizmos :")]
    [Space(10)]

    [SerializeField] Color platformStabilizerColor = Color.red;
    [SerializeField] float platformStabilizerSize = .05f;





    private void Awake()
    {
        InitPhysics();

    }


    private void Update()
    {
        CalculateSpeed();
    }

    

    private void FixedUpdate()
    {

        if (physicsSetup)
        {
            for (int i = 0; i < cornersTransforms.Length; i++)
            {
                if(Physics.Raycast(cornersTransforms[i].position, -cornersTransforms[i].up, out RaycastHit hit, hoverHeight + 100f))
                {
                    hitNormals[i] = body.InverseTransformDirection(hit.normal);

                    if(lastNormals[i] != hitNormals[i])
                    {
                        increment = 0f;
                        lastNormals[i] = hitNormals[i];
                    }

                    dst = hit.distance;
                    if(dst < hoverHeight)
                    {
                        constantForce.relativeForce = (-average + t.up) * rb.mass * jumpingPower * rb.drag * Mathf.Min(hoverHeight, hoverHeight / dst);
                    }
                    else
                    {
                        constantForce.relativeForce = -t.up * rb.mass * landingPower * rb.drag * Mathf.Min(hoverHeight, hoverHeight / dst);
                    }
                }
                else
                {
                    constantForce.relativeForce = -t.up * rb.mass * landingPower * rb.drag * 6f * (1f - Input.GetAxis("Vertical"));
                }


            }

            average = Vector3.zero;
            for (int i = 0; i < hitNormals.Length; i++)
            {
                average += hitNormals[i];
            }
            average = -average / 2f;

            if(increment < 1f || increment > 1f) { increment += .03f; }

            rotation = Quaternion.Slerp(body.localRotation, Quaternion.Euler(average * Mathf.Rad2Deg), increment);
            body.localRotation = rotation;
            body.localRotation = new Quaternion(body.localRotation.x, t.up.y * Mathf.Deg2Rad, body.localRotation.z, body.localRotation.w);

            float forwardForce = Input.GetAxis("Vertical") * forwardPower;
            rb.AddForce(t.forward * forwardForce);

            float steerForce = Input.GetAxis("Horizontal") * steerPower;
            rb.AddTorque(t.up * steerForce);


        }
    }






    private void CalculateSpeed()
    {
        if(lastPosition != t.position)
        {
            float dst = Vector3.Distance(t.position, lastPosition);
            speedUpdate = (dst / 1000f) / (Time.deltaTime / 3600f);   // km/h
            lastPosition = t.position;
        }
    }



    private void InitPhysics()
    {
        t = transform;
        body = t.GetChild(0);

        
        if(!body.TryGetComponent(out bc))
        {
            bc = body.gameObject.AddComponent<BoxCollider>();
        }

        if (!t.TryGetComponent(out constantForce))
        {
            constantForce = gameObject.AddComponent<ConstantForce>();
        }

        if (!t.TryGetComponent(out rb))
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        boxDim = new Vector3(bc.size.x * body.localScale.x, bc.size.y * body.localScale.y, bc.size.z * body.localScale.z) * stability;
        Vector3[] cornersPoint = new Vector3[5];
        cornersPoint[0] = new Vector3(t.position.x - boxDim.x / 2f, t.position.y - boxDim.y / 2f, t.position.z + boxDim.z / 2f);
        cornersPoint[1] = new Vector3(t.position.x + boxDim.x / 2f, t.position.y - boxDim.y / 2f, t.position.z + boxDim.z / 2f);
        cornersPoint[2] = new Vector3(t.position.x + boxDim.x / 2f, t.position.y - boxDim.y / 2f, t.position.z - boxDim.z / 2f);
        cornersPoint[3] = new Vector3(t.position.x - boxDim.x / 2f, t.position.y - boxDim.y / 2f, t.position.z - boxDim.z / 2f);
        cornersPoint[4] = t.position;

        //Destroy(bc);

        for (int i = 0; i < cornersPoint.Length; i++)
        {
            GameObject platformStabilizer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            platformStabilizer.name = string.Format("Platform Stabilizer ({0})", i);
            platformStabilizer.transform.parent = body;
            platformStabilizer.transform.localPosition = t.InverseTransformPoint(cornersPoint[i]);
            cornersTransforms[i] = platformStabilizer.transform;

            Destroy(platformStabilizer.GetComponent<MeshRenderer>());
            Destroy(platformStabilizer.GetComponent<MeshFilter>());
            Destroy(platformStabilizer.GetComponent<Collider>());
        }

        cornersPoint = null;
        physicsSetup = true;

    }


#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        Gizmos.color = platformStabilizerColor;

        for (int i = 0; i < cornersTransforms.Length; i++)
        {
            if(cornersTransforms[i] != null)
            {
                Gizmos.DrawWireSphere(cornersTransforms[i].position, platformStabilizerSize);
            }
        }
    }


#endif
}
