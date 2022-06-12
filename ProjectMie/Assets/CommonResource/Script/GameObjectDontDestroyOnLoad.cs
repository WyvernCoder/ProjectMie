using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectDontDestroyOnLoad : MonoBehaviour
{
    [Header("把这个脚本挂载到一个GameObject上，这个GameObject就不会因为载入而被删除了。")]
    public string aaaa;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Destroy(this);
    }
}
