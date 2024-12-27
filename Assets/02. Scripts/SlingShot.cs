using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class SlingShot : MonoBehaviour {
    #region Variables
    [Header("라인렌더러")]
    [SerializeField] private LineRenderer leftLine;
    [SerializeField] private LineRenderer rightLine;

    [Header("궤적 세팅")]
    [SerializeField] private GameObject trajectoryDotPrefab;
    [SerializeField] private int maxTrajectorySteps = 20; // 최대 궤적 점 개수
    [SerializeField] private float timeStep = 0.1f; // 궤적 계산 간격
    [SerializeField] private float gravityScale = 1f; // 궤적 계산 중 중력 계수
    private List<GameObject> trajectoryDots = new List<GameObject>(); // 궤적 점 목록

    [Header("트랜스폼")]
    [SerializeField] private Transform leftStartPosition; // 왼쪽 라인의 시작 위치
    [SerializeField] private Transform rightStartPosition; // 오른쪽 라인의 시작 위치
    [SerializeField] private Transform centerPosition; // 새총 중심 위치
    [SerializeField] private Transform idlePosition; // 대기 상태 위치
    [SerializeField] private Transform elasticTransform; // 탄성 애니메이션 트랜스폼

    [Header("새총 변수")]
    [SerializeField] private float maxDistance = 3.5f; // 새총 최대 당김 거리
    [SerializeField] private float shotForce = 5f; // 발사 힘
    [SerializeField] private float timeBetweenBirdRespawn = 3f; // 새 리스폰 간격
    [SerializeField] private float elasticDivide = 1.2f; // 탄성 복구 속도 계산용 계수
    [SerializeField] private AnimationCurve elasticCurve; // 줄 복구 애니메이션 곡선
    [SerializeField] private float maxAnimTime = 1f; // 애니메이션 최대 지속 시간

    [Header("스크립트")]
    [SerializeField] private SlingShotArea slingShotArea; 
    [SerializeField] private CameraManager cameramanager; 

    [Header("사운드")]
    [SerializeField] private AudioClip elasticPulledClip; // 줄 당길 때 소리
    [SerializeField] private AudioClip[] elasticReleasedClips; // 줄 놓을 때 소리
    private AudioSource audioSource; 

    [Header("적")]
    [SerializeField] private Bird birdPrefab;
    [SerializeField] private float birdPositionOffset = 2f; // 새 위치 보정값

    // 기타 변수
    private Vector2 slingShotLinePosition; // 새총 당긴 위치
    private Vector2 direction; // 발사 방향
    private Vector2 directionNormalized; // 정규화된 발사 방향
    private bool clickedArea; // 새총 영역 클릭 여부
    private bool birdOnSlingShot; // 새총에 새가 있는지 여부
    private Bird SpawnedBird; // 현재 생성된 새
    
    #endregion
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        leftLine.enabled = false;
        rightLine.enabled = false;
        
        SpawnBird(); // 게임 시작 시 첫 새 생성
    }
    private void Update()
    {   
        // 새총 영역 클릭
        if (InputManager.wasLeftMouseButtonPressed && slingShotArea.isInSlingShotArea())
        {
            clickedArea = true;

            if (birdOnSlingShot)
            {
                AudioManager.instance.PlayClip(elasticPulledClip, audioSource);
            }
        }
        
        // 새총 드래그
        if (InputManager.isLeftMousePressed && clickedArea && birdOnSlingShot) 
        {
            DrawSlingShot(); // 라인 업데이트
            PositionAndRotationBird(); // 새 위치 및 방향 설정
            
            Vector2 trajectoryStartPoint = (Vector2)slingShotLinePosition + directionNormalized * birdPositionOffset;
            DrawTrajectory(trajectoryStartPoint, direction * shotForce);
            // DrawTrajectory(centerPosition.position, direction * shotForce); // 궤적 계산
        }

        // 발사
        if (InputManager.wasLeftMouseButtonReleased && birdOnSlingShot && clickedArea) 
        {
            if (GameManager.instance.HasEnoughShot())
            {
                clickedArea = false;
                birdOnSlingShot = false;
                
                SpawnedBird.LaunchBird(direction, shotForce); // 새 발사
                cameramanager.SwitchToFollow(SpawnedBird.transform); // 카메라 추적 전환

                AudioManager.instance.PlayRandomClip(elasticReleasedClips, audioSource);
                
                GameManager.instance.UseShot();
                AnimateSlingShot();

                // 추가 횟수가 있으면 새 리스폰 예약
                if (GameManager.instance.HasEnoughShot())
                {
                    StartCoroutine(SpawnBirdAfterTime());

                }
            }

        }
    }
    
    #region SlingShot Methods

    // 새총 라인 및 발사 방향
    private void DrawSlingShot() {
        Vector3 touchPos = Camera.main.ScreenToWorldPoint(InputManager.MousePosition);
        slingShotLinePosition = centerPosition.position + Vector3.ClampMagnitude(touchPos - centerPosition.position, maxDistance);
        SetLines(slingShotLinePosition);
        
        direction = (Vector2)centerPosition.position - slingShotLinePosition; // 캐스팅
        directionNormalized = direction.normalized;
    }

    // 궤적 점 그리기
    public void DrawTrajectory(Vector2 startPoint, Vector2 launchVelocity)
    {
        Vector2 gravity = Physics2D.gravity * gravityScale;
        for (int i = 0; i < maxTrajectorySteps; i++)
        {
            float t = i * timeStep;
            Vector2 currentPoint = startPoint + launchVelocity * t + 0.5f * gravity * t * t;

            if (i >= trajectoryDots.Count)
            {
                GameObject dot = Instantiate(trajectoryDotPrefab, currentPoint, Quaternion.identity);
                trajectoryDots.Add(dot);
            }
            else
            {   
                trajectoryDots[i].transform.position = currentPoint;
                trajectoryDots[i].SetActive(true);
            }
        }

        // 초과된 점 비활성화
        for (int i = trajectoryDots.Count - 1; i >= maxTrajectorySteps; i--)
        {
            trajectoryDots[i].SetActive(false);
        }
    }
    
    // 궤적 점 초기화
    public void ClearTrajectory()
    {
        foreach (GameObject dot in trajectoryDots)
        {
            dot.SetActive(false);
        }
    }

    // 라인 설정
    private void SetLines(Vector2 position)
    {
        if (!leftLine.enabled && !rightLine.enabled)
        {
            leftLine.enabled = true;
            rightLine.enabled = true;
        }
        
        leftLine.SetPosition(0, position);
        leftLine.SetPosition(1, leftStartPosition.position);
        
        rightLine.SetPosition(0, position);
        rightLine.SetPosition(1, rightStartPosition.position);
    }
    #endregion
    
    #region Bird Methods

    // 새 생성
    private void SpawnBird()
    {
        elasticTransform.DOComplete(); // 애니메이션 초기화
        SetLines(idlePosition.position);
        
        Vector2 dir = (centerPosition.position - idlePosition.position).normalized;
        Vector2 spawnPos = (Vector2)idlePosition.position + dir * birdPositionOffset;
        
        SpawnedBird = Instantiate(birdPrefab, spawnPos, Quaternion.identity);
        SpawnedBird.transform.right = dir;
        
        birdOnSlingShot = true;
    }
    
    // 새 위치 및 방향 설정
    private void PositionAndRotationBird()
    {
        SpawnedBird.transform.position = slingShotLinePosition + directionNormalized * birdPositionOffset;
        SpawnedBird.transform.right = directionNormalized;
    }

    // 일정 시간 후 새 리스폰
    private IEnumerator SpawnBirdAfterTime()
    {
        yield return new WaitForSeconds(timeBetweenBirdRespawn);

        SpawnBird();
        
        cameramanager.SwitchToIdle();
    }
    #endregion

    #region SlingShot Anim

    // 새총 복구 애니메이션
    private void AnimateSlingShot()
    {
        elasticTransform.position = leftLine.GetPosition(0);
        float dist = Vector2.Distance(elasticTransform.position, centerPosition.position);
        float time = dist / elasticDivide;
        
        elasticTransform.DOMove(centerPosition.position, time).SetEase(elasticCurve);
        StartCoroutine(AnimateSlingShotLines(elasticTransform, time));

    }

    // 새총 라인 복구 애니메이션
    private IEnumerator AnimateSlingShotLines(Transform trans, float time)
    {
        float elapsedTime = 0f;
        while (elapsedTime < time && elapsedTime < maxAnimTime)
        {
            elapsedTime += Time.deltaTime;
            
            SetLines(trans.position);
            
            yield return null;
        }
    }

    #endregion
}
