using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SRP_BlackOut : MonoBehaviour
{
    /// <summary>
    /// SRP_BlackOut
    /// 用以清理PRB_BlackIn和PRB_BlackOut的Prefab，节约内存资源。
    /// </summary>
    void Start() => Destroy(gameObject,GetComponent<Animation>().clip.length);
}
