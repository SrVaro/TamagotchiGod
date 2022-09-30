using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GravityDevice : MonoBehaviour
{

    private InputManager inputManager;

    private Vector2 startPosition;
    private float startTime;
    private Vector2 endPosition;
    private float endTime;

    private Coroutine coroutine;

    private void Awake() {
        inputManager = InputManager.Instance;
    }

    private void OnEnable() {
        inputManager.OnStartTouch += Click;
        inputManager.OnEndTouch += Release;
        

    }

    // Update is called once per frame
    private void OnDisable()
    {
        inputManager.OnStartTouch -= Click;
        inputManager.OnEndTouch -= Release;
        
    }

    public void Click(Vector2 mousePos, float time) {
        coroutine = StartCoroutine(FollowPosition());
    }

    private IEnumerator FollowPosition() {
        while(true) {
            transform.position = new Vector3 (Mathf.Clamp(inputManager.PrimaryPosition().x, -2, 2), transform.position.y);
            yield return null;
        }
    }
    
    public void Release(Vector2 mousePos, float time) {        
        StopCoroutine(coroutine);
    }
}
