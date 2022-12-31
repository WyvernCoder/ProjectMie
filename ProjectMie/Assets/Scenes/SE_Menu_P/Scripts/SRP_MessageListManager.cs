using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SRP_MessageListManager : MonoBehaviour
{
    public GameObject MessagePrefab;
    public void CreateMessage(bool isOK, string Message, float LifeTime = 2.0f) => StartCoroutine(GO(isOK, Message, LifeTime));
    IEnumerator GO(bool isOK, string Message, float LifeTime = 2.0f)
    {
        if(MessagePrefab == null)
        {
            print("输出信息失败！");
            yield break;
        }
        var GO = Instantiate(MessagePrefab);
        yield return null;
        GO.GetComponent<SRP_MessageElement>().InitalMessage(isOK,Message,LifeTime);
        GO.transform.SetParent(transform, false);
        yield break;
    }

    public void DoClear()
    {
        foreach(Transform TM in transform)
        {
            TM.gameObject.GetComponent<SRP_MessageElement>().IAmClear();
        }
    }
}
