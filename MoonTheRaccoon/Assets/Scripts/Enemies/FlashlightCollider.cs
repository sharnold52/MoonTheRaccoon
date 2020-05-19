using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightCollider : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Equals("Player"))
        {
            Debug.Log("Player was caught!");
        }
    }
}
