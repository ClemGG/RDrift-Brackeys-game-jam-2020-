using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargerEcranTexture : MonoBehaviour
{
    public Material matTV;
    public RenderTexture rt;
    Material mat;

    // Start is called before the first frame update
    void Start()
    {

        ChangeTexture();
    }

    private void ChangeTexture()
    {
        mat = new Material(matTV);
        mat.SetTexture("_mainTexture", rt);
        GetComponent<MeshRenderer>().material = mat;
    }
}
