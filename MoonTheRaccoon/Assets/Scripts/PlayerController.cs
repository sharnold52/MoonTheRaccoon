using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Public Variables
    public float speed = 3.0f;

    // Component References
    Rigidbody2D rBody2d;
    AudioSource audioSource;
    Animator animator;

    // Member Variables
    Vector2 movement;
    Vector2 lookDirection = new Vector2(0, -1);

    // Start is called before the first frame update
    void Start()
    {
        rBody2d = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
    }

    // Handle Input
    void Update()
    {
        // Store Input
        movement.x = Input.GetAxis("Horizontal");
        movement.y = Input.GetAxis("Vertical");
        movement = Vector2.ClampMagnitude(movement, 1.0f);

        // Animation
        if (!Mathf.Approximately(movement.x, 0.0f) || !Mathf.Approximately(movement.y, 0.0f))
        {
            lookDirection.Set(movement.x, movement.y);
            lookDirection.Normalize();
        }

        animator.SetFloat("Speed", movement.magnitude);
        animator.SetFloat("LookX", lookDirection.x);
        animator.SetFloat("LookY", lookDirection.y);

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
}
