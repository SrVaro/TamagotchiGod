using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeDetection : MonoBehaviour
{

    public float minimunDistance = .2f;
    public float maximumTime = 1f;
    
    public float directionTreshold = 0.9f;

    public GameObject trail;

    private Coroutine coroutine;

    // ------------ //
    private InputManager inputManager;

    // ------------ //

    private Vector2 startPosition;
    private float startTime;
    private Vector2 endPosition;
    private float endTime;

    private void Awake() {
        inputManager = InputManager.Instance;
    }

    private void OnEnable() {
        inputManager.OnStartTouch += SwipeStart;
        inputManager.OnEndTouch += SwipeEnd;
    }

    // Update is called once per frame
    private void OnDisable()
    {
        inputManager.OnStartTouch -= SwipeStart;
        inputManager.OnEndTouch -= SwipeEnd;
    }

    public void SwipeStart(Vector2 position, float time) {
        startPosition = position;
        startTime = time;
        trail.SetActive(true);
        trail.transform.position = position;
        coroutine = StartCoroutine(Trail());
    }

    private IEnumerator Trail() {
        while(true) {
            trail.transform.position = inputManager.PrimaryPosition();
            yield return null;
        }
    }

    public void SwipeEnd(Vector2 position, float time) {
        trail.SetActive(false);
        StopCoroutine(coroutine);
        endPosition = position;
        endTime = time;
        //DetectSwipe();
    }

   /*  private void DetectSwipe() {
        if (Vector3.Distance(startPosition, endPosition) >= minimunDistance &&
            (endTime - startTime) <= maximumTime) {
                Debug.DrawLine(startPosition, endPosition, Color.red, 5);
                Vector3 direction = endPosition - startPosition;
                Vector2 direction2D = new Vector2(direction.x, direction.y).normalized;
                SwipeDirection(direction2D);
            }
    }

    private void SwipeDirection(Vector2 direction)
    {   
        if (Vector2.Dot(Vector2.up, direction ) > directionTreshold) {
            Debug.Log("Swipe up");
        }
        else if (Vector2.Dot(Vector2.down, direction ) > directionTreshold) {
            Debug.Log("Swipe down");
        }
        else if (Vector2.Dot(Vector2.right, direction ) > directionTreshold) {
            Debug.Log("Swipe right");
        }
        else if (Vector2.Dot(Vector2.left, direction ) > directionTreshold) {
            Debug.Log("Swipe left");
        }
    } */
}
