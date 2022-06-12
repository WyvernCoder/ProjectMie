using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SRP_RandomText : MonoBehaviour
{
    public GameObject DLC;
    void FixedUpdate()
    {
        DLC.GetComponent<SRP_DLC>().InitalThis(null,null,null,null,RandomText(Random.Range(0,9999)),false);
    }
    string RandomText(int I)
    {
        string result = null;
        switch(I%10)
        {
            case 0: result = "今";break;
            case 1: result = "晚";break;
            case 2: result = "必";break;
            case 3: result = "死";break;
            case 4: result = "啊";break;
            case 5: result = "哦";break;
            case 6: result = "额";break;
            case 7: result = "下";break;
            case 8: result = "了";break;
            case 9: result = "看";break;
        }
        return result;
    }
}
