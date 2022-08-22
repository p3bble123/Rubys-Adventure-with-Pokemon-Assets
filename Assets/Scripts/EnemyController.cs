using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float enemySpeed = 3.0f;
    public bool vertical;
    public int maxEnemyHP = 5;
    public ParticleSystem smokeEffect;

    private AudioSource audioSource;
    public AudioClip CaughtAudioSource;

    int enemyHP;

    //reverse movement every 3.0 seconds
    public float changeTime = 3.0f;
    

    Rigidbody2D rigidbody2D;
    float timer;
    int direction = 1;

    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>(); 

        timer = changeTime;
        enemyHP = maxEnemyHP;    
    }

    // Update is called once per frame
    void Update()
    {   
        if ( enemyHP == 0 ) {
            return;
        }

        timer -= Time.deltaTime;
        if (timer < 0) {
            direction = -direction;
            timer = changeTime;
        }
    }

    void FixedUpdate()
    {
        if ( enemyHP == 0 ) {
            return;
        }

        Vector2 position = rigidbody2D.position;

        if (vertical) {
            animator.SetFloat("Move X", 0);
            animator.SetFloat("Move Y", direction);
            position.y = position.y + Time.deltaTime * enemySpeed * direction;
        } 
        else {
            animator.SetFloat("Move X", direction);
            animator.SetFloat("Move Y", 0);
            position.x = position.x + Time.deltaTime * enemySpeed * direction;
        }

        rigidbody2D.MovePosition(position);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        PlayerController player = other.gameObject.GetComponent<PlayerController>();

        if (player != null) {
            player.ChangeHealth(-1);
        }
    }

    public void Catch() {
        enemyHP = 0;
        rigidbody2D.simulated = false;
        animator.SetTrigger("IsCaptivated");
        smokeEffect.Stop();
        audioSource.PlayOneShot(CaughtAudioSource);
        Invoke("StopAudioSource", audioSource.clip.length - 0.8f);
        // Debug.Log(audioSource.clip.length);
    }

    void StopAudioSource() {
        // Debug.Log("Stopped Audio");
        audioSource.Stop();
    }
}
