using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 消消乐
/// </summary>
public class SRP_MH_SelectAndClear : SRP_MH_Master
{
    public Image BGImageObject;
    public SRP_MH_SelectAndClear_Element XiaoObject;
    public HorizontalLayoutGroup XiaoObjectHorLayout;
    [HideInInspector] public Vector2 ImageSize;
    [HideInInspector] public List<SRP_MH_SelectAndClear_Element> XiaoObjectList;
    [HideInInspector] public List<AudioClip> SoundCollection = new List<AudioClip>();
    [HideInInspector] public List<Sprite> ImageCollection = new List<Sprite>();
    [HideInInspector] public AudioSource SoundPlayer;

    public override void Initial(object SettingObject, SRP_MissionHelper_Master MASTER)
    {
        var SACSettingObject = SettingObject as SelectAndClear;
        Master = MASTER;
        BGImageObject.sprite = SACSettingObject.BGImage;
        SoundCollection = SACSettingObject.SoundCollection;
        ImageCollection = SACSettingObject.ImageCollection;
        ImageSize = SACSettingObject.ImageSize;
        Music = SACSettingObject.Music;

        SoundPlayer = gameObject.AddComponent<AudioSource>();
        XiaoObjectHorLayout.spacing = SACSettingObject.ImageSpace;
        
        InitialMusicPlayer(SACSettingObject.Music);

        XiaoObject.GetComponent<RectTransform>().sizeDelta = ImageSize;
        XiaoObject.GetComponent<Image>().sprite = ImageCollection[0];
        XiaoObject.Parent = this;
        XiaoObject.index = 0;
        XiaoObjectList.Add(XiaoObject);

        for(int i = 1; i < ImageCollection.Count; i++)//按数量生成
        {
            XiaoObjectList.Add(Instantiate(XiaoObject));
            XiaoObjectList[i].index = i;
            XiaoObjectList[i].gameObject.GetComponent<Image>().sprite = ImageCollection[i];
            XiaoObjectList[i].gameObject.transform.SetParent(XiaoObjectHorLayout.transform, false);//水平排序固定
        }

        StartCoroutine(ShowAll());
        StartCoroutine(CheckValid());
    }

    IEnumerator ShowAll()
    {
        foreach(var E in XiaoObjectList)//逐个设置位置并渐变显示
        {
            E.gameObject.GetComponent<RectTransform>().anchoredPosition -= new Vector2(Random.Range(-100f,100f), Random.Range(-300f,300f));
            E.StartShow();
            yield return new WaitForSeconds(0.5f);
        }
        yield break;
    }

    IEnumerator CheckValid()
    {
        while(true)
        {
            if(XiaoObjectList[XiaoObjectList.Count - 1] == null) break;
            yield return null;
        }

        CONTROL_PlayNext();
        yield break;
    }

    public void PlayClickSound(int index)
    {
        if(index <= SoundCollection.Count - 1)
        {
            SoundPlayer.PlayOneShot(SoundCollection[index]);
        }
    }
}
