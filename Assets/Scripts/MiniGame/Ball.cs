using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ball : MonoBehaviour
{
    public Rigidbody2D rigidBody2D;

    public float speed = 300;

    private Vector2 velocity;

    // Start is called before the first frame update
    void Start()
    {
        velocity.x = Random.Range(-1f, 1f);

        velocity.y =1;

        rigidBody2D.AddForce(velocity*speed);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

     // when the GameObjects collider arrange for this GameObject to travel to the left of the screen
    void OnTriggerEnter2D(Collider2D col)
    {
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    } 

     // when the GameObjects collider arrange for this GameObject to travel to the left of the screen
    void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.CompareTag("Brick")) {
            Destroy(col.gameObject);
        }
        
    }
}
