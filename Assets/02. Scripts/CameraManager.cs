using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    [Header("카메라")]
    [SerializeField] private CinemachineVirtualCamera idleCam; // 대기 상태 카메라
    [SerializeField] private CinemachineVirtualCamera followCam; // 추적 카메라

    private void Awake()
    {
        SwitchToIdle(); // 게임 시작 시 대기 카메라 활성화
    }

    // 대기 카메라로 전환
    public void SwitchToIdle()
    {
        idleCam.enabled = true;
        followCam.enabled = false;
    }

    // 추적 카메라로 전환 (대상을 지정)
    public void SwitchToFollow(Transform follow)
    {
        followCam.Follow = follow; // 추적할 대상 설정
        followCam.enabled = true;
        idleCam.enabled = false;
    }
}