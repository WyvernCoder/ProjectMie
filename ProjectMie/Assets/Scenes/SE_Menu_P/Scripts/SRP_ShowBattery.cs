using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SRP_ShowBattery : MonoBehaviour
{
    private Image IMAGE;
    public List<Sprite> StatueImage = new List<Sprite>();

    void Awake()
    {
        if(gameObject.GetComponent<Image>() == null) Destroy(this);//如果被绑定的gameobject没有字组件，就自毁
        else IMAGE = gameObject.GetComponent<Image>();
    }

    void Start()
    {
        if(Application.isMobilePlatform == true) StartCoroutine(UpdateBatteryIE());
        else StartCoroutine(UpdateRandomBatteryIE());
    }

    IEnumerator UpdateBatteryIE()
    {
        if(GetBatteryLevel() >= 90) IMAGE.sprite = StatueImage[0];
        if(GetBatteryLevel() >= 70 && GetBatteryLevel() < 90) IMAGE.sprite = StatueImage[1];
        if(GetBatteryLevel() >= 40 && GetBatteryLevel() < 70) IMAGE.sprite = StatueImage[2];
        if(GetBatteryLevel() >= 20 && GetBatteryLevel() < 40) IMAGE.sprite = StatueImage[3];
        if(GetBatteryLevel() >= 0 && GetBatteryLevel() < 20) IMAGE.sprite = StatueImage[4];
        yield return new WaitForSeconds(3f);
        if(SystemInfo.batteryStatus == BatteryStatus.Charging) StartCoroutine(UpdateRandomBatteryIE());
        else if(SystemInfo.batteryStatus == BatteryStatus.Discharging) StartCoroutine(UpdateBatteryIE());
        else
        yield break;
    }

    IEnumerator UpdateRandomBatteryIE()
    {
        if(StatueImage.Count == 0) yield break;
        for(int index = 0; index < StatueImage.Count;index++) 
        {
            IMAGE.sprite = StatueImage[index];
            yield return new WaitForSeconds(0.5f);
        }
        StartCoroutine(UpdateRandomBatteryIE());
        yield break;
    }

    //返回电池电量
    public int GetBatteryLevel()
    {
        return (int)(SystemInfo.batteryLevel*100f);
    }
}
