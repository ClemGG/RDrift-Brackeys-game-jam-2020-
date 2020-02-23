using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotandmovesine : MonoBehaviour
{
    [SerializeField] bool rot, move;
    [SerializeField] float rotSpeed, moveSpeed, moveMagnitude = 1f;

    Transform t;
    Vector3 _startPosition;
    // Start is called before the first frame update
    void Start()
    {
        t = transform;
        _startPosition = t.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (rot)
        {
            t.Rotate(t.InverseTransformDirection(t.up) * rotSpeed * Time.deltaTime);
        }
        if (move)
        {
            t.position = _startPosition + t.InverseTransformDirection(new Vector3(0f, Mathf.Sin(Time.time * moveSpeed) * moveMagnitude, 0f));
        }
    }
}
