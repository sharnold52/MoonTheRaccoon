using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightCollider : MonoBehaviour
{
    // player reference
    PlayerController player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Equals("Player"))
        {
            player.CheckPlayerCaught();
        }
    }
}
