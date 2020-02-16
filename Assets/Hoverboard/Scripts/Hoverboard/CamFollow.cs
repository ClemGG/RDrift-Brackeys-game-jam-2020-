using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    Transform t;
    [SerializeField] Transform target;

    [SerializeField] float dstUp, dstBack, minHeight, moveSmooth = .2f;

    Vector3 posVelocity;

    // Start is called before the first frame update
    void Start()
    {
        t = transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 newPos = target.position + (target.forward * dstBack);
        newPos.y = Mathf.Max(newPos.y + dstUp, minHeight);

        t.position = Vector3.SmoothDamp(t.position, newPos, ref posVelocity, moveSmooth);

        Vector3 focalPoint = target.position + (target.forward * 5f);
        t.LookAt(focalPoint);
    }
}
