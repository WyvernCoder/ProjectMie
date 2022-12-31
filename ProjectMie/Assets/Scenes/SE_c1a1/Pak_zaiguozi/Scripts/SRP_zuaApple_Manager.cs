using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SRP_zuaApple_Manager : MonoBehaviour
{
    public Transform rootTransform;
    public GameObject appleCollection;
    private List<SRP_zuaApple_smallApple> appleList = new List<SRP_zuaApple_smallApple>();

    void Start()
    {
        foreach(Transform T in appleCollection.transform) 
        appleList.Add(T.gameObject.GetComponent<SRP_zuaApple_smallApple>());
        
        StartCoroutine(check());
    }
    
    IEnumerator check()
    {
        if(appleList.Count == 0)
        {
            Destroy(rootTransform.gameObject);
            yield break;
        }

        for(int index = 0; index<appleList.Count; index++)
        {
            if(appleList[index] == null) 
            {
                appleList.RemoveAt(index);
                index--;
            }
        }

        yield return new WaitForSeconds(1f);
        StartCoroutine(check());
    }
}
