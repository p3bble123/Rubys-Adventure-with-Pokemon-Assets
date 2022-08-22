using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pokeball : MonoBehaviour
{
    Rigidbody2D rigidbody2d;
    float timeToDestroy = 1.5f;
    
    void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
    }

    public void ThrowBall(Vector2 direction, float force)
    {
        rigidbody2d.AddForce(direction * force);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        EnemyController enemy = other.collider.GetComponent<EnemyController>();

        if (enemy != null) {
            enemy.Catch();
        }

        Destroy(gameObject);
    }


    // Update is called once per frame
    void Update()
    {
        if (timeToDestroy > 0) {
            timeToDestroy -= Time.deltaTime;
        } else {
            Destroy(gameObject);
        }
    }

}
