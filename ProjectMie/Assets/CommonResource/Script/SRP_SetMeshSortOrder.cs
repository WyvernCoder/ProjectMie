using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteAlways]
public class SRP_SetMeshSortOrder : MonoBehaviour
{
    public int order = 0;
    void FixedUpdate()
    {
        if(GetComponent<SkinnedMeshRenderer>() != null)
            GetComponent<SkinnedMeshRenderer>().sortingOrder = order;
        if(GetComponent<MeshRenderer>() != null)
            GetComponent<MeshRenderer>().sortingOrder = order;
    }
}
