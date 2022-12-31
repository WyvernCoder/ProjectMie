using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SRP_UserWeight : MonoBehaviour
{
    TMPro.TMP_Text TEXT;
    void Awake()
    {
        if(GetComponent<TMPro.TMP_Text>() == null) Destroy(this);
        TEXT = GetComponent<TMPro.TMP_Text>();
    }

    void Start()
    {
        
        StartCoroutine(UpdateUserWeight());
    }

    IEnumerator UpdateUserWeight()
    {
        if(UserManager.isLogin) TEXT.text = "普通用户";
        else TEXT.text = "";
        yield return new WaitForSeconds(1f);
        StartCoroutine(UpdateUserWeight());
        yield break;
    }
}
