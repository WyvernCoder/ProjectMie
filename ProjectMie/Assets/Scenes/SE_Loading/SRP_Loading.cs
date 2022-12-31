using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SRP_Loading : MonoBehaviour
{
    //public Slider ProcessBar;
    private string DownloadURL;
    private string savePath;
    private string filePath;
    string platformFullName = "";

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        APIManager.GENERATE_BODY();
        DownloadURL = APIManager.API.HttpServer + "/OnlineAssets/";
    }

    void Start()
    {
        // ����ƽ̨��������������ȷƽ̨�� AB �����硱ABC_Windows.ab����
        UpdatePlatformName();

        // ���ñ���Ŀ¼
        // persistentDataPath ���·���� Win �� Android ƽ̨���ǰ�ȫ�ģ����������Ȩ�޵������������棬���� Debug һ�¿������Ŀ¼ָ��������
        // ���ǰ����ص� AB �����������ȫĿ¼�µ� DownloadedContent �ļ����﷽����ࡣ
        savePath = Path.Combine(Application.persistentDataPath, "DownloadedContent");

        // �����ļ�·��
        // �ļ�·��ָ��Ҫ���� ab ����·����
        filePath = Path.Combine(savePath, APIManager.API.API_LoadScene_GetName() + ".ab");

        // ����Ŀ¼�Ƿ���ڣ���������ھʹ���һ��
        if (Directory.Exists(savePath) == false) Directory.CreateDirectory(savePath);

        // ����
        StartCoroutine(LoadStream());
    }

    IEnumerator LoadStream()
    {
        // ��ͼ�����ļ�
        yield return StartCoroutine(Download_IE());

        // ���غ���֤һ���ļ��Ƿ����
        if (File.Exists(filePath) == false)
        {
            Debug.LogError("�����ص��ļ������ڣ��������ع��̳������⣬���ڷ������˵���");
            APIManager.API.API_LoadScene_SetName("SE_Selector");
            var load = SceneManager.LoadSceneAsync("SE_Selector");
            while (load.isDone == false) yield return null;
            APIManager.API.API_IsGameLoading(false);
            APIManager.API.API_Transition_Stop();
            Destroy(gameObject);
            yield break;
        }

        // ����AB��
        yield return StartCoroutine(LoadAB_IE());

        // ����ؿ�
        var scene = SceneManager.LoadSceneAsync(APIManager.API.API_LoadScene_GetName());
        while (scene.isDone == false) yield return null;
        APIManager.API.API_Transition_Stop();
        APIManager.API.API_IsGameLoading(false);

        Destroy(gameObject);
        yield break;
    }

    IEnumerator Download_IE()
    {
        // �ж��ļ��Ƿ���ڣ�������ڣ���ֱ�� return ��
        if (File.Exists(filePath))
        {
            //Debug.Log("�ļ��Ѵ��ڣ�����Ҫ���ء�");
            yield break;
        }

        // �������ص�ַ
        var downloadURL = Path.Combine(DownloadURL, APIManager.API.API_LoadScene_GetName() + platformFullName + ".ab");
        downloadURL = downloadURL.Replace('\\', '/');   // HTML ��̫ϲ����б��

        // �����ص�ַ�����ļ�
        var downloader = UnityWebRequest.Get(downloadURL);  // ����GET���͵� WebRequest
        downloader.SendWebRequest();    // ��������
        while (downloader.isDone == false)     // ��������С�� 0.99 ����δ�������ʱ��ʼ��ѭ��
        {
            // ÿ��ѭ��������һ�½��������ò�ֵʵ������������
            //ProcessBar.value = Mathf.Lerp(ProcessBar.value, downloader.downloadProgress, Time.deltaTime);
            yield return null;
        }

        // ��ȡ���صĶ���������
        byte[] filebytes = downloader.downloadHandler.data;

        // ������ص�����С�� 1KB
        if (filebytes.Length <= 1024)
        {
            Debug.LogWarning("�����ļ������ڣ����ڷ������˵���");
            APIManager.API.API_LoadScene_SetName("SE_Selector");
            APIManager.API.API_LoadScene();
            Destroy(gameObject);
            yield break;
        }

        FileStream fs = new FileInfo(filePath).Create();    // �������ļ��������������������
        fs.Write(filebytes, 0, filebytes.Length);   // ������������д�뻺����
        fs.Flush(); // ������������д���ļ������������
        fs.Close(); // �ر�������

        yield break;
    }

    IEnumerator LoadAB_IE()
    {
        // �� AB �����ݼ��ؽ��ڴ�
        AssetBundle AB = AssetBundle.LoadFromFile(filePath);

        if (AB == null)
        {
            var newPath = Path.ChangeExtension(filePath, "error");
            if (File.Exists(newPath)) File.Delete(newPath);
            File.Move(filePath, newPath);
            Debug.LogError("�޷��򿪸�AB�����������ļ��𻵣��ѱ�Ǹ��ļ����������˵������ļ������´�����ʱ�������ء�");
            APIManager.API.API_LoadScene_SetName("SE_Selector");
            var load = SceneManager.LoadSceneAsync("SE_Selector");
            while (load.isDone == false) yield return null;
            APIManager.API.API_IsGameLoading(false);
            APIManager.API.API_Transition_Stop();
            Destroy(gameObject);
            yield break;
        }

        // �ж� AB �������޹ؿ�
        if (AB.GetAllScenePaths().Length == 0)
        {
            AB.Unload(true);    // ����ڴ��е� AB ����Դ
            Debug.LogError("AB ����û���κιؿ����������˵���");
            APIManager.API.API_LoadScene_SetName("SE_Selector");
            var load = SceneManager.LoadSceneAsync("SE_Selector");
            while (load.isDone == false) yield return null;
            APIManager.API.API_IsGameLoading(false);
            APIManager.API.API_Transition_Stop();
            Destroy(gameObject);
        }

        // ���� AB ������û��ָ���ؿ������AB����û��ָ���Ĺؿ����Ͷ�ȡ���е�һ���ؿ���
        if (Application.CanStreamedLevelBeLoaded(APIManager.API.API_LoadScene_GetName()) == false)
        {
            var sceneName = Path.GetFileNameWithoutExtension(AB.GetAllScenePaths()[0]);
            APIManager.API.API_LoadScene_SetName(sceneName);
        }

        yield break;
    }

    /// <summary>
    /// ����ƽ̨����
    /// AB ���Ƿ�ƽ̨�ģ����ʹ���Ҹ���AB����ű�����ô��������AB��������ƽ̨������Ϊ��β�����硱ABC_Windows.ab����
    /// </summary>
    void UpdatePlatformName()
    {
        switch (Application.platform)
        {
            case RuntimePlatform.WindowsPlayer: platformFullName = "Windows"; break;
            case RuntimePlatform.WindowsEditor: platformFullName = "Windows"; break;
            case RuntimePlatform.Android: platformFullName = "Android"; break;
            case RuntimePlatform.IPhonePlayer: platformFullName = "iOS"; break;
        }
        if (platformFullName != "") platformFullName = "_" + platformFullName;
    }
}
