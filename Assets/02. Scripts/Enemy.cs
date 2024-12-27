using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float maxHealth = 3f;
    [SerializeField] private float damageThreshold = 0.2f;
    [SerializeField] private GameObject dieEffect;
    private float currentHealth;
    
    [SerializeField] AudioClip Popclip;
    
    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        GameManager.instance.RemoveEnemy(this); // 게임 매니저 스크립트의 적 리스트에서 제거
        
        Instantiate(dieEffect, transform.position, Quaternion.identity); // 죽는 곳에 이펙트 발생
        
        AudioSource.PlayClipAtPoint(Popclip, transform.position); // 오디오 소스 추가안하고 클립 재생하는 법
        
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        float impactVelocity = other.relativeVelocity.magnitude; // 충돌 속도 계산

        if (impactVelocity > damageThreshold) // 최소 속도 초과할 경우 충돌 데미지 적용
        {
            TakeDamage((impactVelocity));
        }
    }
}
