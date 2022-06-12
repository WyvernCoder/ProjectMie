using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SRP_MainmenuLevelManager : MonoBehaviour
{
    [Tooltip("把主菜单的东西放到这里，开始游戏后这里的东西会全部被隐藏掉！")]
    public List<GameObject> MainmenuGOs = new List<GameObject>();
    
    void Start()
    {
        
    }
}
