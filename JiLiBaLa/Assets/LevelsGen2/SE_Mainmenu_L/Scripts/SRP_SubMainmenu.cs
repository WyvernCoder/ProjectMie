using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SRP_SubMainmenu : MonoBehaviour
{
    [HideInInspector]
    public SRP_Island Island;
    [HideInInspector]
    public int SubMainmenuIndex = -1;

    void Start()
    {

    }

    private void TestVar()
    {
        if(Island == null)print("未指定Island！");
        if(SubMainmenuIndex == -1)print("未指定IslandIndex！");
    }
}
