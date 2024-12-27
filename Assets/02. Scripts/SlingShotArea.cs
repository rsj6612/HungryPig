using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SlingShotArea : MonoBehaviour // 새총 주변 구역을 감지하도록
{
    [SerializeField] private LayerMask slingShotAreaMask;
    
    
    public bool isInSlingShotArea()
    {
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(InputManager.MousePosition);

        if (Physics2D.OverlapPoint(worldPos, slingShotAreaMask))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    
    
}
