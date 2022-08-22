using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{    
    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;

    public float playerSpeed = 4.0f;
   
    public GameObject pokeballPrefab;
    public float throwCooldown = 1.0f;
    float currentThrowCooldown;

    public int maxHealth = 5;
    public int health{ get { return currentHealth; }}
    int currentHealth;

    public float timeInvincible = 2.0f;    
    bool isInvincible;
    float invincibleTimer;

    Animator animator;
    Vector2 lookDirection = new Vector2(1,0);

    private AudioSource audioSource;
    public AudioClip ThrowAudioSource;
    public AudioClip HitAudioSource;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        currentHealth = maxHealth;
        currentThrowCooldown = 0.0f;

        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        currentThrowCooldown = Mathf.Clamp(currentThrowCooldown - Time.deltaTime, 0, throwCooldown);
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        
        Vector2 move = new Vector2(horizontal, vertical);

        if(!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f)){
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }

        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        if (isInvincible) {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0) {
                isInvincible = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            if (Mathf.Approximately(currentThrowCooldown, 0.0f)) {
                ThrowBall();
                audioSource.PlayOneShot(ThrowAudioSource);
            }
        }

        if (Input.GetKeyDown(KeyCode.X)) {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if(hit.collider != null){
                NonPlayerCharacter npc = hit.collider.GetComponent<NonPlayerCharacter>();
                if (npc != null) {
                    npc.DisplayDialog();
                }
            }


        }
    }

    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;
        
        position.x = position.x + playerSpeed * horizontal * Time.deltaTime;
        position.y = position.y + playerSpeed * vertical * Time.deltaTime;
        
        rigidbody2d.MovePosition(position);
    }

    public void ChangeHealth(int amount)
    {
        // is damage
        if ( amount < 0 ) {     
            animator.SetTrigger("Hit");
            if ( isInvincible ) {
                // take no damage
                return;
            }

            // will take damage, so give invincible time
            isInvincible = true;
            invincibleTimer = timeInvincible;
            audioSource.PlayOneShot(HitAudioSource);
        }

        // compute damage/heal
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UIHealthBar.instance.SetValue(currentHealth/(float)maxHealth);        
            
    }


    void ThrowBall()
    {
        GameObject pokeballObject = Instantiate(pokeballPrefab, rigidbody2d.position, Quaternion.identity);
        Pokeball pokeball = pokeballObject.GetComponent<Pokeball>();

        pokeball.ThrowBall(lookDirection, 300); 
        currentThrowCooldown = throwCooldown;
    }

    public void PlaySound(AudioClip clip) {
        audioSource.PlayOneShot(clip);
    }
}
