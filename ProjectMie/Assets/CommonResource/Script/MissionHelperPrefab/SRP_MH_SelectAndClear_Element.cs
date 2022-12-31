using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 消消乐模块所用元素
/// </summary>
public class SRP_MH_SelectAndClear_Element : MonoBehaviour
{
    [HideInInspector] public UnityEngine.CanvasGroup CG;
    [HideInInspector] public SRP_MH_SelectAndClear Parent;
    [HideInInspector] public int index = -1;

    void Awake()
    {
        if(gameObject.GetComponent<CanvasGroup>() == null) CG = gameObject.AddComponent<CanvasGroup>();
        else CG = gameObject.GetComponent<CanvasGroup>();
        CG.alpha = 0f;
    }

    public void StartShow() => StartCoroutine(ShowMe());

    IEnumerator ShowMe()
    {
        while(true)
        {
            if(CG.alpha >= 1f - Mathf.Epsilon) break;
            CG.alpha += Time.deltaTime * 1.5f;
            yield return null;
        }

        CG.alpha = 1f;
        yield break;
    }

    bool doOnce = false;
    public void CONTROL_ClickMe()
    {
        if(doOnce == true) return;
        StartCoroutine(ClickAnim());
        doOnce = true;
        Parent.PlayClickSound(index);
    }
    IEnumerator ClickAnim()
    {
        var RT = gameObject.GetComponent<RectTransform>();
        var FinalRotate = RT.transform.eulerAngles + new Vector3(0, 0, Random.Range(-180f, 180f));
        var FinalSizeDelta = Vector2.zero;
        float Speed = 5f * Time.deltaTime;

        while(true)
        {
            if(RT.sizeDelta == FinalSizeDelta) break;
            RT.transform.Rotate(Vector3.Lerp(RT.rotation.eulerAngles, FinalRotate, Speed));
            RT.sizeDelta = Vector2.Lerp(RT.sizeDelta, FinalSizeDelta, Speed);
            yield return null;
        }

        Destroy(gameObject);
        yield break;
    }
}
