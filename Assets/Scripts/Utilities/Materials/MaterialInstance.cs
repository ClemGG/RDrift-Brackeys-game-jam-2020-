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
                MaterialPropertyBlock mblock = new MaterialPropertyBlock();

                mblock.SetVector("_BaseMap_ST", new Vector4(tiling.x, tiling.y, 0, 0));
                mr.material = m;
                mr.SetPropertyBlock(mblock);
            }
        }
    }


#endif
}
