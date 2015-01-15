using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public abstract class AOCommon : MonoBehaviour
{
    public IntBounds IntBounds
    {
        get { return Grid.CalculateIntBounds(GetComponent<MeshFilter>().sharedMesh, transform); }
    }
}
