using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrusterScript : MonoBehaviour
{
    [SerializeField] float thrusterStrength;
    [SerializeField] float thrusterDistance;
    [SerializeField] Transform[] thrusters;


    Transform t;
    Rigidbody rb;




    [Space(10)]
    [Header("Gizmos :")]
    [Space(10)]

    [SerializeField] Color platformStabilizerColor = Color.red;
    [SerializeField] float platformStabilizerSize = .05f;






    private void Start()
    {
        t = transform;
        rb = GetComponent<Rigidbody>();
    }



    private void FixedUpdate()
    {
        for (int i = 0; i < thrusters.Length; i++)
        {

            if(Physics.Raycast(thrusters[i].position, -thrusters[i].up, out RaycastHit hit, thrusterDistance))
            {
                float dstPercentage = 1 - hit.distance / thrusterDistance;
                Vector3 downwardForce = t.up * thrusterStrength * dstPercentage;
                downwardForce *= rb.mass * Time.deltaTime;

                rb.AddForceAtPosition(downwardForce, thrusters[i].position);
            }
        }
    }



#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        Gizmos.color = platformStabilizerColor;

        for (int i = 0; i < thrusters.Length; i++)
        {
            if (thrusters[i] != null)
            {
                Gizmos.DrawWireSphere(thrusters[i].position, platformStabilizerSize);
            }
        }
    }


#endif
}
