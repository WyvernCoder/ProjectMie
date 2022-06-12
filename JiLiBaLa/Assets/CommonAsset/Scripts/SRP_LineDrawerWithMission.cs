using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SRP_LineDrawerWithMission : MonoBehaviour
{
    [Tooltip("笔划Prefab")]
    public GameObject SingleLine;
    [Tooltip("完成任务特效Prefab")]
    public GameObject CelebratePrefab;
    [Tooltip("下一任务LineDrawer Prefab，留空即为无下一任务。")]
    public GameObject NextLineDrawer;
    [Tooltip("转场Prefab，可留空。")]
    public GameObject TransPrefab;
    [Tooltip("笔划起始宽度")]
    public float StartWidth = 0.02f;
    [Tooltip("笔划结束宽度")]
    public float EndWidth = 0.01f;
    [Tooltip("笔划起始材质")]
    public Color StartColor = Color.red;
    [Tooltip("笔划结束材质")]
    public Color EndColor = Color.yellow;
    [Tooltip("笔划材质，默认应选Sprite-Default")]
    public Material PenMaterial = null;
    [Tooltip("笔划在三维世界的Z轴深度，10代表0，0代表-10")]
    public float PenDepth = 10f;

    [Tooltip("任务点触发半径")]
    public float MissionCompleteDelta = 1f;

    [HideInInspector]
    public List<GameObject> LineList = new List<GameObject>();//线条GO的List

    public List<GameObject> MissionGOList = new List<GameObject>();

    private int currentMissionIndex = 0;
    
    private SRP_SingleLine cache_;

    void Awake()
    {
        transform.Find("Texture/").GetComponent<SpriteMask>().sprite = transform.Find("Texture/").GetComponent<SpriteRenderer>().sprite;
        transform.Find("Texture/").GetComponent<SpriteRenderer>().enabled = false;
    }
    bool isComplete = false;
    void Update()
    {
        if(Input.GetMouseButtonDown(0) && isComplete == false) 
        {
            //创建新线条
            cache_ = Instantiate(SingleLine).GetComponent<SRP_SingleLine>();

            //初始化线条
            cache_.InitalLine(PenMaterial,StartColor,EndColor,StartWidth,EndWidth,PenDepth);

            // cache_.lineDrawer = this;

            //添加进列表
            LineList.Add(cache_.gameObject);
        }

        if(MissionGOList.Count == 0)
        {
            print("未设置任务，脚本终止。");
            Destroy(gameObject);
        }

        if(currentMissionIndex == MissionGOList.Count)
        {
            if(isComplete == false)
            {
                isComplete = true;
                print("任务完成。");
                foreach (var GO in LineList) Destroy(GO);
                StartCoroutine(MissionComplete());
            }
        }

        //去除任务点的深度
        if(currentMissionIndex != MissionGOList.Count)
        MissionGOList[currentMissionIndex].transform.position = new Vector3(MissionGOList[currentMissionIndex].transform.position.x,MissionGOList[currentMissionIndex].transform.position.y,0f);
        
        if(currentMissionIndex != MissionGOList.Count)
        if(Input.GetMouseButton(0))
        if(cache_ != null)
        if(GetVector2Length(cache_.lineRenderer.GetPosition(cache_.lineRenderer.positionCount - 1) - MissionGOList[currentMissionIndex].transform.position) <= MissionCompleteDelta)
        {
            print("切换到下一个目标！");
            //Instantiate(CelebratePrefab).transform.position = cache_.lineRenderer.GetPosition(cache_.lineRenderer.positionCount - 1);
            currentMissionIndex++;
        }

        if(currentMissionIndex != MissionGOList.Count)
        if(Input.GetMouseButtonUp(0))
        {

            cache_ = null;
        }
    }

    public float GetVector2Length(Vector3 vec)
    {   //不要深度
        //return Mathf.Sqrt(Mathf.Pow(vec.x,2)+Mathf.Pow(vec.y,2)+Mathf.Pow(vec.z,2));
        return Mathf.Sqrt(Mathf.Pow(vec.x,2)+Mathf.Pow(vec.y,2));
    }

    
    IEnumerator MissionComplete()
    {
        transform.Find("Texture/").GetComponent<SpriteMask>().enabled = false;
        transform.Find("Texture/").GetComponent<SpriteRenderer>().enabled = true;
        Instantiate(CelebratePrefab).transform.position = cache_.lineRenderer.GetPosition(cache_.lineRenderer.positionCount - 1);
        yield return new WaitForSeconds(2f);
        if(TransPrefab != null)Instantiate(TransPrefab);
        yield return new WaitForSeconds(0.5f);
        if (NextLineDrawer != null) Instantiate(NextLineDrawer);
        Destroy(gameObject);
        yield break;
    }
}
