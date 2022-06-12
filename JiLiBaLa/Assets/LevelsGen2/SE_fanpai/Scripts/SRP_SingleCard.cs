using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SRP_SingleCard : MonoBehaviour
{
    [HideInInspector]
    public int FruitIndex = -1;

    [Tooltip("音标")]
    public string SoundBiaoText = "orange \n [ˈɒrɪndʒ] \n 橙子";
    [Tooltip("音频")]
    public AudioClip VoiceFile;

    void Awake()
    {
        gameObject.AddComponent<AudioSource>();
    }

    public void CONTROL_ClickMe(bool noGetCi = false)
    {
        if(FruitIndex == -1)
        {
            print("未赋值水果Index。");
        }

        //无论对错都播放声音
        if(SRP_FanpaiManager.isWatchingFruit == false && SRP_FanpaiManager.FanpaiManagerInstance.isMoveing == false && noGetCi == false)
        GetComponent<AudioSource>().PlayOneShot(VoiceFile);

        //判断自己的FruitIndex是否和幸运Index相同
        if(FruitIndex == SRP_FanpaiManager.ForcusingIndex)
        {

            SRP_FanpaiManager.FanpaiManagerInstance.MoveForcusingPrefabToFace(gameObject,FruitIndex,true,SoundBiaoText);
        }
        else
        {
            SRP_FanpaiManager.FanpaiManagerInstance.MoveForcusingPrefabToFace(gameObject,FruitIndex,false,SoundBiaoText);
        }
    }

    public void PlayVoice()
    {
        GetComponent<AudioSource>().PlayOneShot(VoiceFile);
    }
}
