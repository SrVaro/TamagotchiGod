using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-1)]
public class InputManager : Singleton<InputManager>
{
    private TouchControls touchControls;
    
    private Camera mainCamera;

    #region Events
    public delegate void StartTouchEvent(Vector2 position, float time);
    public event StartTouchEvent OnStartTouch;

    public delegate void EndTouchEvent(Vector2 position, float time);
    public event EndTouchEvent OnEndTouch;
    #endregion


    private void Awake() {
        touchControls = new TouchControls();
        mainCamera = Camera.main;
    }

    private void OnEnable() {
        touchControls.Enable();
    }

    private void OnDisable() {
        touchControls.Disable();
    }

    private void Start() {
        touchControls.Touch.TouchPress.started += ctx => StartTouch(ctx);
        touchControls.Touch.TouchPress.canceled += ctx => EndTouch(ctx);

    }

    private void StartTouch(InputAction.CallbackContext context) {
        Debug.Log("Touch started " + touchControls.Touch.TouchPosition.ReadValue<Vector2>());
        if (OnStartTouch != null) OnStartTouch(Utils.ScreenToWorld(mainCamera, touchControls.Touch.TouchPosition.ReadValue<Vector2>()), (float)context.startTime);
    }

    private void EndTouch(InputAction.CallbackContext context) {
        Debug.Log("Touch ended " + touchControls.Touch.TouchPosition.ReadValue<Vector2>());
        if (OnEndTouch != null) OnEndTouch(Utils.ScreenToWorld(mainCamera, touchControls.Touch.TouchPosition.ReadValue<Vector2>()), (float)context.time);

    }

    public Vector2 PrimaryPosition() {
        return Utils.ScreenToWorld(mainCamera, touchControls.Touch.TouchPosition.ReadValue<Vector2>());
    }
}

