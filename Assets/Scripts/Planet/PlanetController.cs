using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlanetController : MonoBehaviour
{

    #region ParallaxEffect
    [SerializeField]
    private ParallaxEffect pole;
    [SerializeField]
    private ParallaxEffect land;
    [SerializeField]
    private ParallaxEffect cloud;
    [SerializeField]
    private ParallaxEffect water;
    #endregion

    private InputManager inputManager;

    private Vector2 startPosition;
    private float startTime;
    private Vector2 endPosition;
    private float endTime;

    [SerializeField]
    private Sprite[] _skins;
    [SerializeField]
    private SpriteRenderer planetSkin;
    public int PlanetSkins {
        get {return _skins.Length;}
        set {
                if(planetSkin.sprite == _skins[value]) {
                    planetSkin.sprite = null;
                } else {
                    planetSkin.sprite = _skins[value];
                }
            }
    }

    [SerializeField]
    private GameController gameController;
    
    /* 
    [SerializeField]
    private float minimunDistance = .2f;
    [SerializeField]
    private float maximumTime = 1f;
    [SerializeField]
    private float directionTreshold = 0.9f;

    private bool planetSwiped = false; */

    private void Awake() {
        inputManager = InputManager.Instance;
    }

    private void OnEnable() {
        inputManager.OnStartTouch += Click;
        inputManager.OnEndTouch += Release;
        //inputManager.OnStartTouch += SwipeStart;
        //inputManager.OnEndTouch += SwipeEnd;

    }

    // Update is called once per frame
    private void OnDisable()
    {
        inputManager.OnStartTouch -= Click;
        inputManager.OnEndTouch -= Release;
        //inputManager.OnStartTouch -= SwipeStart;
        //inputManager.OnEndTouch -= SwipeEnd;
    }

    public void Click(Vector2 mousePos, float time) {
        /* RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
        if (hit.collider != null) {
            Debug.Log(hit.collider.tag);
        } */
    }

    public void Release(Vector2 mousePos, float time) {        
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
        if (hit.collider != null) {
            if(hit.collider.gameObject.CompareTag("Planet") ) {
                gameController.Inteaction();
                LeanTween.cancel(gameObject);
                transform.localScale = new Vector3(1f, 1f, 1f);
                LeanTween.scale(gameObject, new Vector3(1.2f, 1.2f), 1.2f).setEase(LeanTweenType.punch);
            }
        }
    }

   /*  public void SwipeStart(Vector2 position, float time) {
        startPosition = position;
        startTime = time;
        RaycastHit2D hit = Physics2D.Raycast(startPosition, Vector2.zero);
        if (hit.collider != null) {
            if(hit.collider.gameObject.CompareTag("Planet")) {
                planetSwiped = true;
            }
        }
    }


    public void SwipeEnd(Vector2 position, float time) {
        endPosition = position;
        endTime = time;
        DetectSwipe();
    }

    private void DetectSwipe() {
        if (Vector3.Distance(startPosition, endPosition) >= minimunDistance &&
            (endTime - startTime) <= maximumTime) {
                //Debug.DrawLine(startPosition, endPosition, Color.red, 5);
                Vector3 direction = endPosition - startPosition;
                Vector2 direction2D = new Vector2(direction.x, direction.y).normalized;
                //SwipeDirection(direction2D);
            }
        else {
            planetSwiped = false;
        }
    }

    private void SwipeDirection(Vector2 direction)
    {   
        if(planetSwiped) {
            if (Vector2.Dot(Vector2.up, direction ) > directionTreshold) {
                Debug.Log("Swipe up");
            }
            else if (Vector2.Dot(Vector2.down, direction ) > directionTreshold) {
                Debug.Log("Swipe down");
            }
            else if (Vector2.Dot(Vector2.right, direction ) > directionTreshold) {
                RotatePlanet(0.5f);
            }
            else if (Vector2.Dot(Vector2.left, direction ) > directionTreshold) {
                RotatePlanet(-0.5f);
            }
        }
    }

    private void RotatePlanet(float newSpeed) {
        pole.speed = newSpeed;
        land.speed = newSpeed;
        cloud.speed = newSpeed;
        water.speed = newSpeed;
        planetSwiped = false;
    } */
}
