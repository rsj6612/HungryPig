using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainManager : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private AudioClip MainBgm;  
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
        AudioManager.instance.PlayBgmClip(MainBgm, audioSource);
    }

    public void StartGame()
    {
        AudioManager.instance.StopBgm(audioSource);
        
        SceneManager.LoadScene("Level1");
    }
}
