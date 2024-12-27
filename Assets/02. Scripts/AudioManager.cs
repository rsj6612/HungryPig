using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance; // 싱글톤 인스턴스. AudioManager를 전역적으로 사용 가능하게

    private void Awake()
    {
        if (instance == null)  // 싱글톤 초기화: 인스턴스가 없을 경우 현재 오브젝트를 인스턴스로 설정
        {
            instance = this;
        }
    }

    public void PlayClip(AudioClip clip, AudioSource source)
    {
        source.clip = clip;
        source.Play();
    }

    public void PlayRandomClip(AudioClip[] clips, AudioSource source) // 배열에서 랜덤하게 재생
    {
        int randomIndex = Random.Range(0, clips.Length);

        source.clip = clips[randomIndex];
        source.Play();
    }

    public void PlayBgmClip(AudioClip clip, AudioSource source)
    {
        source.clip = clip;
        source.Play();
        source.loop = true;
    }

    public void StopBgm(AudioSource source){
        source.Stop();
    }


}
