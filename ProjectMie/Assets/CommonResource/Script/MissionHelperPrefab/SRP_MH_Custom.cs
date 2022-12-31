using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SRP_MH_Custom : SRP_MH_Master
{
    private GameObject ThePrefab;

    public override void Initial(object SettingObject, SRP_MissionHelper_Master MASTER)
    {
        var CustomSettingObject = SettingObject as Custom;
        Master = MASTER;
        ThePrefab = Instantiate(CustomSettingObject.PrefabToSpawn);
        StartCoroutine(StartCheckProcess());
    }

    IEnumerator StartCheckProcess()
    {
        yield return null;//等待prefab实例化完成

        while(true)
        {
            if(ThePrefab == null) break;
            yield return null;
        }
        
        Master.PlayNext();

        yield break;
    }
}
