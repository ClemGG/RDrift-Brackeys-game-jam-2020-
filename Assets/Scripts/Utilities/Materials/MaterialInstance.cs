using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class MaterialInstance : MonoBehaviour
{
#if UNITY_EDITOR

    [SerializeField] Vector2 tiling = Vector2.one;


    MeshRenderer mr;

    private void OnValidate()
    {
        Reset();
    }


    private void Reset()
    {

        if (TryGetComponent(out mr))
        {
            if (mr.sharedMaterial)
            {
                Material m = new Material(mr.sharedMaterial);
                m.mainTextureScale = tiling;
                mr.material = m;
            }
        }
    }

#endif
}
