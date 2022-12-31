using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SRP_YangFen : MonoBehaviour
{
    [HideInInspector] public SRP_MapManager Master;
    [Header("引用设置")]
    public TMPro.TMP_Text TEXT;
    public SpriteRenderer TEXTURE;
    [Space(10)][Header("关卡设置")]
    public string label = "0";
    public string sceneName = "SE_test";
    public string sceneID = "NONE";
    private bool canInteractable = true;

    void Start()
    {
        TEXT.text = label;
        Master = GameObject.Find("MapManager").GetComponent<SRP_MapManager>();

        /* 解锁羊粪 */
        if (UserManager.GetUserSubscribeClassBool(sceneID) == true)
        {
            canInteractable = true;
            TEXTURE.color = Color.white;
        }
        else
        {
            canInteractable = false;
            TEXTURE.color = Color.grey;
        }

        /* debug用户无条件解锁关卡 */
        if(UserManager.GetUserInfo(UserInfoField.username) == "debug")
        {
            canInteractable = true;
            TEXTURE.color = Color.white;
        }
    }

    public void CONTROL_PLAY()
    {
        if(canInteractable == false) return;
        if(APIManager.API == null) SceneManager.LoadScene(sceneName);
        else 
        {
            APIManager.API.API_LoadScene_SetName(sceneName);
            APIManager.API.API_LoadScene();
        }
    }
}
