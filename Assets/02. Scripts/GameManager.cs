using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region Variables
    
    public static GameManager instance; // 싱글톤 인스턴스
    [Header("UI")]
    [SerializeField] private GameObject RestartScreen; // 재시작 화면
    [SerializeField] private GameObject LoseGameScreen; // 패배 화면
    [SerializeField] private Image nextLevelImage; // 다음 레벨 이미지

    [Header("스크립트")]
    [SerializeField] private SlingShot slingShot; // 새총 제어
    [SerializeField] private IconHandler iconHandler; // UI 아이콘 제어

    [Header("사운드")]
    private AudioSource audioSource; // 오디오 소스
    [SerializeField] private AudioClip BgmClip; // 배경 음악
    [SerializeField] private AudioClip WinClip; // 승리 효과음
    [SerializeField] private AudioClip LoseClip; // 패배 효과음

    [Header("기타 변수")]
    [SerializeField] private float secondsToWaitBeforeDie = 3f; // 적 사망 상태를 확인할 대기 시간
    public int maxShotNum = 3; // 최대 발사 가능 횟수
    private int usedShotNum; // 사용한 발사 횟수
    
    private List<Enemy> _enemies = new List<Enemy>(); // 적 목록
    
    #endregion
    
    private void Awake()
    {
        // 싱글톤 초기화
        if (instance == null)
        {
            instance = this;
        }

        // 필요한 컴포넌트 참조
        iconHandler = FindObjectOfType<IconHandler>();
        audioSource = GetComponent<AudioSource>();

        // 씬 내 모든 적 추가
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        foreach (var enemy in enemies)
        {
            _enemies.Add(enemy);
        }
    }

    private void Start()
    {
        AudioManager.instance.PlayBgmClip(BgmClip, audioSource);
    }

    #region Gameplay Logic
    public void UseShot()
    {
        usedShotNum++;
        iconHandler.UseShot(usedShotNum); // UI 업데이트
        CheckForLastShot();
    }

    public bool HasEnoughShot()
    {
        return usedShotNum < maxShotNum;
    }

    public void CheckForLastShot()
    {
        if (usedShotNum == maxShotNum)
        {
            StartCoroutine(CheckAfterWaitTime()); // 대기 후 게임 상태 확인
        }
    }

    private IEnumerator CheckAfterWaitTime()
    {
        yield return new WaitForSeconds(secondsToWaitBeforeDie);

        if (_enemies.Count == 0) // 모든 적이 처치되었으면
        {
            WinGame();
        }
        else
        {
            LoseGame();
        }
    }

    public void RemoveEnemy(Enemy enemy)
    {
        _enemies.Remove(enemy); // 목록에서 적 제거

        if (_enemies.Count == 0 && usedShotNum <= maxShotNum)
        {
            WinGame();
        }
    }
    #endregion

    #region Scene Management
    public void GoHome()
    {
        SceneManager.LoadScene(0);
    }

    public void RestartGame()
    {
        DOTween.Clear(true); // DOTween 클리어
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // 현재 씬 재로드
    }

    public void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); // 다음 씬 로드
    }
    #endregion

    #region Win/Lose
    private void WinGame()
    {
        if (RestartScreen.activeSelf) return; // 이미 처리 중일 경우 종료

        RestartScreen.SetActive(true); // 재시작 화면 표시
        slingShot.enabled = false; // 새총 비활성화

        int currentScene = SceneManager.GetActiveScene().buildIndex;
        int maxLevel = SceneManager.sceneCountInBuildSettings;

        nextLevelImage.enabled = currentScene + 1 < maxLevel; // 다음 레벨 활성화 여부

        AudioManager.instance.StopBgm(audioSource);
        AudioManager.instance.PlayClip(WinClip, audioSource);
    }

    private void LoseGame()
    {
        if (RestartScreen.activeSelf) return; // 이미 처리 중일 경우 종료

        LoseGameScreen.SetActive(true); // 패배 화면 표시
        slingShot.enabled = false; // 새총 비활성화

        AudioManager.instance.StopBgm(audioSource);
        AudioManager.instance.PlayClip(LoseClip, audioSource);
    }
    #endregion
}
