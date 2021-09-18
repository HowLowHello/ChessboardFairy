using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class MaterialSetter : MonoBehaviour
{
    private MeshRenderer _meshRenderer;

    private MeshRenderer meshRenderer
    {
        get
        {
            if(this._meshRenderer == null)
            {
                this._meshRenderer = GetComponent<MeshRenderer>();
            }
            return this._meshRenderer;
        }
    }

    internal void SetSingleMaterial(Material materialIn)
    {
        meshRenderer.material = materialIn;
    }
}
