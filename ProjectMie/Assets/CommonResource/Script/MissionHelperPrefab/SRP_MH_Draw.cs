using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 画
/// </summary>
public class SRP_MH_Draw : SRP_MH_Master
{
    [HideInInspector]
    public List<SRP_LineDrawerWithMission> lineDrawer;
    [HideInInspector]
    public SRP_LineDrawerWithMission _lineDrawer;
    [HideInInspector]
    public Sprite BGImage;

    public override void Initial(object SettingObject, SRP_MissionHelper_Master MASTER)
    {
        Draw DrawSettingObject = SettingObject as Draw;
        Master = MASTER;
        BGImage = DrawSettingObject.BGImage;
        lineDrawer = DrawSettingObject.MissionList;
        Music = DrawSettingObject.Music;

        InitialMusicPlayer(DrawSettingObject.Music);

        StartCoroutine(CheckIsVaild());
    }

    IEnumerator CheckIsVaild()
    {
        int drawMissionIndex = -1;
        while(true)
        {
            if(_lineDrawer == null)
            {
                drawMissionIndex++;
                if(drawMissionIndex == lineDrawer.Count) break;
                _lineDrawer = Instantiate(lineDrawer[drawMissionIndex]);
                if(BGImage != null) _lineDrawer.BGImage.sprite = BGImage;//初始化背景图片
            }
            yield return null;
        }

        //print("画画完成");

        Master.PlayNext();
        yield break;
    }
}
