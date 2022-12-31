using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SRP_MH_Master : MonoBehaviour
{
    [HideInInspector] public SRP_MissionHelper_Master Master;
    [HideInInspector] public AudioClip Music = null;
    [HideInInspector] public AudioSource MusicPlayer;
    [HideInInspector] public CanvasGroup CG;

    public abstract void Initial(object SettingObject, SRP_MissionHelper_Master MASTER);

    void Awake()
    {
        CG = gameObject.AddComponent<CanvasGroup>();
    }

    public T Selector<T>(T A, T B, bool isA)
    {
        if(isA) return A;
        else return B;
    }

    public void InitialMusicPlayer(AudioClip Music)
    {
        if(Music != null)
        {
            MusicPlayer = gameObject.AddComponent<AudioSource>();
            MusicPlayer.loop = true;
            MusicPlayer.clip = Music;
            MusicPlayer.Play();
        }
    }

    public virtual void CONTROL_BackToMenu()
    {
        if(APIManager.API != null)
        APIManager.API.API_BackToMenu();
    }

    /// <summary>
    /// 播放下一章节；
    /// 请不要手动Destroy掉当前MH，交由Master处理即可。
    /// </summary>
    public virtual void CONTROL_PlayNext()
    {
        Master.PlayNext();
    }
}
