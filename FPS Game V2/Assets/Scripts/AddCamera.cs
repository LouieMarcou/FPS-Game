using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddCamera : UnityEngine.InputSystem.PlayerInput
{
    // Start is called before the first frame update
    Camera camera2
    {
        get => g_Camera;
        set => g_Camera = value;
    }
    [SerializeField] internal Camera g_Camera;
}
