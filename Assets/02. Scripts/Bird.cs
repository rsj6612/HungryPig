using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{
    #region Variables
    
    [Header("사운드, 이펙트")]
    [SerializeField] private AudioClip hitClip; // 충돌 시 재생되는 사운드
    [SerializeField] private ParticleSystem disappearEffect; // 소멸 효과
    
    // 기타 변수
    private Rigidbody2D rb; // Rigidbody2D 참조
    private CircleCollider2D circle; // CircleCollider2D 참조
    
    private bool hasBeenLaunched; // 발사 여부
    private bool shouldFaceVelDir; // 속도 방향을 향하도록 회전할지 여부
    
    private AudioSource audioSource; // 오디오 소스
    
    #endregion
    private void Awake()
    {
        // 필요한 컴포넌트 초기화
        rb = GetComponent<Rigidbody2D>();
        circle = GetComponent<CircleCollider2D>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        // 초기 설정
        rb.isKinematic = true; // Rigidbody2D 비활성화
        circle.enabled = false; // Collider 비활성화
    }

    private void FixedUpdate()
    {
        // 발사된 경우 속도 방향을 따라 회전
        if (hasBeenLaunched && shouldFaceVelDir)
        {
            transform.right = rb.velocity;    
        }
    }
    
    #region Launch 
    public void LaunchBird(Vector2 dir, float force)
    {
        // 새 발사
        rb.isKinematic = false;
        circle.enabled = true;
        
        rb.AddForce(dir * force, ForceMode2D.Impulse); // 힘 추가
        
        hasBeenLaunched = true;
        shouldFaceVelDir = true;
    }
    #endregion

    #region Collisionn
    private void OnCollisionEnter2D(Collision2D other)
    {
        // 충돌 시 처리
        shouldFaceVelDir = false; // 회전 멈춤
        AudioManager.instance.PlayClip(hitClip, audioSource); // 충돌 사운드 재생
        StartCoroutine(DestroyBird(3f)); // 지연 후 새 소멸
    }

    private IEnumerator DestroyBird(float delay)
    {
        // 일정 시간 후 새 소멸
        yield return new WaitForSeconds(delay);
        Instantiate(disappearEffect, transform.position, Quaternion.identity); // 소멸 효과 생성
        Destroy(gameObject); // 오브젝트 삭제
    }
    #endregion
}
