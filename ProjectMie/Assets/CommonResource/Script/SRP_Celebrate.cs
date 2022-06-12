using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SRP_Celebrate : MonoBehaviour
{
    /// <summary>
    /// SRP_Celebrate
    /// 作用：用于实现“庆祝”效果。
    /// 用法：直接实例化该Prefab即可实现“庆祝”效果。
    /// </summary>
    
    [Header("对象引用部分")]

    [Tooltip("“彩带”Prefab，可选其他Prefab，具体参考PRB_Celebrate_ColorWire。")]
    public GameObject ColorWirePrefab;

    [Tooltip("彩带Prefab的创建位置，可以使用空GameObject标记位置。")]
    public GameObject SpawnLocationPrefab;




    
    [Header("用户赋值部分")]

    [Tooltip("彩带Prefab的实例化数量。")]
    public int ColorWirePrefabAmount = 32;
    
    [Tooltip("彩带施力最大力度。")]
    public float ForcePower = 2000f;
    
    [Tooltip("彩带重力。")]
    public float Gravity = 5f;
    
    [Tooltip("彩带销毁时间，避免炸内存。")]
    public float RemoveTimeSeconds = 3f;
    
    [Tooltip("使用随机彩带颜色。")]
    public bool UseRandomColor = true;
    
    [Tooltip("使用随机彩带大小。")]
    public bool UseRandomScale = false;

    [Tooltip("使用发散力道。")]
    public bool UseRandomDirection = false;




    private List<GameObject> ColorWirePrefabList = new List<GameObject>();//彩带Prefab的List，便于后期销毁彩带。
    void Awake()
    {
        Invoke("Boom",0.1f);
    }

    void Boom()
    {
        for (int i = 0; i < ColorWirePrefabAmount; i++)
        {
            ColorWirePrefabList.Add(Instantiate(ColorWirePrefab));//实例化彩带Prefab并增加List元素
            ColorWirePrefabList[i].transform.position = SpawnLocationPrefab.transform.position;//初始化彩带位置
            foreach (Transform T in ColorWirePrefabList[i].transform)//初始化彩带Prefab属性
            {
                if (UseRandomDirection) T.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f) * ForcePower);//为彩带施力
                else T.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector3(Random.Range(-1f, 1f), Random.Range(0.3f, 1f), 0f) * ForcePower);//为彩带施力
                T.gameObject.GetComponent<Rigidbody2D>().gravityScale = Gravity;//为彩带设置重力
                if (UseRandomScale) T.transform.localScale = new Vector3(Random.Range(0.7f, 3f), Random.Range(0.7f, 3f), 1f);
                if (UseRandomColor) T.gameObject.GetComponent<SpriteRenderer>().color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)); //随机彩带颜色
            }
        }
    }

    void Start()
    {
        Invoke("CleanColorWire",RemoveTimeSeconds);//N秒后清理彩带Prefab
    }

    private void CleanColorWire()
    {
        foreach(GameObject GO in ColorWirePrefabList) Destroy(GO);
        Destroy(gameObject,0.3f);//清理自己
    }
}
