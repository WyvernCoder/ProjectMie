using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SRP_TTT : MonoBehaviour
{
    public GameObject DLC;
    public GameObject Content;
    public int DLCAmount = 10;

    // Start is called before the first frame update
    void Start()
    {
        for(int i =0; i<DLCAmount; i++)
        {
            var GO = Instantiate(DLC);
            GO.GetComponent<SRP_DLC>().InitalThis(null,null,null,null,"吗",false);
            GO.transform.SetParent(Content.transform,true);
        }
    }
}
