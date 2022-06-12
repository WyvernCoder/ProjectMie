using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SRP_ZBK_Item : MonoBehaviour
{
    [HideInInspector]
    public int ItemIndex = -1;
    [Tooltip("水果种类")]
    public int KindIndex = -1;
    [Tooltip("speed")]
    public float MoveSpeed = 0.5f;
    private Vector3 OriginalLoc;

    void Awake()
    {
        OriginalLoc = transform.position;
        if(KindIndex == -1) print("未设置水果种类");
    }

    bool toggle = false;
    bool moving = false;
    public void ToggleMove() => StartCoroutine(ToggleMove_());
    IEnumerator ToggleMove_()
    {
        if(moving == true) yield break;
        moving = true;

        if (toggle == false)
        {
            if(SRP_ZBK_Manager.ZBKManagerInstance.CurrentPlaceIndex == 3) 
            {
                moving = false;
                yield break;
            }
            toggle = true;

            Vector3 GoLoc = SRP_ZBK_Manager.ZBKManagerInstance.LockerGOList[SRP_ZBK_Manager.ZBKManagerInstance.CurrentPlaceIndex].transform.position;

            if(SRP_ZBK_Manager.ZBKManagerInstance.CurrentPlaceIndex != 3) SRP_ZBK_Manager.ZBKManagerInstance.CurrentPlaceIndex++;
            SRP_ZBK_Manager.SelectedPrefabList.Add(gameObject);
            SRP_ZBK_Manager.ZBKManagerInstance.AddedMessage();

            while (GetVector3Length(GoLoc - transform.position) >= 0.1f)
            {
                transform.position = Vector3.Lerp(transform.position, GoLoc, MoveSpeed);
                yield return 0;
            }
            transform.position = GoLoc;//纠正位置
        }
        else
        {
            if(SRP_ZBK_Manager.SelectedPrefabList[SRP_ZBK_Manager.SelectedPrefabList.Count - 1] != gameObject) 
            {
                moving = false;
                yield break;
            }
            toggle = false;

            if(SRP_ZBK_Manager.ZBKManagerInstance.CurrentPlaceIndex != 0) SRP_ZBK_Manager.ZBKManagerInstance.CurrentPlaceIndex--;
            SRP_ZBK_Manager.SelectedPrefabList.Remove(gameObject);

            while (GetVector3Length(OriginalLoc - transform.position) >= 0.1f)
            {
                transform.position = Vector3.Lerp(transform.position, OriginalLoc, MoveSpeed);
                yield return 0;
            }
            transform.position = OriginalLoc;//纠正位置
        }
        moving = false;
        yield break;
    }


    float GetVector3Length(Vector3 vec)
    {
        //放弃深度
        return Mathf.Sqrt(Mathf.Pow(vec.x,2)+Mathf.Pow(vec.y,2));
    }
}
