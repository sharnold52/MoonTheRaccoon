using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Public Variables
    public float speed = 3.0f;
    public GameObject noiseMakerPrefab;
    public float noisemakerCooldownTime = 5f;

    // Component References
    Rigidbody2D rBody2d;
    AudioSource audioSource;
    Animator animator;
    Collider2D playerCol;

    // Member Variables
    Vector2 movement;
    Vector2 lookDirection = new Vector2(0, -1);
    float noiseMakerInput = 0f;
    float interactInput = 0f;
    bool isThrown = false;
    bool isHidden = false;

    // Start is called before the first frame update
    void Start()
    {
        rBody2d = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        playerCol = GetComponent<Collider2D>();
    }

    // Handle Input
    void Update()
    {
        // check if player is hidden
        if (!isHidden)
        {
            // Store Input
            movement.x = Input.GetAxis("Horizontal");
            movement.y = Input.GetAxis("Vertical");
            movement = Vector2.ClampMagnitude(movement, 1.0f);
            noiseMakerInput = Input.GetAxis("Firecracker");

            // Animation
            if (!Mathf.Approximately(movement.x, 0.0f) || !Mathf.Approximately(movement.y, 0.0f))
            {
                lookDirection.Set(movement.x, movement.y);
                lookDirection.Normalize();
            }

            animator.SetFloat("Speed", movement.magnitude);
            animator.SetFloat("LookX", lookDirection.x);
            animator.SetFloat("LookY", lookDirection.y);


            if (noiseMakerInput > 0 && isThrown == false)
            {
                Launch();
                isThrown = true;
                Invoke("NoisemakerCooldown", noisemakerCooldownTime);
            }
        }
        else
        {
            // Get interactInput
            interactInput = Input.GetAxis("Interact");

            if(interactInput > 0)
            {
                // exit hiding spot

            }
        }
    }

    // Handle Physics
    void FixedUpdate()
    {
        // Get Position
        Vector2 position = rBody2d.position;

        // Update Position
        position = position + movement * speed * Time.fixedDeltaTime;

        // Set Position
        rBody2d.MovePosition(position);
    }
    
    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    void Launch()
    {
        // check if firecracker would collide with obstacles at instantiation point
        LayerMask walls = LayerMask.GetMask("Obstacles");
        Vector3 spawnLocation = rBody2d.position + Vector2.up * 0.5f;
        if(Physics2D.OverlapCircle(spawnLocation, 0.3f, walls) != null)
        {
            spawnLocation = rBody2d.position;
        }

        // instantiate projectile and launch it
        GameObject projectileObject = Instantiate(noiseMakerPrefab, spawnLocation, Quaternion.identity);
        Firecracker projectile = projectileObject.GetComponent<Firecracker>();
        projectile.Launch(lookDirection, 300);

        // Trigger player animation
        animator.SetTrigger("Launch");
    }

    void NoisemakerCooldown()
    {
        isThrown = false;
    }

    // Hide and reveal player
    public void HidePlayer()
    {
        // hide sprite
        isHidden = true;
        playerCol.enabled = false;
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;

        // zero out movement
        movement = Vector2.zero;
    }

    public void RevealPlayer()
    {
        // hide sprite
        isHidden = false;
        playerCol.enabled = true;
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
        gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    // check if player got caught
    public void CheckPlayerCaught()
    {
        if (isHidden)
        {
            // player was not caught
        }
        else
        {
            // player was caught
            Debug.Log("Player was caught!");
        }
    }
}
