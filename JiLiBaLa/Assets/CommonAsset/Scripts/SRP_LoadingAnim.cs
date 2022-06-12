using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SRP_LoadingAnim : MonoBehaviour
{
    private Canvas LoadCanvas;
    void Awake()
    {
        LoadCanvas = gameObject.GetComponent<Canvas>();
    }

    public void CloseTheLoad(float CloseAfterTime = 3f) => StartCoroutine(CallPolice(CloseAfterTime));

    IEnumerator CallPolice(float time)
    {
        yield return new WaitForSeconds(time);
        //TODO:载入完成动画
        Destroy(gameObject);
    }
}
