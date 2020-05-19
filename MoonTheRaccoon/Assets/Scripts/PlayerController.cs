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

    // Member Variables
    Vector2 movement;
    Vector2 lookDirection = new Vector2(0, -1);
    float noiseMakerInput = 0f;
    bool isThrown = false;

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
        GameObject projectileObject = Instantiate(noiseMakerPrefab, rBody2d.position + Vector2.up * 0.5f, Quaternion.identity);

        Firecracker projectile = projectileObject.GetComponent<Firecracker>();
        projectile.Launch(lookDirection, 300);

        animator.SetTrigger("Launch");
    }

    void NoisemakerCooldown()
    {
        isThrown = false;
    }
}
