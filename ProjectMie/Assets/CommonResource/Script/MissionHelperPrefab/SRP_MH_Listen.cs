using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 听
/// </summary>
public class SRP_MH_Listen : SRP_MH_Master
{
    public Image BGImageObject;
    public TMPro.TMP_Text FontObject;
    public TMPro.TMP_Text PinYinObject;

    [HideInInspector] public AudioSource VoicePlayer;
    [HideInInspector] public AudioClip Voice;
    [HideInInspector] public Sprite BGImage;
    [HideInInspector] public string FontText;
    [HideInInspector] public string PinYinText;


    public override void Initial(object SettingObject, SRP_MissionHelper_Master MASTER)
    {
        var ListenSettingObject = SettingObject as Listen;
        Master = MASTER;
        Music = ListenSettingObject.Music;
        Voice = ListenSettingObject.Voice;
        FontText = ListenSettingObject.Word;
        PinYinText = ListenSettingObject.PinYin;
        BGImage = ListenSettingObject.BGImage;

        BGImageObject.sprite = BGImage;
        FontObject.text = FontText;
        PinYinObject.text = PinYinText;

        InitialMusicPlayer(ListenSettingObject.Music);

        if(Voice != null)
        {
            VoicePlayer = gameObject.AddComponent<AudioSource>();
            Invoke("CONTROL_PlayVoice", 2f);//播放声音
        }
    }

    public void CONTROL_PlayVoice()
    {
        if(Voice == null) return;
        VoicePlayer.PlayOneShot(Voice);
    }
}
