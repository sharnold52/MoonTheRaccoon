using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firecracker : MonoBehaviour
{
    public float noiseTime = 5f;


    // rigidbody and trigger
    Rigidbody2D rigidBody2d;
    CircleCollider2D trigger;

    // pathing
    AIDestinationSetter destination;
    EnemyPath enemyPath;

    // variables for enabling the trigger and particle system
    bool isEnabled = false;
    bool inMotion = false;
    ParticleSystem pSystem;
    ParticleSystem.EmissionModule particles;

    // Start is called before the first frame update
    void Awake()
    {
        rigidBody2d = GetComponent<Rigidbody2D>();
        trigger = GetComponent<CircleCollider2D>();
        trigger.enabled = false;
        pSystem = gameObject.GetComponent<ParticleSystem>();
        particles = pSystem.emission;
        particles.enabled = false;
    }

    // Called 
    void FixedUpdate()
    {
        if (rigidBody2d.velocity.magnitude <= 0.4f && !isEnabled && inMotion)
        {
            // enable trigger box and particle system
            trigger.enabled = true;
            isEnabled = true;
            particles = pSystem.emission;
            particles.enabled = true;

            // destroy the object after X seconds
            Invoke("DeleteFirecracker", noiseTime);
        }
        else if(rigidBody2d.velocity.magnitude >= 0.4f)
        {
            inMotion = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Equals("Enemy"))
        {
            enemyPath = other.GetComponent<EnemyPath>();
            enemyPath.ChaseNoisemaker(gameObject);
        }
    }

    public void Launch(Vector2 direction, float force)
    {
        rigidBody2d.AddForce(direction * force);
    }

    void DeleteFirecracker()
    {
        Destroy(gameObject);
    }
}
