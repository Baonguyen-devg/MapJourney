using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    public static event Action OnLeftMouseDown;

    public Vector3 GetMousePosition() => Input.mousePosition;
    public Ray GetMousePointToRay() => Camera.main.ScreenPointToRay(Input.mousePosition);

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) OnLeftMouseDown?.Invoke();
    }
}
