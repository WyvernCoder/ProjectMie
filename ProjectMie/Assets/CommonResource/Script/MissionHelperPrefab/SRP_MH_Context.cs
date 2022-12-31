using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
/// <summary>
/// 对话
/// </summary>
public class SRP_MH_Context : SRP_MH_Master
{
    public TMP_Text ParagraphObject;
    public TMP_Text CharacterNickObject;
    public Image LeftCharacterImageObject;
    public Image RightCharacterImageObject;
    public Image BGImageObject;
    public RectTransform Tag;
    public RectTransform Tag_LeftAnchor;
    public RectTransform Tag_RightAnchor;
    public float Tag_MoveSpeed = 0.1f;
    private Vector2 Tag_FinalLoc;

    //以下内容均由Master赋值而来
    [HideInInspector]
    public List<string> Paragraph = new List<string>();
    [HideInInspector]
    public List<Sprite> LeftCharacterImage = new List<Sprite>();
    [HideInInspector]
    public List<Sprite> RightCharacterImage = new List<Sprite>();
    [HideInInspector]
    public string CharacterNick;
    [HideInInspector]
    public List<AudioClip> Voice = new List<AudioClip>();
    [HideInInspector]
    public AudioClip WordPop;
    [HideInInspector]
    public Sprite BGImage;

    [HideInInspector] public AudioSource VoicePlayer;
    [HideInInspector] public int CurrentProcessIndex = -1;//当前对话
    [HideInInspector] public int MaxProcessIndex;//该对话的总对话数

    private List<string> NickList;//人物名单，根据奇偶判断人物左右侧
    private float averageAlpha = 1f;
    private CanvasGroup lCG;
    private CanvasGroup rCG;

    public override void Initial(object SettingObject, SRP_MissionHelper_Master MASTER)
    {
        Context ContextSettingObject = SettingObject as Context;
        Master = MASTER;
        Paragraph = ContextSettingObject.Paragraph;
        LeftCharacterImage = ContextSettingObject.LeftCharacterImage;
        RightCharacterImage = ContextSettingObject.RightCharacterImage;
        Voice = ContextSettingObject.Voice;
        Music = ContextSettingObject.Music;
        BGImage = ContextSettingObject.BGImage;
        WordPop = ContextSettingObject.WordPop;

        CurrentProcessIndex = 0;
        MaxProcessIndex = Paragraph.Count - 1;

        VoicePlayer = gameObject.AddComponent<AudioSource>();
        MusicPlayer = gameObject.AddComponent<AudioSource>();

        VoicePlayer.loop = false;
        MusicPlayer.loop = true;
        VoicePlayer.playOnAwake = false;
        MusicPlayer.playOnAwake = false;
        
        BGImageObject.sprite = BGImage;//初始化背景图

        InitialMusicPlayer(ContextSettingObject.Music);

        if(MaxProcessIndex == -1)
        {
            print("函数终止，总话的数量为0？");
            return;
        }

        //初始化人物名单，用于配合atLeft函数以分出人物的左右
        NickList = GetCharacterNameList(Paragraph);
        
        //初始化Tag位置
        Tag_FinalLoc = Tag_LeftAnchor.anchoredPosition;
 
        lCG = LeftCharacterImageObject.gameObject.AddComponent<CanvasGroup>();
        rCG = RightCharacterImageObject.gameObject.AddComponent<CanvasGroup>();

        StartCoroutine("Play");

    }

    private IEnumerator Play()
    {
        if(CurrentProcessIndex > MaxProcessIndex)
        {
            //print("下一页已超出课程上限，函数已中断，正在进行下一项任务。");
            Master.PlayNext();
            yield break;
        }

        ParagraphObject.text = "";//初始化对话栏

        //解析对话
        CharacterNick = Paragraph[CurrentProcessIndex].Split('：')[0];   //提取名字
        Paragraph[CurrentProcessIndex] = Paragraph[CurrentProcessIndex].Split('：')[1];//提取对话
        CharacterNickObject.text = CharacterNick;//设置当前角色名
        
        //使Tag左右移动并使相对角色变暗
        if(atLeft(CharacterNick) == 0) 
        {   //聚焦在左侧
            SetTagMoveLeft();
            LeftCharacterImageObject.color = Color.white;
            RightCharacterImageObject.color = new Color(1,1,1,0);
        }
        else 
        {   //聚焦在右侧
            SetTagMoveLeft(false);
            LeftCharacterImageObject.color = new Color(1,1,1,0);
            RightCharacterImageObject.color = Color.white;
        }

        //切换角色图片，因为要等渐变值到一半才换图片，所以需要弄个协程去等
        StartCoroutine(ChangeAverageImage_IE());


        if(Voice.Count >= CurrentProcessIndex + 1)//播放语音
        {
            if(Voice[CurrentProcessIndex] != null) 
                PlayVoice(Voice[CurrentProcessIndex]);
                //VoicePlayer.PlayOneShot(Voice[CurrentProcessIndex]);
        }

        List<string> SingleWord = new List<string>();//将一段话拆成一个字
        for(int i = 0; i < Paragraph[CurrentProcessIndex].Length; i++)
        {
            if(i == Paragraph[CurrentProcessIndex].Length) break;
            SingleWord.Add(Paragraph[CurrentProcessIndex].Substring(i, 1));
        }

        foreach(string S in SingleWord)//逐字填充
        {
            ParagraphObject.text += S;
            if(WordPop != null) MusicPlayer.PlayOneShot(WordPop);
            yield return new WaitForSeconds(0.05f);
        }

        yield break;
    }

