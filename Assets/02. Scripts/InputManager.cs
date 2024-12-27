using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// 유니티 INPUT SYSTEM 관리
public class InputManager : MonoBehaviour
{
    
    public static PlayerInput playerInput; // 싱글톤 인스턴스. 전역적으로 쓸 수 있게
    
    private InputAction mousePositionAction;
    private InputAction mouseAction;

    public static Vector2 MousePosition;
    
    public static bool wasLeftMouseButtonPressed;
    public static bool wasLeftMouseButtonReleased;
    public static bool isLeftMousePressed;
    
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        
        mousePositionAction = playerInput.actions["MousePosition"];
        mouseAction = playerInput.actions["Mouse"];
    }

    private void Update()
    {
        // 마우스 위치 업데이트
        MousePosition = mousePositionAction.ReadValue<Vector2>(); 
        
        // 마우스 클릭 상태 업데이트
        wasLeftMouseButtonPressed = mouseAction.WasPressedThisFrame(); // 버튼이 눌린 순간
        wasLeftMouseButtonReleased = mouseAction.WasReleasedThisFrame(); // 버튼이 떼진 순간
        isLeftMousePressed = mouseAction.IsPressed(); // 버튼이 눌려있는 상태
        
    }
}
