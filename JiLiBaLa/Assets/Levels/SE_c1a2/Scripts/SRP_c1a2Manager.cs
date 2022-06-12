using System.Runtime.Serialization.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using UnityEngine.UI;
using Baidu.Aip.Speech;

public class SRP_c1a2Manager : MonoBehaviour
{
    public GameObject EnemyPrefab;
    public List<GameObject> EnemyPrefabList = new List<GameObject>();
    public GameObject MicStatusText;
    public GameObject MicUIObject;
    public bool isGameOver = false;
    public Vector2 EnemySpawnLocationY = new Vector2(0.85f,-0.79f);
    public int TargetScore = 50;
    public float EnemySpawnDelay = 3f;
    public float EnemySpeed = 0.1f;
    private int Score = 0;
    // private Vector2 PanderTigerStatus = new Vector2(1f,1f);//记录熊猫老虎谁伤亡了
    private Vector2Int pandertigerstatus = new Vector2Int(1,1);
    public GameObject TigerPrefab;
    public GameObject PanderPrefab;
    public GameObject MissingCompletePrefab;
    private bool isRecording = false;
    public Vector2Int PanderTigerStatus
    {
        get
        {
            return pandertigerstatus;
        }
        set
        {
            pandertigerstatus = value;
            if(pandertigerstatus.x == 0 && pandertigerstatus.y == 0) 
            {
                //!MISSION FAILED
                Destroy(GetComponent<AudioSource>());
                foreach(GameObject GO in EnemyPrefabList) if(GO != null) GO.GetComponent<SRP_eCharacter_c1a2>().BeingKilled();
                GameManager.Instance.FailedTheMission();
                isGameOver = true;
            }
        }
    }



    

    void Start() => StartCoroutine(EnemySpawner());
    public void AddScore(int score)
    {
        if(!isGameOver) Score+=score;
    }
    IEnumerator EnemySpawner()
    {   //!READY GO
        yield return new WaitForSeconds(3);
        GameObject EnemyPRB;
        while(Score < TargetScore)
        {
            EnemyPRB = Instantiate(EnemyPrefab);
            EnemyPRB.transform.SetParent(gameObject.transform,true);
            EnemyPrefabList.Add(EnemyPRB);
            switch(Random.Range(0,2))
            {
                case 0:
                {
                    EnemyPRB.transform.position = new Vector3(10.9f,EnemySpawnLocationY.x,0);
                    break;
                }
                case 1:
                {
                    EnemyPRB.transform.position = new Vector3(10.9f,EnemySpawnLocationY.y,0);
                    break;
                }
            }
            yield return new WaitForSeconds(EnemySpawnDelay);
        }
        //TODO:CompleteGame
        Destroy(TigerPrefab,0.5f);
        Destroy(PanderPrefab,0.5f);
        Destroy(MicUIObject);
        Destroy(GetComponent<AudioSource>());
        Instantiate(MissingCompletePrefab);
        yield break;
    }

    List<string> SimlarText_Pander = new List<string>{("pander"),("thunder"),("ponder"),("under"),("planned are"),("plunder"),("wonder")};
    List<string> SimlarText_Tiger = new List<string>{("tiger"),("hi girl"),("tigers"),("Hager"),("thank God"),("hi Kurt"),("tigert")};
    void FixedUpdate()
    {
        if (!isGameOver)
        {
            foreach (string TEXT in SimlarText_Pander)
            {
                if (SpeakingText == TEXT)
                {
                    SpeakingText = "";//归零
                    MicStatusText.GetComponent<Text>().text = "说的好，小熊猫射了！";
                    PanderPrefab.GetComponent<SRP_Character_c1a2>().FIRE_ONCE();
                }
            }
            foreach (string TEXT in SimlarText_Tiger)
            {
                if (SpeakingText == TEXT)
                {
                    SpeakingText = "";//归零
                    MicStatusText.GetComponent<Text>().text = "说的好，小老虎射了！";
                    TigerPrefab.GetComponent<SRP_Character_c1a2>().FIRE_ONCE();
                }
            }
        }
        else MicStatusText.GetComponent<Text>().text = "熊猫老虎死了！";
    }











    /// <summary>
    /// 百度语音API部分
    /// 最终识别结果是SpeakingText
    /// </summary>
    public string API_ID = "25540429";
    public string API_KEY = "sOXndEAjLr5iGl5ueLG5HS8r";
    public string SECRET_KEY = "MTWHQpSEEIxqT1tIj51uD0Zz2PEC44zh";
    AudioClip RecordAudioClipAC;//录音数据
    int iRecordTime = 2;//一次录制的长度
    string SpeakingText;//识别出来的文字
    public string sMicrophoneName = "";
    Asr clientASR;

    void Awake()
    {
        /// <summary>
        /// 百度语音
        /// </summary>
        /// <returns></returns>
        clientASR = new Baidu.Aip.Speech.Asr(API_ID, API_KEY, SECRET_KEY);
        clientASR.Timeout = 160000;//超时时间
    }
    public void StartRecord()
    {
        if(!isRecording)
        {
            isRecording = !isRecording;
            StartCoroutine(IE_StartRecord());
        }
    }
    IEnumerator IE_StartRecord()
    {
        if (isGameOver) yield break;
        MicStatusText.GetComponent<Text>().text = "正在录音...";
        GetComponent<AudioSource>().volume *= 0.05f; //录音时降低背景音乐音量
        RecordAudioClipAC = Microphone.Start(sMicrophoneName, false, iRecordTime, 16000);
        int timer = 0;
        while(timer <= iRecordTime)
        {
            timer++;
            yield return new WaitForSeconds(1);
        }
        StartCoroutine(IE_EndRecord());
        yield break;
    }
    IEnumerator IE_EndRecord()
    {
        Microphone.End(sMicrophoneName);//停止录音
        isRecording = !isRecording;
        if(GetComponent<AudioSource>() != null)GetComponent<AudioSource>().volume = 1f; //录制完成后恢复音量
        if(RecordAudioClipAC == null )//啥也没录到
        {
            MicStatusText.GetComponent<Text>().text = "触摸任意区域以录音";
            yield break;
        }
        //音频转字节
        float[] samples = new float[16000 * iRecordTime * RecordAudioClipAC.channels];
        RecordAudioClipAC.GetData(samples, 0);
        short[] sampleshort = new short[samples.Length];
        for (int i = 0; i < samples.Length; i++) sampleshort[i] = (short)(samples[i] * short.MaxValue); 
        byte[] data = new byte[samples.Length * 2];
        System.Buffer.BlockCopy(sampleshort, 0, data, 0, data.Length);
        var options = new Dictionary<string, object>//英文语音识别
        {
            {"dev_pid",1737 }
        };
        var BaiduResult = clientASR.Recognize(data, "pcm", 16000, options);

        /// <summary>
        /// JSON解析、反序列化
        /// </summary>
        /// <value></value>
        RootObject RB = JsonConvert.DeserializeObject<RootObject>(BaiduResult.ToString());
        if(RB.result[0] != null) SpeakingText = RB.result[0];
        else 
        {
            if (!isGameOver) MicStatusText.GetComponent<Text>().text = "触摸任意区域以录音";
        }
        print(SpeakingText);
        yield break;
    }
    class RootObject//JSON 结构
    {
        public string corpus_no;
        public string err_msg;
        public string err_no;
        public List<string> result;
        public string sn;
    };
}