    IEnumerator ChangeAverageImage_IE()
    {
        if (CurrentProcessIndex + 1 <= LeftCharacterImage.Count)
        {
            if (LeftCharacterImageObject.sprite != LeftCharacterImage[CurrentProcessIndex]) averageAlpha = 0f;
            while(true)
            {
                if(averageAlpha >= 0.5f)//0.5是渐变到一半时
                    {
                        LeftCharacterImageObject.sprite = LeftCharacterImage[CurrentProcessIndex];
                        break;
                    }
                yield return null;
            }
        }   


        if (CurrentProcessIndex + 1 <= RightCharacterImage.Count)
        {
            if (RightCharacterImageObject.sprite != RightCharacterImage[CurrentProcessIndex]) averageAlpha = 0f;
            while(true)
            {
                if(averageAlpha >= 0.5f)//0.5是渐变到一半时
                    {
                    RightCharacterImageObject.sprite = RightCharacterImage[CurrentProcessIndex];
                        break;
                    }
                yield return null;
            }
        }
        yield break;
    }

    void FixedUpdate()
    {
        if (Tag == null) return;
        Tag.anchoredPosition = Math_LerpVector(Tag.anchoredPosition, Tag_FinalLoc, Tag_MoveSpeed);

        //0.9+0.1*sin((6.28*x + 1.57))
        if(lCG == null || rCG == null) return;
        switch(atLeft(CharacterNick))
        {
            case -1 : break;
            case 0: lCG.alpha = 0.8f + 0.2f * Mathf.Sin((float)(2 * Mathf.PI * averageAlpha + 0.5 * Mathf.PI));break;
            case 1: rCG.alpha = 0.8f + 0.2f * Mathf.Sin((float)(2 * Mathf.PI * averageAlpha + 0.5 * Mathf.PI));break;
        }
        if (averageAlpha < 1.1f) averageAlpha += 0.05f;
    }

    public override void CONTROL_PlayNext()
    {
        CurrentProcessIndex++;
        StopAllCoroutines();
        StartCoroutine("Play");
    }

    /// <summary>
    /// 提取所有角色名称到一个List里，不重样的。
    /// </summary>
    /// <param name="ParagraphList"></param>
    /// <returns></returns>
    List<string> GetCharacterNameList(List<string> ParagraphList)
    {
        List<string> cache = new List<string>();
        List<string> final = new List<string>();
        foreach(string S in ParagraphList) cache.Add(S.Split('：')[0]);
        foreach(string S in cache) if(final.FindAll(s => s == S).Count == 0) final.Add(S); 
        return final;
    }

    /// <summary>
    /// 判断角色是在左还是右，
    /// 返回1表示右，0表示左，-1表示名称不存在
    /// </summary>
    /// <param name="Name"></param>
    /// <returns></returns>
    int atLeft(string Name)
    {
        if(NickList.IndexOf(Name) == -1)
        {
            print("角色名称不存在。");
            return -1;
        }
        if(NickList.IndexOf(Name) == 0) return 0;
        if(NickList.IndexOf(Name) % 2 == 0) return 0;
        else return 1;
    }

    /// <summary>
    /// 控制Tag的左右移动
    /// </summary>
    /// <param name="isLeft"></param>
    void SetTagMoveLeft(bool isLeft = true)
    {
        if(isLeft == true) Tag_FinalLoc = Tag_LeftAnchor.anchoredPosition;
        else Tag_FinalLoc = Tag_RightAnchor.anchoredPosition;
    }

    void PlayVoice(AudioClip AC)
    {
        VoicePlayer.Pause();
        VoicePlayer.clip = AC;
        VoicePlayer.Play();
    }








    Vector3 Math_LerpVector(Vector3 A, Vector3 B, float C)
    {
        return new Vector3(Mathf.Lerp(A.x, B.x, C), Mathf.Lerp(A.y, B.y, C), Mathf.Lerp(A.z, B.z, C));
    }
    Vector2 Math_LerpVector(Vector2 A, Vector2 B, float C)
    {
        return new Vector2(Mathf.Lerp(A.x, B.x, C), Mathf.Lerp(A.y, B.y, C));
    }
}
