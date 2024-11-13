using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    public static event Action OnLeftMouseDown;
    public static event Action OnRightMouseDown;

    public Vector3 GetMousePosition() => Input.mousePosition;
    public Ray GetMousePointToRay() => Camera.main.ScreenPointToRay(Input.mousePosition);

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) OnLeftMouseDown?.Invoke();
        if (Input.GetMouseButtonDown(1)) OnRightMouseDown?.Invoke();
    }
}
