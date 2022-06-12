using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// SRP_PlayRandomSound
/// 作用：实例化后会随机播放声音，用于按钮声音。
/// 用法：依靠GameManager使用。
/// </summary>

public class SRP_PlayRandomSound : MonoBehaviour
{
    [Tooltip("该Prefab的自毁时间。应长于声音播放时间")]
    public float DestroyAfterTime = 1.5f;
    [Tooltip("音频列表，实例化后会随机播放任意一个。")]
    public List<AudioClip> ACList = new List<AudioClip>();
    void Awake() => Destroy(gameObject,DestroyAfterTime);
    void Start() => GetComponent<AudioSource>().PlayOneShot(ACList[Random.Range(0,ACList.Count)]);
}
