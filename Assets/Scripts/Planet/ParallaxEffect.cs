using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    public Transform sprite;
    public Transform cloneSprite;
    public float speed;

    Vector3 startPos = new Vector3(-10.76f, 0 , 0);
    Vector3 endPos = new Vector3(10.76f, 0 , 0);
    Vector3 middlePos = new Vector3(0, 0 , 0);

    private float startSpeed;
    private float t = 0.0f;

    void Awake() {
        startSpeed = speed;
    }
    

    void Update()
    {

        if(startSpeed != speed) {
            if(speed > 0) {
                speed = Mathf.Lerp(speed, startSpeed, t);
                t += 0.2f * Time.deltaTime;
            } else {
                speed = Mathf.Lerp(speed, -startSpeed, t);
                t += 0.2f * Time.deltaTime;
            }
        } else {
            t = 0;
        }

        sprite.transform.position = new Vector3(sprite.transform.position.x + speed * Time.deltaTime, sprite.transform.position.y, sprite.transform.position.z);
        cloneSprite.transform.position = new Vector3(cloneSprite.transform.position.x + speed * Time.deltaTime, cloneSprite.transform.position.y, cloneSprite.transform.position.z);


        if (sprite.transform.localPosition.x >= endPos.x ) {
            sprite.transform.localPosition = startPos;
            cloneSprite.localPosition = middlePos;
        } else if (sprite.transform.localPosition.x <= startPos.x) {
            sprite.transform.localPosition = endPos;
            cloneSprite.localPosition = middlePos;
        }

        if (cloneSprite.transform.localPosition.x >= endPos.x ) {
            cloneSprite.transform.localPosition = startPos;
            sprite.localPosition = middlePos;
        } else if (cloneSprite.transform.localPosition.x <= startPos.x) {
            cloneSprite.transform.localPosition = endPos;
            sprite.localPosition = middlePos;
        }
    }
}
