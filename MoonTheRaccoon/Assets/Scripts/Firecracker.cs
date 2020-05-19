using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firecracker : MonoBehaviour
{
    public float noiseTime = 5f;


    // rigidbody and trigger
    Rigidbody2D rigidBody2d;
    CircleCollider2D trigger;
    AIDestinationSetter destination;
    Path enemyPath;
    bool isEnabled = false;

    // Start is called before the first frame update
    void Awake()
    {
        rigidBody2d = GetComponent<Rigidbody2D>();
        trigger = GetComponent<CircleCollider2D>();
        trigger.enabled = false;
    }

    // Called 
    void FixedUpdate()
    {
        if (rigidBody2d.velocity.magnitude <= 1f && !isEnabled)
        {
            trigger.enabled = true;

            // destroy the object after X seconds
            Invoke("DeleteFirecracker", noiseTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Equals("Enemy"))
        {
            enemyPath = other.GetComponent<Path>();
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
